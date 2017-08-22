﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AnticevicApi.Model.Database.Main.Finance
{
    [Table(nameof(IncomeSource), Schema = nameof(Finance))]
    public class IncomeSource : UserEntity, IHasValueId
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public string Name { get; set; }

        public ICollection<Income> Incomes { get; set; }
    }
}
