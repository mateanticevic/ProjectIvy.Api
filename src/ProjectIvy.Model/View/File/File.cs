using ProjectIvy.Extensions.BuiltInTypes;
using System;

namespace ProjectIvy.Model.View.File
{
    public class File
    {
        public File(Database.Main.Storage.File entity)
        {
            Created = entity.Created;
            Id = entity.ValueId;
            Type = entity.FileType.ConvertTo(x => new FileType.FileType(x));
        }

        public string Id { get; set; }

        public DateTime Created { get; set; }

        public FileType.FileType Type { get; set; }
    }
}
