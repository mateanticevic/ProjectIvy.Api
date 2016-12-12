using AnticevicApi.BL.Handlers.Airport;
using AnticevicApi.BL.Handlers.Application;
using AnticevicApi.BL.Handlers.Car;
using AnticevicApi.BL.Handlers.Currency;
using AnticevicApi.BL.Handlers.Expense;
using AnticevicApi.BL.Handlers.Income;
using AnticevicApi.BL.Handlers.Movie;
using AnticevicApi.BL.Handlers.Poi;
using AnticevicApi.BL.Handlers.Project;
using AnticevicApi.BL.Handlers.Task;
using AnticevicApi.BL.Handlers.Tracking;
using AnticevicApi.BL.Handlers.User;
using AnticevicApi.BL.Handlers.Vendor;
using AnticevicApi.BL.Handlers;
using AnticevicApi.Config;
using AnticevicApi.Model.Constants;
using AnticevicApi.Model.Database.Main.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AnticevicApi.Controllers
{
    public abstract class BaseController<TController> : Controller
    {
        private IAirportHandler _airportHandler;
        private IApplicationHandler _applicationHandler;
        private ICarHandler _carHandler;
        private ICurrencyHandler _currencyHandler;
        private IExpenseHandler _expenseHandler;
        private IExpenseTypeHandler _expenseTypeHandler;
        private IIncomeHandler _incomeHandler;
        private IMovieHandler _movieHandler;
        private IPoiHandler _poiHandler;
        private IProjectHandler _projectHandler;
        private SecurityHandler _securityHandler;
        private ITaskHandler _taskHandler;
        private ITrackingHandler _trackingHandler;
        private IVendorHandler _vendorHandler;
        private IUserHandler _userHandler;

        private IOptions<AppSettings> options;

        public BaseController(IOptions<AppSettings> settingsAccessor, ILogger<TController> logger)
        {
            logger.LogInformation((int)LogEvent.ControllerInstance, nameof(LogEvent.ControllerInstance));

            Logger = logger;
            Settings = settingsAccessor.Value;
        }

        protected AppSettings Settings { get; set; }

        protected ILogger<TController> Logger { get; private set; } 

        #region Handlers

        protected AccessToken AccessToken
        {
            get
            {
                return (AccessToken)HttpContext.Items["AccessToken"];
            }
        }

        protected SecurityHandler SecurityHandler
        {
            get
            {
                if (_securityHandler == null)
                {
                    _securityHandler = new SecurityHandler(Settings.ConnectionStrings.Main, AccessToken.UserId);
                }

                return _securityHandler;
            }
        }

        protected IAirportHandler AirportHandler
        {
            get
            {
                _airportHandler.Initialize(Settings.ConnectionStrings.Main, AccessToken.UserId, Logger);
                return _airportHandler;
            }
            set
            {
                _airportHandler = value;
            }
        }

        protected IApplicationHandler ApplicationHandler
        {
            get
            {
                _applicationHandler.Initialize(Settings.ConnectionStrings.Main, AccessToken.UserId, Logger);
                return _applicationHandler;
            }
            set
            {
                _applicationHandler = value;
            }
        }

        protected ICarHandler CarHandler
        {
            get
            {
                _carHandler.Initialize(Settings.ConnectionStrings.Main, AccessToken.UserId, Logger);
                return _carHandler;
            }
            set
            {
                _carHandler = value;
            }
        }

        protected ICurrencyHandler CurrencyHandler
        {
            get
            {
                _currencyHandler.Initialize(Settings.ConnectionStrings.Main, AccessToken.UserId, Logger);
                return _currencyHandler;
            }
            set
            {
                _currencyHandler = value;
            }
        }

        protected IExpenseHandler ExpenseHandler
        {
            get
            {
                _expenseHandler.Initialize(Settings.ConnectionStrings.Main, AccessToken.UserId, Logger);
                return _expenseHandler;
            }
            set
            {
                _expenseHandler = value;
            }
        }

        protected IExpenseTypeHandler ExpenseTypeHandler
        {
            get
            {
                _expenseTypeHandler.Initialize(Settings.ConnectionStrings.Main, AccessToken.UserId, Logger);
                return _expenseTypeHandler;
            }
            set
            {
                _expenseTypeHandler = value;
            }
        }

        protected IIncomeHandler IncomeHandler
        {
            get
            {
                _incomeHandler.Initialize(Settings.ConnectionStrings.Main, AccessToken.UserId, Logger);
                return _incomeHandler;
            }
            set
            {
                _incomeHandler = value;
            }
        }

        protected IMovieHandler MovieHandler
        {
            get
            {
                _movieHandler.Initialize(Settings.ConnectionStrings.Main, AccessToken.UserId, Logger);
                return _movieHandler;
            }
            set
            {
                _movieHandler = value;
            }
        }

        protected IPoiHandler PoiHandler
        {
            get
            {
                _poiHandler.Initialize(Settings.ConnectionStrings.Main, AccessToken.UserId, Logger);
                return _poiHandler;
            }
            set
            {
                _poiHandler = value;
            }
        }

        protected IProjectHandler ProjectHandler
        {
            get
            {
                _projectHandler.Initialize(Settings.ConnectionStrings.Main, AccessToken.UserId, Logger);
                return _projectHandler;
            }
            set
            {
                _projectHandler = value;
            }
        }

        protected ITaskHandler TaskHandler
        {
            get
            {
                _taskHandler.Initialize(Settings.ConnectionStrings.Main, AccessToken.UserId, Logger);
                return _taskHandler;
            }
            set
            {
                _taskHandler = value;
            }
        }

        protected ITrackingHandler TrackingHandler
        {
            get
            {
                _trackingHandler.Initialize(Settings.ConnectionStrings.Main, AccessToken.UserId, Logger);
                return _trackingHandler;
            }
            set
            {
                _trackingHandler = value;
            }
        }

        protected IVendorHandler VendorHandler
        {
            get
            {
                _vendorHandler.Initialize(Settings.ConnectionStrings.Main, AccessToken.UserId, Logger);
                return _vendorHandler;
            }
            set
            {
                _vendorHandler = value;
            }
        }

        protected IUserHandler UserHandler
        {
            get
            {
                _userHandler.Initialize(Settings.ConnectionStrings.Main, AccessToken.UserId, Logger);
                return _userHandler;
            }
            set
            {
                _userHandler = value;
            }
        }

        #endregion
    }
}
