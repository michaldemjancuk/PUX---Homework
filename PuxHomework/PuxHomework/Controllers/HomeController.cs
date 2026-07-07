using Microsoft.AspNetCore.Mvc;
using PuxHomework.Models.DTOs.Home.Index;
using PuxHomework.Services;

namespace PuxHomework.Controllers;

[ApiController]
[Route("Home/")]
public class HomeController(IFileComparerService fileComparerService) : ControllerBase
{
    private readonly IFileComparerService _fileComparerService = fileComparerService;

    [HttpGet("Index")]
    [ProducesResponseType(typeof(HomeIndexResponseModel), 200)]
    [ProducesResponseType(500)]
    public HomeIndexResponseModel Index(string folderPath = "C:\\Temp\\")
    {
        HomeIndexResponseModel result = _fileComparerService.CompareFiles(folderPath);

        return result;
    }
}
