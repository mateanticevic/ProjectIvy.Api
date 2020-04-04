using Google.Apis.Dialogflow.v2.Data;
using Newtonsoft.Json.Linq;
using ProjectIvy.Business.Handlers.Car;
using ProjectIvy.Business.Handlers.Expense;
using ProjectIvy.Model.Binding.Car;
using ProjectIvy.Model.Binding.Expense;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectIvy.Business.Handlers.Webhooks
{
    public class DialogflowHandler : Handler<DialogflowHandler>, IDialogflowHandler
    {
        private readonly ICarHandler _carHandler;
        private readonly IExpenseHandler _expenseHandler;

        public DialogflowHandler(IHandlerContext<DialogflowHandler> context, ICarHandler carHandler, IExpenseHandler expenseHandler) : base(context)
        {
            _carHandler = carHandler;
            _expenseHandler = expenseHandler;
        }

        public async Task<GoogleCloudDialogflowV2WebhookResponse> ProcessWebhook(GoogleCloudDialogflowV2WebhookRequest request)
        {
            switch (request.QueryResult.Intent.Name)
            {
                case "projects/projectivy-rkgwxr/agent/intents/82855d04-184d-43f7-bc39-c594a9dc5773":
                    return await SetLatestOdometer(request);
                case "projects/projectivy-rkgwxr/agent/intents/a26b869b-23ff-4426-9158-8566fffc843b":
                    return await GetExpenseSum(request);
                default:
                    return await GetLatestOdometer();
            }

            throw new NotImplementedException();
        }

        public async Task<GoogleCloudDialogflowV2WebhookResponse> GetExpenseSum(GoogleCloudDialogflowV2WebhookRequest request)
        {
            var datePeriod = request.QueryResult.Parameters["date-period"];
            var dateTime = request.QueryResult.Parameters["date-time"];

            var binding = new ExpenseSumGetBinding()
            {
                From = dateTime is DateTime fromDate ? fromDate.Date : (DateTime)((JObject)datePeriod)["startDate"],
                To = dateTime is DateTime toDate ? toDate.Date : (DateTime)((JObject)datePeriod)["endDate"]
            };

            decimal sum = await _expenseHandler.SumAmount(binding);

            return new GoogleCloudDialogflowV2WebhookResponse()
            {
                FulfillmentText = $"You've spent {sum} {User.DefaultCurrency.Code}"
            };
        }

        public async Task<GoogleCloudDialogflowV2WebhookResponse> GetLatestOdometer()
        {
            var carLog = _carHandler.GetLatestLog(new CarLogGetBinding() { HasOdometer = true });

            return new GoogleCloudDialogflowV2WebhookResponse()
            {
                FulfillmentText = $"Latest odometer value is {carLog.Odometer} kilometers."
            };
        }

        public async Task<GoogleCloudDialogflowV2WebhookResponse> SetLatestOdometer(GoogleCloudDialogflowV2WebhookRequest request)
        {
            var odometer = (JObject)request.QueryResult.Parameters.FirstOrDefault().Value;
            var carLog = new CarLogBinding()
            {
                Odometer = (int)odometer["amount"]
            };
            _carHandler.CreateLog(carLog);

            return new GoogleCloudDialogflowV2WebhookResponse();
        }
    }
}
