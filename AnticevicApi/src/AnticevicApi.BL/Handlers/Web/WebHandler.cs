using AnticevicApi.DL.Sql;
using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.Binding.Web;
using AnticevicApi.Model.View.Web;
using Dapper;
using System.Collections.Generic;

namespace AnticevicApi.BL.Handlers.Web
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

                var command = new CommandDefinition(SqlLoader.Load(MainSnippets.GetWebTimeSum), parameters);

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
                    WebValueId = binding.Webid,
                    IsSecured = binding.IsSecured,
                    From = binding.From,
                    To = binding.To,
                    UserId = User.Id
                };
                return db.ExecuteScalar<int>(SqlLoader.Load(MainSnippets.GetWebTimeTotal), parameters);
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
                    WebValueId = binding.Webid,
                    IsSecured = binding.IsSecured,
                    From = binding.From,
                    To = binding.To,
                    UserId = User.Id
                };

                var command = new CommandDefinition(SqlLoader.Load(MainSnippets.GetWebTimeTotalByDay), parameters);

                return db.Query<TimeByDay>(command);
            }
        }
    }
}
