using Microsoft.AspNetCore.Mvc;
using PuxHomework.Models.DTOs.Home.Index;
using PuxHomework.Services;

namespace PuxHomework.Controllers;

[ApiController]
[Route("[controller]")]
public class HomeController(IFileComparerService fileComparerService) : ControllerBase
{
    private readonly IFileComparerService _fileComparerService = fileComparerService;

    [HttpGet(Name = "Index")]
    [ProducesResponseType(typeof(HomeIndexResponseModel), 200)]
    [ProducesResponseType(500)]
    public HomeIndexResponseModel Get(string folderPath = "C:\\Temp\\")
    {
        HomeIndexResponseModel result = _fileComparerService.CompareFiles(folderPath);

        return new HomeIndexResponseModel();
    }
}
