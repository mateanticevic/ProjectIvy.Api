using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.Currency
{
    public class Currency
    {
        public Currency(DatabaseModel.Common.Currency x)
        {
            Code = x.Code;
            Id = x.Code;
            Name = x.Name;
            Symbol = x.Symbol;
        }

        public string Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Symbol { get; set; }
    }
}
