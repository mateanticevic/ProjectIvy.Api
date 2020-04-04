using Google.Apis.Dialogflow.v2.Data;
using Newtonsoft.Json.Linq;
using ProjectIvy.Business.Handlers.Car;
using ProjectIvy.Business.Handlers.Consumation;
using ProjectIvy.Business.Handlers.Expense;
using ProjectIvy.Business.Handlers.Tracking;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Model.Binding.Car;
using ProjectIvy.Model.Binding.Consumation;
using ProjectIvy.Model.Binding.Expense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectIvy.Business.Handlers.Webhooks
{
    public class DialogflowHandler : Handler<DialogflowHandler>, IDialogflowHandler
    {
        private readonly ICarHandler _carHandler;
        private readonly IConsumationHandler _consumationHandler;
        private readonly IExpenseHandler _expenseHandler;
        private readonly ITrackingHandler _trackingHandler;

        public DialogflowHandler(IHandlerContext<DialogflowHandler> context,
                                 ICarHandler carHandler,
                                 IConsumationHandler consumationHandler,
                                 IExpenseHandler expenseHandler,
                                 ITrackingHandler trackingHandler) : base(context)
        {
            _carHandler = carHandler;
            _consumationHandler = consumationHandler;
            _expenseHandler = expenseHandler;
            _trackingHandler = trackingHandler;
        }

        public async Task<GoogleCloudDialogflowV2WebhookResponse> ProcessWebhook(GoogleCloudDialogflowV2WebhookRequest request)
        {
            switch (request.QueryResult.Intent.Name)
            {
                case "projects/projectivy-rkgwxr/agent/intents/2f5e913e-036d-437f-b33e-4ab91e3fbdc7":
                    return await GetConsecutiveConsumationDays(request);
                case "projects/projectivy-rkgwxr/agent/intents/6aed36d0-3e1c-406b-9acf-39bc6fcece99":
                    return await GetDistance(request);
                case "projects/projectivy-rkgwxr/agent/intents/c7020a73-a387-4d04-8c3f-961e6de9f99a":
                    return await GetTopSpeed(request);
                case "projects/projectivy-rkgwxr/agent/intents/82855d04-184d-43f7-bc39-c594a9dc5773":
                    return await SetLatestOdometer(request);
                case "projects/projectivy-rkgwxr/agent/intents/45356f36-e342-4c71-ae0d-c9c06e3df76d":
                    return await GetConsumationSum(request);
                case "projects/projectivy-rkgwxr/agent/intents/a26b869b-23ff-4426-9158-8566fffc843b":
                    return await GetExpenseSum(request);
                default:
                    return await GetLatestOdometer();
            }

            throw new NotImplementedException();
        }

        public async Task<GoogleCloudDialogflowV2WebhookResponse> GetConsecutiveConsumationDays(GoogleCloudDialogflowV2WebhookRequest request)
        {
            var consecutive = _consumationHandler.ConsecutiveDates(new ConsumationGetBinding()).FirstOrDefault();

            return new GoogleCloudDialogflowV2WebhookResponse()
            {
                FulfillmentText = $"{consecutive.To.Subtract(consecutive.From).TotalDays} days, from {consecutive.From} to {consecutive.To}"
            };
        }
        
        public async Task<GoogleCloudDialogflowV2WebhookResponse> GetConsumationSum(GoogleCloudDialogflowV2WebhookRequest request)
        {
            var binding = new ConsumationGetBinding(request.ToFilteredBinding());

            int sum = _consumationHandler.SumVolume(binding);

            return new GoogleCloudDialogflowV2WebhookResponse()
            {
                FulfillmentText = $"You've drank {sum/1000} liters."
            };
        }

        public async Task<GoogleCloudDialogflowV2WebhookResponse> GetDistance(GoogleCloudDialogflowV2WebhookRequest request)
        {
            int distance = _trackingHandler.GetDistance(request.ToFilteredBinding(true));

            return new GoogleCloudDialogflowV2WebhookResponse()
            {
                FulfillmentText = $"You covered {Math.Ceiling((decimal)distance / 1000)}km"
            };
        }
        
        public async Task<GoogleCloudDialogflowV2WebhookResponse> GetExpenseSum(GoogleCloudDialogflowV2WebhookRequest request)
        {
            var binding = new ExpenseSumGetBinding(request.ToFilteredBinding());

            binding.TypeId = request.QueryResult.Parameters.ContainsKey("ExpenseType") ? ((JArray)request.QueryResult.Parameters["ExpenseType"]).Select(x => (string)x) : null;

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

        public async Task<GoogleCloudDialogflowV2WebhookResponse> GetTopSpeed(GoogleCloudDialogflowV2WebhookRequest request)
        {
            double maxSpeed = _trackingHandler.GetMaxSpeed(request.ToFilteredBinding(true));

            return new GoogleCloudDialogflowV2WebhookResponse()
            {
                FulfillmentText = $"Your top speed was {(int)(maxSpeed * 3.6)} km/h."
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
