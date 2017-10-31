using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace ProjectIvy.Model.Database.Main.Finance
{
    [Table(nameof(Card), Schema = nameof(Finance))]
    public class Card : UserEntity, IHasValueId, IHasName
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public string Name { get; set; }

        public DateTime Expires { get; set; }

        public DateTime Issued { get; set; }

        public string LastFourDigits { get; set; }

        public bool IsActive { get; set; }
    }
}
