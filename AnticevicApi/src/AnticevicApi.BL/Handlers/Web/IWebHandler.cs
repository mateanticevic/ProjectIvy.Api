using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.Binding.Web;
using AnticevicApi.Model.View.Web;
using System.Collections.Generic;

namespace AnticevicApi.BL.Handlers.Web
{
    public interface IWebHandler
    {
        IEnumerable<WebTime> GetTimeSummed(WebTimeGetPagedBinding binding);

        int GetTimeSum(WebTimeGetBinding binding);

        IEnumerable<TimeByDay> GetTimeTotalByDay(WebTimeGetBinding binding);
    }
}
