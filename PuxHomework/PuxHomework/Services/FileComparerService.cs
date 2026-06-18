using Microsoft.Extensions.Options;
using PuxHomework.Models.Configuration;
using PuxHomework.Models.DTOs.Home.Index;

namespace PuxHomework.Services;

public interface IFileComparerService
{
    HomeIndexResponseModel CompareFiles(string folderPath);
}

public class FileComparerService : IFileComparerService
{
    private readonly FileComparerConfigurationModel _configuration;

    public FileComparerService(IOptions<FileComparerConfigurationModel> configuration)
    {
        _configuration = configuration.Value;
    }

    public HomeIndexResponseModel CompareFiles(string folderPath)
    {
        // Implementation will go here

        throw new NotImplementedException("Yet to be implemented");
    }
}
