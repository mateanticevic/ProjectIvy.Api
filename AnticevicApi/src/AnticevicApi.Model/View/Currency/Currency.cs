using DatabaseModel = AnticevicApi.Model.Database.Main;

namespace AnticevicApi.Model.View.Currency
{
    public class Currency
    {
        public Currency(DatabaseModel.Common.Currency x)
        {
            Code = x.Code;
            ValueId = x.Code;
            Name = x.Name;
            Symbol = x.Symbol;
        }

        public string ValueId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Symbol { get; set; }
    }
}
