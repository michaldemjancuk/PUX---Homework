namespace PuxHomework.Models.Internal.FileData;

public class FileDataModel
{
    public List<FileDataModelItem> Items { get; set; } = new List<FileDataModelItem>();

    public class FileDataModelItem
    {
        public required string FilePath { get; set; } = string.Empty;
        public required string Md5Hash { get; set; } = string.Empty;
    }
}