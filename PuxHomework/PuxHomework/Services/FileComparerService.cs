using Microsoft.Extensions.Options;
using PuxHomework.Enums.FileData;
using PuxHomework.Models.Configuration;
using PuxHomework.Models.DTOs.Home.Index;
using PuxHomework.Models.Internal.FileData;

namespace PuxHomework.Services;

public interface IFileComparerService
{
    HomeIndexResponseModel CompareFiles(string folderPath);
}

public class FileComparerService(IOptions<FileComparerConfigurationModel> configuration, IMd5CalculatorService md5CalculatorService) : IFileComparerService
{
    private readonly IOptions<FileComparerConfigurationModel> _configurationOptions = configuration;
    private readonly IMd5CalculatorService _md5CalculatorService = md5CalculatorService;

    public HomeIndexResponseModel CompareFiles(string folderPath)
    {
        var currentConfig = _configurationOptions.Value;
        var filePath = currentConfig.SnapshotFilePath;
        StoredFileDataModel dataSnapshot = GetStoredFileSnapshot(currentConfig);

        string[] existingFiles = GetAllFiles(folderPath);
        string[] existingFolders = GetAllFolders(folderPath);

        StoredFileDataModel currentData = LoadCurrentData(existingFiles, existingFolders);
        HomeIndexResponseModel response = new();

        // New / Updated / Unchanged
        foreach (var item in currentData.Items)
        {
            var dataSnapshotItem = dataSnapshot.Items.FirstOrDefault(snapshotItem => snapshotItem.FilePath == item.FilePath);

            FileStatusEnum fileStatus = dataSnapshotItem != null
                ? (dataSnapshotItem.Md5Hash != item.Md5Hash ? FileStatusEnum.Updated : FileStatusEnum.Unchanged)
                : FileStatusEnum.New;

            item.Version = fileStatus == FileStatusEnum.Updated
                ? dataSnapshotItem.Version + 1 
                : dataSnapshotItem?.Version ?? 1;

            response.Items.Add(new HomeIndexResponseModel.HomeIndexResponseModelItem
            {
                Path = item.FilePath,
                Version = item.Version,
                FileStatus = fileStatus
            });
        }

        // Deleted
        var currentPaths = currentData.Items.Select(item => item.FilePath).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var deletedItems = dataSnapshot.Items.Where(snapshotItem => !currentPaths.Contains(snapshotItem.FilePath)).ToList();

        foreach (var deletedItem in deletedItems)
        {
            response.Items.Add(new HomeIndexResponseModel.HomeIndexResponseModelItem
            {
                Path = deletedItem.FilePath,
                Version = deletedItem.Version,
                FileStatus = FileStatusEnum.Deleted
            });
        }

        SaveFileSnapshot(currentData, currentConfig);

        return response;
    }

    private StoredFileDataModel LoadCurrentData(string[] existingFiles, string[] existingFolders)
    {
        StoredFileDataModel currentData = new StoredFileDataModel();

        foreach (var file in existingFiles)
        {
            currentData.Items.Add(new StoredFileDataModel.FileDataModelItem
            {
                FilePath = file,
                Md5Hash = _md5CalculatorService.CalculateMD5(file),
                Version = 1, // Versioning starts at 1 for new files
                IsFile = true
            });
        }
        foreach (var folder in existingFolders)
        {
            currentData.Items.Add(new StoredFileDataModel.FileDataModelItem
            {
                FilePath = folder,
                Version = 1, // Versioning starts at 1 for new folders
                IsFile = false
            });
        }

        return currentData;
    }

    private string[] GetAllFiles(string folderPath)
    {
        if (string.IsNullOrWhiteSpace(folderPath))
            throw new ArgumentException("Folder path cannot be null or whitespace.", nameof(folderPath));

        if (!Directory.Exists(folderPath))
            throw new DirectoryNotFoundException($"The specified folder path does not exist: {folderPath}");

        return Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);
    }

    private string[] GetAllFolders(string folderPath)
    { 
        if (string.IsNullOrWhiteSpace(folderPath))
            throw new ArgumentException("Folder path cannot be null or whitespace.", nameof(folderPath));

        if (!Directory.Exists(folderPath))
            throw new DirectoryNotFoundException($"The specified folder path does not exist: {folderPath}");

        return Directory.GetDirectories(folderPath, "*", SearchOption.AllDirectories);
    }

    private StoredFileDataModel GetStoredFileSnapshot(FileComparerConfigurationModel config)
    {
        string filePath = config.SnapshotFilePath;

        // Check if the file exists, if not return an empty model
        if (filePath == null)
            throw new ArgumentNullException(nameof(filePath));

        if (!File.Exists(filePath))
            return new StoredFileDataModel();

        // Read the file content and deserialize it into the model
        var fileContent = File.ReadAllText(filePath);
        var storedFileData = System.Text.Json.JsonSerializer.Deserialize<StoredFileDataModel>(fileContent);

        return storedFileData ?? new StoredFileDataModel();
    }

    private void SaveFileSnapshot(StoredFileDataModel snapshot, FileComparerConfigurationModel config)
    {
        var filePath = config.SnapshotFilePath;

        if (filePath == null)
            throw new ArgumentNullException(nameof(filePath));

        var serializedData = System.Text.Json.JsonSerializer.Serialize(
            snapshot, 
            new System.Text.Json.JsonSerializerOptions { WriteIndented = true }
        );

        File.WriteAllText(filePath, serializedData);
    }
}
