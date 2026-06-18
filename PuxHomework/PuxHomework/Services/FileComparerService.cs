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

public class FileComparerService(
    IOptions<FileComparerConfigurationModel> configuration,
    IMd5CalculatorService md5CalculatorService,
    ILogger<FileComparerService> logger) : IFileComparerService
{
    private readonly IOptions<FileComparerConfigurationModel> _configurationOptions = configuration;
    private readonly IMd5CalculatorService _md5CalculatorService = md5CalculatorService;
    private readonly ILogger<FileComparerService> _logger = logger;

    public HomeIndexResponseModel CompareFiles(string folderPath)
    {
        _logger.LogInformation("Starting file comparison for folder path '{FolderPath}'", folderPath);

        var currentConfig = _configurationOptions.Value;
        StoredFileDataModel dataSnapshot = GetStoredFileSnapshot(currentConfig);

        string[] existingFiles = GetAllFiles(folderPath);
        string[] existingFolders = GetAllFolders(folderPath);

        _logger.LogInformation(
            "Loaded {FileCount} files, {FolderCount} folders and {SnapshotCount} snapshot items",
            existingFiles.Length,
            existingFolders.Length,
            dataSnapshot.Items.Count);

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
                ? dataSnapshotItem!.Version + 1
                : dataSnapshotItem?.Version ?? 1;

            response.Items.Add(new HomeIndexResponseModel.HomeIndexResponseModelItem
            {
                Path = item.FilePath,
                Version = item.Version,
                FileStatus = fileStatus
            });

            _logger.LogInformation(
                "Processed path '{Path}' with status '{Status}' and version {Version}",
                item.FilePath,
                fileStatus,
                item.Version);
        }

        // Deleted
        var currentPaths = currentData.Items
            .Select(item => item.FilePath)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var deletedItems = dataSnapshot.Items
            .Where(snapshotItem => !currentPaths.Contains(snapshotItem.FilePath))
            .ToList();

        foreach (var deletedItem in deletedItems)
        {
            response.Items.Add(new HomeIndexResponseModel.HomeIndexResponseModelItem
            {
                Path = deletedItem.FilePath,
                Version = deletedItem.Version,
                FileStatus = FileStatusEnum.Deleted
            });

            _logger.LogInformation(
                "Detected deleted path '{Path}' with version {Version}",
                deletedItem.FilePath,
                deletedItem.Version);
        }

        SaveFileSnapshot(currentData, currentConfig);

        _logger.LogInformation(
            "Saved snapshot with {ItemCount} items. Comparison finished with {ResponseCount} response items",
            currentData.Items.Count,
            response.Items.Count);

        return response;
    }

    private StoredFileDataModel LoadCurrentData(string[] existingFiles, string[] existingFolders)
    {
        StoredFileDataModel currentData = new();

        foreach (var file in existingFiles)
        {
            currentData.Items.Add(new StoredFileDataModel.FileDataModelItem
            {
                FilePath = file,
                Md5Hash = _md5CalculatorService.CalculateMD5(file),
                Version = 1,
                IsFile = true
            });
        }

        foreach (var folder in existingFolders)
        {
            currentData.Items.Add(new StoredFileDataModel.FileDataModelItem
            {
                FilePath = folder,
                Version = 1,
                IsFile = false
            });
        }

        _logger.LogInformation(
            "Current data model built with {ItemCount} items",
            currentData.Items.Count);

        return currentData;
    }

    private string[] GetAllFiles(string folderPath)
    {
        if (string.IsNullOrWhiteSpace(folderPath))
        {
            _logger.LogWarning("File comparison failed because folder path was empty");
            throw new ArgumentException("Folder path cannot be null or whitespace.", nameof(folderPath));
        }

        if (!Directory.Exists(folderPath))
        {
            _logger.LogWarning("File comparison failed because directory '{FolderPath}' was not found", folderPath);
            throw new DirectoryNotFoundException($"The specified folder path does not exist: {folderPath}");
        }

        return Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);
    }

    private string[] GetAllFolders(string folderPath)
    {
        if (string.IsNullOrWhiteSpace(folderPath))
        {
            _logger.LogWarning("Folder enumeration failed because folder path was empty");
            throw new ArgumentException("Folder path cannot be null or whitespace.", nameof(folderPath));
        }

        if (!Directory.Exists(folderPath))
        {
            _logger.LogWarning("Folder enumeration failed because directory '{FolderPath}' was not found", folderPath);
            throw new DirectoryNotFoundException($"The specified folder path does not exist: {folderPath}");
        }

        return Directory.GetDirectories(folderPath, "*", SearchOption.AllDirectories);
    }

    private StoredFileDataModel GetStoredFileSnapshot(FileComparerConfigurationModel config)
    {
        string filePath = config.SnapshotFilePath;

        if (filePath == null)
            throw new ArgumentNullException(nameof(filePath));

        if (!File.Exists(filePath))
        {
            _logger.LogInformation("Snapshot file '{SnapshotFilePath}' does not exist. Empty snapshot will be used", filePath);
            return new StoredFileDataModel();
        }

        var fileContent = File.ReadAllText(filePath);
        var storedFileData = System.Text.Json.JsonSerializer.Deserialize<StoredFileDataModel>(fileContent);

        _logger.LogInformation("Snapshot file '{SnapshotFilePath}' loaded", filePath);

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

        _logger.LogInformation("Snapshot file '{SnapshotFilePath}' saved", filePath);
    }
}
