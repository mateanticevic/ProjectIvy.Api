using Google.Apis.Dialogflow.v2.Data;
using Newtonsoft.Json.Linq;
using ProjectIvy.Model.Binding;
using System;

namespace ProjectIvy.Business.MapExtensions
{
    public static class GoogleCloudDialogflowV2WebhookRequestExtensions
    {
        public static FilteredBinding ToFilteredBinding(this GoogleCloudDialogflowV2WebhookRequest request)
        {
            var datePeriod = request.QueryResult.Parameters["date-period"];
            var dateTime = request.QueryResult.Parameters["date-time"];

            return new FilteredBinding()
            {
                From = dateTime is DateTime fromDate ? fromDate.Date : (DateTime)((JObject)datePeriod)["startDate"],
                To = dateTime is DateTime toDate ? toDate.Date : (DateTime)((JObject)datePeriod)["endDate"]
            };
        }
    }
}
