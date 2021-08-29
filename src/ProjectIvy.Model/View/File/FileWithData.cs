using System.IO;

namespace ProjectIvy.Model.View.File
{
    public class FileWithData : File
    {
        public FileWithData(Database.Main.Storage.File entity) : base(entity)
        {
        }

        public byte[] Data { get; set; }

        public Stream Stream { get; set; }
    }
}
