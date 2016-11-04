using AnticevicApi.BL.Handlers;
using AnticevicApi.Model.Database.Main.Security;
using Microsoft.AspNetCore.Mvc;

namespace AnticevicApi.Controllers
{
    public abstract class BaseController : Controller
    {
        private AirportHandler _airportHandler;
        private ApplicationHandler _applicationHandler;
        private CarHandler _carHandler;
        private ExpenseHandler _expenseHandler;
        private ExpenseTypeHandler _expenseTypeHandler;
        private IncomeHandler _incomeHandler;
        private MovieHandler _movieHandler;
        private ProjectHandler _projectHandler;
        private SecurityHandler _securityHandler;
        private TaskHandler _taskHandler;
        private TrackingHandler _trackingHandler;

        public ApplicationHandler ApplicationHandler
        {
            get
            {
                if (_applicationHandler == null)
                {
                    _applicationHandler = new ApplicationHandler(AccessToken.UserId);
                }

                return _applicationHandler;
            }
        }

        public AccessToken AccessToken
        {
            get
            {
                return (AccessToken)HttpContext.Items["AccessToken"];
            }
        }

        public AirportHandler AirportHandler
        {
            get
            {
                if(_airportHandler == null)
                {
                    _airportHandler = new AirportHandler(AccessToken.UserId);
                }

                return _airportHandler;
            }
        }

        public CarHandler CarHandler
        {
            get
            {
                if (_carHandler == null)
                {
                    _carHandler = new CarHandler(AccessToken.UserId);
                }

                return _carHandler;
            }
        }

        public ExpenseHandler ExpenseHandler
        {
            get
            {
                if (_expenseHandler == null)
                {
                    _expenseHandler = new ExpenseHandler(AccessToken.UserId);
                }

                return _expenseHandler;
            }
        }

        public ExpenseTypeHandler ExpenseTypeHandler
        {
            get
            {
                if (_expenseTypeHandler == null)
                {
                    _expenseTypeHandler = new ExpenseTypeHandler(AccessToken.UserId);
                }

                return _expenseTypeHandler;
            }
        }

        public MovieHandler MovieHandler
        {
            get
            {
                if (_movieHandler == null)
                {
                    _movieHandler = new MovieHandler(AccessToken.UserId);
                }

                return _movieHandler;
            }
        }

        public IncomeHandler IncomeHandler
        {
            get
            {
                if (_incomeHandler == null)
                {
                    _incomeHandler = new IncomeHandler(AccessToken.UserId);
                }

                return _incomeHandler;
            }
        }

        public ProjectHandler ProjectHandler
        {
            get
            {
                if (_projectHandler == null)
                {
                    _projectHandler = new ProjectHandler(AccessToken.UserId);
                }

                return _projectHandler;
            }
        }

        public SecurityHandler SecurityHandler
        {
            get
            {
                if (_securityHandler == null)
                {
                    _securityHandler = new SecurityHandler(AccessToken.UserId);
                }

                return _securityHandler;
            }
        }

        public TaskHandler TaskHandler
        {
            get
            {
                if (_taskHandler == null)
                {
                    _taskHandler = new TaskHandler(AccessToken.UserId);
                }

                return _taskHandler;
            }
        }

        public TrackingHandler TrackingHandler
        {
            get
            {
                if (_trackingHandler == null)
                {
                    _trackingHandler = new TrackingHandler(AccessToken.UserId);
                }

                return _trackingHandler;
            }
        }
    }
}
