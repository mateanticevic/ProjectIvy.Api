using ProjectIvy.DL.Extensions;
using ProjectIvy.Model.Binding.Beer;
using ProjectIvy.Model.View;
using ProjectIvy.DL.Extensions.Entities;
using System.Collections.Generic;
using System.Linq;
using View = ProjectIvy.Model.View.Beer;

namespace ProjectIvy.BL.Handlers.Beer
{
    public class BeerHandler : Handler<BeerHandler>, IBeerHandler
    {
        public BeerHandler(IHandlerContext<BeerHandler> context) : base(context)
        {
        }

        public PagedView<View.Beer> GetBeers(BeerGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Beers.OrderBy(binding)
                                    .Select(x => new View.Beer(x))
                                    .ToPagedView(binding);
            }
        }

        public IEnumerable<View.BeerServing> GetServings()
        {
            using (var context = GetMainContext())
            {
                return context.BeerServings.Select(x => new View.BeerServing(x))
                                           .ToList();
            }
        }
    }
}
