namespace ProjectIvy.Model.View.FileType;

public class FileType
{
    public FileType(Database.Main.Storage.FileType entity)
    {
        Id = entity.ValueId;
        Name = entity.Name;
        MimeType = entity.MimeType;
    }

    public string Id { get; set; }

    public string Name { get; set; }

    public string MimeType { get; set; }
}
