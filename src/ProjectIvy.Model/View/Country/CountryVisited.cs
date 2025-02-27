using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.Country;

public class CountryVisited : Country
{
    public CountryVisited(DatabaseModel.Common.Country x, DateTime visitedOn) : base(x)
    {
        VisitedOn = visitedOn;
    }

    public DateTime VisitedOn { get; set; }
}
