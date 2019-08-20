using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Org
{
    [Table(nameof(ToDo), Schema = nameof(Org))]
    public class ToDo : UserEntity, IHasName, IHasValueId
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public string Name { get; set; }

        public bool IsDone { get; set; }

        public DateTime Created { get; set; }
    }
}
