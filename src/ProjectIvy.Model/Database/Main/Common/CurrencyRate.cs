using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Common
{
    [Table(nameof(CurrencyRate), Schema = nameof(Common))]
    public class CurrencyRate
    {
        public int FromCurrencyId { get; set; }

        public int ToCurrencyId { get; set; }

        public DateTime Timestamp { get; set; }

        public decimal Rate { get; set; }
    }
}
