namespace PuxHomework.Models.Internal.FileData;

public class FileDataModel
{
    public required string FilePath { get; set; } = string.Empty;
    public required string Md5Hash { get; set; } = string.Empty;
    public required int Version { get; set; } = -1;
}