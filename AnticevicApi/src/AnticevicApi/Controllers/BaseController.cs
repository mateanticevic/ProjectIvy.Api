using AnticevicApi.BL.Handlers;
using AnticevicApi.Config;
using AnticevicApi.Model.Database.Main.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AnticevicApi.Controllers
{
    public abstract class BaseController : Controller
    {
        private AirportHandler _airportHandler;
        private ApplicationHandler _applicationHandler;
        private CarHandler _carHandler;
        private CurrencyHandler _currencyHandler;
        private ExpenseHandler _expenseHandler;
        private ExpenseTypeHandler _expenseTypeHandler;
        private IncomeHandler _incomeHandler;
        private MovieHandler _movieHandler;
        private PoiHandler _poiHandler;
        private ProjectHandler _projectHandler;
        private SecurityHandler _securityHandler;
        private TaskHandler _taskHandler;
        private TrackingHandler _trackingHandler;
        private VendorHandler _vendorHandler;

        public BaseController(IOptions<AppSettings> optionsAccessor)
        {
            Settings = optionsAccessor.Value;
        }

        protected AppSettings Settings { get; set; }

        #region Handlers

        protected ApplicationHandler ApplicationHandler
        {
            get
            {
                if (_applicationHandler == null)
                {
                    _applicationHandler = new ApplicationHandler(Settings.ConnectionStrings.Main, AccessToken.UserId);
                }

                return _applicationHandler;
            }
        }

        protected AccessToken AccessToken
        {
            get
            {
                return (AccessToken)HttpContext.Items["AccessToken"];
            }
        }

        protected AirportHandler AirportHandler
        {
            get
            {
                if(_airportHandler == null)
                {
                    _airportHandler = new AirportHandler(Settings.ConnectionStrings.Main, AccessToken.UserId);
                }

                return _airportHandler;
            }
        }

        protected CarHandler CarHandler
        {
            get
            {
                if (_carHandler == null)
                {
                    _carHandler = new CarHandler(Settings.ConnectionStrings.Main, AccessToken.UserId);
                }

                return _carHandler;
            }
        }

        protected CurrencyHandler CurrencyHandler
        {
            get
            {
                if (_currencyHandler == null)
                {
                    _currencyHandler = new CurrencyHandler(Settings.ConnectionStrings.Main, AccessToken.UserId);
                }

                return _currencyHandler;
            }
        }

        protected ExpenseHandler ExpenseHandler
        {
            get
            {
                if (_expenseHandler == null)
                {
                    _expenseHandler = new ExpenseHandler(Settings.ConnectionStrings.Main, AccessToken.UserId);
                }

                return _expenseHandler;
            }
        }

        protected ExpenseTypeHandler ExpenseTypeHandler
        {
            get
            {
                if (_expenseTypeHandler == null)
                {
                    _expenseTypeHandler = new ExpenseTypeHandler(Settings.ConnectionStrings.Main, AccessToken.UserId);
                }

                return _expenseTypeHandler;
            }
        }

        protected MovieHandler MovieHandler
        {
            get
            {
                if (_movieHandler == null)
                {
                    _movieHandler = new MovieHandler(Settings.ConnectionStrings.Main, AccessToken.UserId);
                }

                return _movieHandler;
            }
        }

        protected IncomeHandler IncomeHandler
        {
            get
            {
                if (_incomeHandler == null)
                {
                    _incomeHandler = new IncomeHandler(Settings.ConnectionStrings.Main, AccessToken.UserId);
                }

                return _incomeHandler;
            }
        }

        protected PoiHandler PoiHandler
        {
            get
            {
                if (_poiHandler == null)
                {
                    _poiHandler = new PoiHandler(Settings.ConnectionStrings.Main, AccessToken.UserId);
                }

                return _poiHandler;
            }
        }

        protected ProjectHandler ProjectHandler
        {
            get
            {
                if (_projectHandler == null)
                {
                    _projectHandler = new ProjectHandler(Settings.ConnectionStrings.Main, AccessToken.UserId);
                }

                return _projectHandler;
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

        protected TaskHandler TaskHandler
        {
            get
            {
                if (_taskHandler == null)
                {
                    _taskHandler = new TaskHandler(Settings.ConnectionStrings.Main, AccessToken.UserId);
                }

                return _taskHandler;
            }
        }

        protected TrackingHandler TrackingHandler
        {
            get
            {
                if (_trackingHandler == null)
                {
                    _trackingHandler = new TrackingHandler(Settings.ConnectionStrings.Main, AccessToken.UserId);
                }

                return _trackingHandler;
            }
        }

        protected VendorHandler VendorHandler
        {
            get
            {
                if (_vendorHandler == null)
                {
                    _vendorHandler = new VendorHandler(Settings.ConnectionStrings.Main, AccessToken.UserId);
                }

                return _vendorHandler;
            }
        }

        #endregion
    }
}
