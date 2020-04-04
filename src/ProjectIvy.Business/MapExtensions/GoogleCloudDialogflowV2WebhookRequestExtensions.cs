using Google.Apis.Dialogflow.v2.Data;
using Newtonsoft.Json.Linq;
using ProjectIvy.Model.Binding;
using System;

namespace ProjectIvy.Business.MapExtensions
{
    public static class GoogleCloudDialogflowV2WebhookRequestExtensions
    {
        public static FilteredBinding ToFilteredBinding(this GoogleCloudDialogflowV2WebhookRequest request, bool includeTime = false)
        {
            var datePeriod = request.QueryResult.Parameters.ContainsKey("date-period") ? request.QueryResult.Parameters["date-period"] : null;
            var dateTime = request.QueryResult.Parameters.ContainsKey("date-time") ? request.QueryResult.Parameters["date-time"] : null;

            return new FilteredBinding()
            {
                From = dateTime is DateTime fromDate ? fromDate.Date : datePeriod is JObject datePeriodStart ? (DateTime)datePeriodStart["startDate"] : (DateTime?)null,
                To = dateTime is DateTime toDate ? (includeTime ? toDate.Date.AddDays(1) : toDate.Date) : datePeriod is JObject datePeriodEnd ? (DateTime)datePeriodEnd["endDate"] : (DateTime?)null
            };
        }
    }
}
