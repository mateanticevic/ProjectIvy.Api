using Google.Apis.Dialogflow.v2.Data;
using Newtonsoft.Json.Linq;
using ProjectIvy.Business.Handlers.Car;
using ProjectIvy.Business.Handlers.Consumation;
using ProjectIvy.Business.Handlers.Expense;
using ProjectIvy.Business.Handlers.Tracking;
using ProjectIvy.Business.Handlers.User;
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
        private readonly IUserHandler _userHandler;

        public DialogflowHandler(IHandlerContext<DialogflowHandler> context,
                                 ICarHandler carHandler,
                                 IConsumationHandler consumationHandler,
                                 IExpenseHandler expenseHandler,
                                 ITrackingHandler trackingHandler,
                                 IUserHandler userHandler) : base(context)
        {
            _carHandler = carHandler;
            _consumationHandler = consumationHandler;
            _expenseHandler = expenseHandler;
            _trackingHandler = trackingHandler;
            _userHandler = userHandler;
        }

        public async Task<GoogleCloudDialogflowV2WebhookResponse> ProcessWebhook(GoogleCloudDialogflowV2WebhookRequest request)
        {
            switch (request.QueryResult.Intent.Name)
            {
                case "projects/projectivy-rkgwxr/agent/intents/f55a44e6-65c8-44c8-a418-e1f535c7bb89":
                    return await CreateExpense(request);
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
                case "projects/projectivy-rkgwxr/agent/intents/970e9c94-06f0-482f-b459-568af5b631e8":
                    return await SetWeight(request);
                default:
                    return await GetLatestOdometer();
            }

            throw new NotImplementedException();
        }

        public async Task<GoogleCloudDialogflowV2WebhookResponse> CreateExpense(GoogleCloudDialogflowV2WebhookRequest request)
        {
            var unitCurrency = (JObject)request.QueryResult.Parameters["unit-currency"];
            string currencyId = (string)unitCurrency["currency"];

            var binding = new ExpenseBinding()
            {
                Amount = Convert.ToDecimal(unitCurrency["amount"]),
                CurrencyId = string.IsNullOrEmpty(currencyId) ? User.DefaultCurrency.Code : currencyId,
                Comment = (string)request.QueryResult.Parameters["description"],
                Date = (DateTime)request.QueryResult.Parameters["date"],
                ExpenseTypeId = (string)request.QueryResult.Parameters["expense-type"],
                NeedsReview = true,
                PaymentTypeId = (string)request.QueryResult.Parameters["payment-type"]
            };

            _expenseHandler.Create(binding);

            return new GoogleCloudDialogflowV2WebhookResponse();
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
                FulfillmentText = $"You covered {FormatDistance(distance)}"
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

        public async Task<GoogleCloudDialogflowV2WebhookResponse> SetWeight(GoogleCloudDialogflowV2WebhookRequest request)
        {
            var unitWeight = (JObject)request.QueryResult.Parameters["unit-weight"];
            await _userHandler.SetWeight((decimal)unitWeight["amount"]);

            return new GoogleCloudDialogflowV2WebhookResponse();
        }

        private string FormatDistance(int distanceInMeters)
        {
            if (distanceInMeters < 1000)
            {
                return $"{distanceInMeters}m";
            }
            else if (distanceInMeters < 10000)
            {
                return $"{Math.Round((decimal)distanceInMeters/1000, 1)}km";
            }
            else
            {
                return $"{Math.Floor((decimal)distanceInMeters / 1000)}km";
            }
        }
    }
}
