namespace PuxHomework.Models.Internal.FileData;

public class StoredFileDataModel
{
    public List<FileDataModelItem> Items { get; set; } = new List<FileDataModelItem>();

    public class FileDataModelItem
    {
        public required string FilePath { get; set; } = string.Empty;
        public string Md5Hash { get; set; } = string.Empty;
        public required int Version { get; set; } = -1;
        public required bool IsFile { get; set; }
    }
}
