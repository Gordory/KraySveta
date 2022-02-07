using System.Threading.Tasks;
using KraySveta.External.ThatsMyBis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace KraySveta.Api.Controllers;

[ApiController]
[Route("discord/auth")]
public class DiscordAuthController : ControllerBase
{
    private readonly ILogger<DiscordAuthController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IThatsMyBisClient _thatsMyBisClient;

    public DiscordAuthController(
        ILogger<DiscordAuthController> logger,
        IConfiguration configuration,
        IThatsMyBisClient thatsMyBisClient)
    {
        _logger = logger;
        _configuration = configuration;
        _thatsMyBisClient = thatsMyBisClient;
    }

    [HttpGet]
    public RedirectResult Get()
    {
        var discordOAuth2Url = _configuration["Discord:OAuth2Url"];
        return new RedirectResult(discordOAuth2Url);
    }

    [HttpGet]
    [Route("callback")]
    public async Task<ActionResult> DiscordAuthCallbackAsync(string code)
    {
        return Ok("Hello there");
    }
}