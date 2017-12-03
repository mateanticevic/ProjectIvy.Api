using ProjectIvy.Model.Database.Main.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace ProjectIvy.Model.Database.Main.Finance
{
    [Table(nameof(Income), Schema = nameof(Finance))]
    public class Income : UserEntity, IHasCreatedModified
    {
        [Key]
        public int Id { get; set; }

        public decimal Ammount { get; set; }

        public int CurrencyId { get; set; }

        public int IncomeTypeId { get; set; }

        public int IncomeSourceId { get; set; }

        public Currency Currency { get; set; }

        public string Description { get; set; }

        public IncomeSource IncomeSource { get; set; }

        public IncomeType IncomeType { get; set; }

        public DateTime Date { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }
    }
}
