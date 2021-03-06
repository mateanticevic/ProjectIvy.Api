﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Finance
{
    [Table(nameof(PaymentProvider), Schema = nameof(Finance))]
    public class PaymentProvider : IHasName, IHasValueId
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public string Name { get; set; }
    }
}
