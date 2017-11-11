using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace ProjectIvy.Model.Database.Main.Storage
{
    [Table(nameof(File), Schema = nameof(Storage))]
    public class File : IHasValueId
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public int ProviderId { get; set; }

        public string Uri { get; set; }

        public int FileTypeId { get; set; }

        public DateTime Created { get; set; }

        public FileType FileType { get; set; }

        public ICollection<Finance.ExpenseFile> ExpenseFiles { get; set; }
    }
}
