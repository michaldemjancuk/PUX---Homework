using PuxHomework.Enums.FileData;

namespace PuxHomework.Models.DTOs.Home.Index;

public class HomeIndexResponseModel
{
    public int ItemsCount => Items.Count;
    public List<HomeIndexResponseModelItem> Items { get; set; } = new List<HomeIndexResponseModelItem>();

    public class HomeIndexResponseModelItem
    {
        public required string FilePath { get; set; } = string.Empty;
        public required string Md5Hash { get; set; } = string.Empty;
        public required FileStatusEnum FileStatus { get; set; } = FileStatusEnum.Unknown;
        public required int Version { get; set; } = -1;
    }
}
