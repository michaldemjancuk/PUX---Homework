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
    [ProducesResponseType(typeof(IEnumerable<HomeIndexResponseModel>), 200)]
    [ProducesResponseType(500)]
    public HomeIndexResponseModel Get()
    {
        throw new NotImplementedException("Yet to be implemented");
    }
}
