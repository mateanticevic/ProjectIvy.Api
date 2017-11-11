using ProjectIvy.Extensions.BuiltInTypes;

namespace ProjectIvy.Model.View.File
{
    public class File
    {
        public File(Database.Main.Storage.File entity)
        {
            Id = entity.ValueId;
            FileType = entity.FileType.ConvertTo(x => new FileType.FileType(x));
        }

        public string Id { get; set; }

        public FileType.FileType FileType { get; set; }
    }
}
