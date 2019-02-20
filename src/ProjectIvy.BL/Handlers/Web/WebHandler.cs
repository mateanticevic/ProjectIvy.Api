using Dapper;
using ProjectIvy.Data.Sql;
using ProjectIvy.Model.Binding.Web;
using ProjectIvy.Model.View.Web;
using ProjectIvy.Data.Sql.Main.Models;
using ProjectIvy.Data.Sql.Main.Scripts;
using ProjectIvy.Model.View;
using System.Collections.Generic;
using System.Linq;

namespace ProjectIvy.BL.Handlers.Web
{
    public class WebHandler : Handler<WebHandler>, IWebHandler
    {
        public WebHandler(IHandlerContext<WebHandler> context) : base(context)
        {
        }

        public IEnumerable<WebTime> GetTimeSummed(WebTimeGetPagedBinding binding)
        {
            using (var db = GetSqlConnection())
            {
                var parameters = new
                {
                    DeviceValueId = binding.DeviceId,
                    From = binding.From,
                    Page = binding.Page,
                    PageSize = binding.PageSize,
                    To = binding.To,
                    UserId = User.Id
                };

                var command = new CommandDefinition(SqlLoader.Load(Constants.GetWebTimeSum), parameters);

                return db.Query<WebTime>(command);
            }
        }

        public int GetTimeSum(WebTimeGetBinding binding)
        {
            using (var db = GetSqlConnection())
            {
                var parameters = new
                {
                    DeviceValueId = binding.DeviceId,
                    DomainValueId = binding.DomainId,
                    WebValueId = binding.WebId,
                    IsSecured = binding.IsSecured,
                    From = binding.From,
                    To = binding.To,
                    UserId = User.Id
                };
                return db.ExecuteScalar<int>(SqlLoader.Load(Constants.GetWebTimeTotal), parameters);
            }
        }

        public IEnumerable<TimeByDay> GetTimeTotalByDay(WebTimeGetBinding binding)
        {
            using (var db = GetSqlConnection())
            {
                var parameters = new
                {
                    DeviceValueId = binding.DeviceId,
                    DomainValueId = binding.DomainId,
                    WebValueId = binding.WebId,
                    IsSecured = binding.IsSecured,
                    From = binding.From,
                    To = binding.To,
                    UserId = User.Id
                };

                var command = new CommandDefinition(SqlLoader.Load(Constants.GetWebTimeTotalByDay), parameters);

                return db.Query<TimeByDay>(command);
            }
        }

        public IEnumerable<GroupedByMonth<int>> GetTimeTotalByMonth(WebTimeGetBinding binding)
        {
            using (var db = GetSqlConnection())
            {
                var parameters = new
                {
                    DeviceValueId = binding.DeviceId,
                    DomainValueId = binding.DomainId,
                    WebValueId = binding.WebId,
                    IsSecured = binding.IsSecured,
                    From = binding.From,
                    To = binding.To,
                    UserId = User.Id
                };

                var command = new CommandDefinition(SqlLoader.Load(Constants.GetWebTimeTotalByMonth), parameters);

                return db.Query<GetWebTimeTotalByMonth>(command)
                         .Select(x => new GroupedByMonth<int>(x.Seconds, x.Year, x.Month));
            }
        }

        public IEnumerable<GroupedByYear<int>> GetTimeTotalByYear(WebTimeGetBinding binding)
        {
            using (var db = GetSqlConnection())
            {
                var parameters = new
                {
                    DeviceValueId = binding.DeviceId,
                    DomainValueId = binding.DomainId,
                    WebValueId = binding.WebId,
                    IsSecured = binding.IsSecured,
                    From = binding.From,
                    To = binding.To,
                    UserId = User.Id
                };

                var command = new CommandDefinition(SqlLoader.Load(Constants.GetWebTimeTotalByYear), parameters);

                return db.Query<GetWebTimeTotalByYear>(command)
                         .Select(x => new GroupedByYear<int>(x.Seconds, x.Year));
            }
        }
    }
}
