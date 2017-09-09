using ProjectIvy.Model.Database.Main.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace ProjectIvy.Model.Database.Main.Finance
{
    [Table(nameof(Income), Schema = nameof(Finance))]
    public class Income : UserEntity, IHasTimestamp
    {
        [Key]
        public int Id { get; set; }

        public decimal Ammount { get; set; }

        public int CurrencyId { get; set; }

        public Currency Currency { get; set; }

        public string Description { get; set; }

        public IncomeSource IncomeSource { get; set; }

        public IncomeType IncomeType { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
