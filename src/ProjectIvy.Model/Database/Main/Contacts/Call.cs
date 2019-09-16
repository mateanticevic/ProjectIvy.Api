using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Contacts
{
    [Table(nameof(Contacts), Schema = "User")]
    public class Call : UserEntity, IHasTimestamp, IHasValueId
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public DateTime Timestamp { get; set; }

        public string Number { get; set; }

        public int Duration { get; set; }

        public int FileId { get; set; }

        public Storage.File File { get; set; }
    }
}
