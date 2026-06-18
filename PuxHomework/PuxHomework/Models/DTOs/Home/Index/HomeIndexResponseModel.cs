namespace PuxHomework.Models.DTOs.Home.Index;

public class HomeIndexResponseModel
{
    public List<HomeIndexResponseModelItem> Items { get; set; } = new List<HomeIndexResponseModelItem>();
    public int ItemsCount => Items.Count;

    public class HomeIndexResponseModelItem
    {
        public required string FilePath { get; set; } = string.Empty;
        public required string Md5Hash { get; set; } = string.Empty;
        public required int Version { get; set; } = -1;
    }
}
