using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Storage;

[Table(nameof(File), Schema = nameof(Storage))]
public class File : UserEntity, IHasValueId
{
    [Key]
    public int Id { get; set; }

    public string ValueId { get; set; }

    public int ProviderId { get; set; }

    public string Uri { get; set; }

    public int FileTypeId { get; set; }

    public int SizeInBytes { get; set; }

    public DateTime Created { get; set; }

    public FileType FileType { get; set; }

    public ICollection<Finance.ExpenseFile> ExpenseFiles { get; set; }

    public ICollection<Travel.Trip> Trips { get; set; }
}
