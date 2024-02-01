using Google.Apis.Dialogflow.v2.Data;
using Newtonsoft.Json.Linq;
using ProjectIvy.Business.Constants;
using ProjectIvy.Business.Handlers.Car;
using ProjectIvy.Business.Handlers.Consumation;
using ProjectIvy.Business.Handlers.Expense;
using ProjectIvy.Business.Handlers.Movie;
using ProjectIvy.Business.Handlers.Tracking;
using ProjectIvy.Business.Handlers.User;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Model.Binding.Car;
using ProjectIvy.Model.Binding.Consumation;
using ProjectIvy.Model.Binding.Expense;
using ProjectIvy.Model.Binding.Movie;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectIvy.Business.Handlers.Webhooks
{
    public class DialogflowHandler : Handler<DialogflowHandler>, IDialogflowHandler
    {
        private readonly ICarHandler _carHandler;
        private readonly IConsumationHandler _consumationHandler;
        private readonly IExpenseHandler _expenseHandler;
        private readonly IMovieHandler _movieHandler;
        private readonly ITrackingHandler _trackingHandler;
        private readonly IUserHandler _userHandler;

        public DialogflowHandler(IHandlerContext<DialogflowHandler> context,
                                 ICarHandler carHandler,
                                 IConsumationHandler consumationHandler,
                                 IExpenseHandler expenseHandler,
                                 IMovieHandler movieHandler,
                                 ITrackingHandler trackingHandler,
                                 IUserHandler userHandler) : base(context)
        {
            _carHandler = carHandler;
            _consumationHandler = consumationHandler;
            _expenseHandler = expenseHandler;
            _movieHandler = movieHandler;
            _trackingHandler = trackingHandler;
            _userHandler = userHandler;
        }

        public async Task<GoogleCloudDialogflowV2WebhookResponse> ProcessWebhook(GoogleCloudDialogflowV2WebhookRequest request)
        {
            switch (request.QueryResult.Intent.Name)
            {
                case DialogflowIntents.CreateExpense:
                    return await CreateExpense(request);

                case DialogflowIntents.GetConsecutiveConsumationDays:
                    return await GetConsecutiveConsumationDays(request);

                case DialogflowIntents.GetDistance:
                    return await GetDistance(request);

                case DialogflowIntents.GetTopSpeed:
                    return await GetTopSpeed(request);

                case DialogflowIntents.GetLatestOdometer:
                    return await GetLatestOdometer();

                case DialogflowIntents.SetLatestOdometer:
                    return await SetLatestOdometer(request);

                case DialogflowIntents.GetConsumationCount:
                    return await GetConsumationCount(request);

                case DialogflowIntents.GetConsumationSum:
                    return await GetConsumationSum(request);

                case DialogflowIntents.GetExpenseSum:
                    return await GetExpenseSum(request);

                case DialogflowIntents.SetWeight:
                    return await SetWeight(request);

                case DialogflowIntents.GetMovieCount:
                    return await GetMovieCount(request);

                default:
                    return new GoogleCloudDialogflowV2WebhookResponse()
                    {
                        FulfillmentText = "Unknown intent"
                    };
            }
        }

        public async Task<GoogleCloudDialogflowV2WebhookResponse> CreateExpense(GoogleCloudDialogflowV2WebhookRequest request)
        {
            var unitCurrency = (JObject)request.QueryResult.Parameters["unit-currency"];
            string currencyId = (string)unitCurrency["currency"];

            var user = _userHandler.Get(UserId);

            var binding = new ExpenseBinding()
            {
                Amount = Convert.ToDecimal(unitCurrency["amount"]),
                CurrencyId = string.IsNullOrEmpty(currencyId) ? user.DefaultCurrency.Code : currencyId,
                Comment = (string)request.QueryResult.Parameters["description"],
                Date = (DateTime)request.QueryResult.Parameters["date"],
                ExpenseTypeId = ((string)request.QueryResult.Parameters["expense-type"]).Replace(" ", "-"),
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

        public async Task<GoogleCloudDialogflowV2WebhookResponse> GetConsumationCount(GoogleCloudDialogflowV2WebhookRequest request)
        {
            var binding = new ConsumationGetBinding(request.ToFilteredBinding());

            int count = _consumationHandler.Count(binding);

            return new GoogleCloudDialogflowV2WebhookResponse()
            {
                FulfillmentText = $"You've drank {count} beers."
            };
        }

        public async Task<GoogleCloudDialogflowV2WebhookResponse> GetConsumationSum(GoogleCloudDialogflowV2WebhookRequest request)
        {
            var binding = new ConsumationGetBinding(request.ToFilteredBinding());

            int sum = _consumationHandler.SumVolume(binding);

            return new GoogleCloudDialogflowV2WebhookResponse()
            {
                FulfillmentText = $"You've drank {sum / 1000} liters."
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

            binding.TypeId = request.QueryResult.Parameters.ContainsKey("ExpenseType") ? ((JArray)request.QueryResult.Parameters["ExpenseType"]).Select(x => ((string)x).Replace(" ", "-")) : null;

            decimal sum = await _expenseHandler.SumAmount(binding);

            var user = _userHandler.Get(UserId);

            return new GoogleCloudDialogflowV2WebhookResponse()
            {
                FulfillmentText = $"You've spent {sum} {user.DefaultCurrency.Code}"
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

        public async Task<GoogleCloudDialogflowV2WebhookResponse> GetMovieCount(GoogleCloudDialogflowV2WebhookRequest request)
        {
            var filteredBinding = request.ToFilteredBinding();
            var movieGetBinding = new MovieGetBinding()
            {
                From = filteredBinding.From,
                To = filteredBinding.To
            };
            int movieCount = _movieHandler.Count(movieGetBinding);

            return new GoogleCloudDialogflowV2WebhookResponse()
            {
                FulfillmentText = $"You've watched {movieCount} movies."
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
                return $"{Math.Round((decimal)distanceInMeters / 1000, 1)}km";
            }
            else
            {
                return $"{Math.Floor((decimal)distanceInMeters / 1000)}km";
            }
        }
    }
}
