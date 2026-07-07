namespace PuxHomework.Models.Configuration;

public class FileComparerConfigurationModel
{
    /// <summary>
    /// Gets or sets the path to the snapshot file where the file comparison results will be stored.
    /// </summary>
    public string SnapshotFilePath { get; set; } = "data.json";
}
