using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KraySveta.App.Controllers;

[Route("api/attendance/warnings")]
[ApiController]
[Authorize]
public class AttendanceWarningsController : Controller
{
    [HttpGet]
    public object GetAll()
    {
        return new
        {
            Description = "Hello, World",
        };
    }
}