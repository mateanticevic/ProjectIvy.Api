using AnticevicApi.DL.Extensions;
using System.Collections.Generic;
using System.Linq;
using View = AnticevicApi.Model.View.Poi;

namespace AnticevicApi.BL.Handlers.Poi
{
    public class PoiHandler : Handler<PoiHandler>, IPoiHandler
    {
        public PoiHandler(IHandlerContext<PoiHandler> context) : base(context)
        {
        }
    }
}
