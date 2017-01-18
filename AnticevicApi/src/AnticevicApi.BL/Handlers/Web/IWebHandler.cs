using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.View.Web;
using System.Collections.Generic;

namespace AnticevicApi.BL.Handlers.Web
{
    public interface IWebHandler
    {
        IEnumerable<WebTime> GetTimeSummed(FilteredPagedBinding binding, string deviceId);

        int GetTimeTotal(FilteredBinding binding, string deviceValueId);
    }
}
