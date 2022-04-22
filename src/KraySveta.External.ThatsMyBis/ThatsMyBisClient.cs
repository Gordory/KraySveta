using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using KraySveta.External.ThatsMyBis.Models;
using KraySveta.External.ThatsMyBis.Parsers;
using Microsoft.Extensions.Options;

namespace KraySveta.External.ThatsMyBis;

public interface IThatsMyBisClient
{
    ValueTask<Raid> GetRaidAsync(int id);

    ValueTask<Roster> GetRosterAsync();

    ValueTask SyncRolesAsync();

    ValueTask UpdateRaidGroupAsync(RaidGroup raidGroup, Character[] characters, bool isMainGroup = false);
}

public class ThatsMyBisClient : IThatsMyBisClient
{
    private const string BaseUrl = "https://thatsmybis.com";
    private const string GuildInfix = "3439/krai-sveta";

    private readonly CookieContainer _cookieContainer;
    private readonly HttpClient _httpClient;
    private readonly IOptions<ThatsMyBisConfiguration> _configuration;
    private readonly IRosterParser _rosterParser;
    private readonly IRaidParser _raidParser;

    public ThatsMyBisClient(IOptions<ThatsMyBisConfiguration> configuration, IRosterParser rosterParser, IRaidParser raidParser)
    {
        _configuration = configuration;
        _rosterParser = rosterParser;
        _raidParser = raidParser;

        _cookieContainer = new CookieContainer();
        var httpClientHandler = new HttpClientHandler
        {
            AllowAutoRedirect = false,
            CookieContainer = _cookieContainer
        };
        _httpClient = new HttpClient(httpClientHandler);

        SetupAuthConfig();
    }

    public async ValueTask<Raid> GetRaidAsync(int id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/{GuildInfix}/raids/{id}");
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        await using var contentStream = await response.Content.ReadAsStreamAsync();
        using var streamReader = new StreamReader(contentStream);

        var raid = await _raidParser.ParseAsync(streamReader);
        return raid;
    }

    public async ValueTask<Roster> GetRosterAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/{GuildInfix}/roster");
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        await using var contentStream = await response.Content.ReadAsStreamAsync();
        using var streamReader = new StreamReader(contentStream);

        var roster = await _rosterParser.ParseAsync(streamReader);
        return roster;
    }

    public async ValueTask SyncRolesAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/{GuildInfix}/syncRoles");
        var response = await _httpClient.SendAsync(request);

        if (response.StatusCode != HttpStatusCode.Found)
            response.EnsureSuccessStatusCode();
    }

    public async ValueTask UpdateRaidGroupAsync(RaidGroup raidGroup, Character[] characters, bool isMainGroup = false)
    {
        var updateMethod = isMainGroup
            ? "update-characters"
            : "update-other-characters";

        var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/{GuildInfix}/raid-groups/{updateMethod}");
        var csrfToken = _configuration.Value.CSRF;

        var form = CreateFormWithCharacterIds(characters);
        form.Add(new StringContent(csrfToken), "_token");
        form.Add(new StringContent(raidGroup.Id.ToString()), "raid_group_id");

        request.Content = form;

        var response = await _httpClient.SendAsync(request);
        if (response.StatusCode != HttpStatusCode.Found)
            response.EnsureSuccessStatusCode();
    }

    private static MultipartFormDataContent CreateFormWithCharacterIds(Character[] characters)
    {
        var form = new MultipartFormDataContent();

        foreach (var character in characters)
        {
            form.Add(new StringContent(character.Id.ToString()), "characters[][character_id]");
        }

        return form;
    }

    private void SetupAuthConfig()
    {
        var xsrf = _configuration.Value.XSRF;
        var session = _configuration.Value.Session;

        _cookieContainer.Add(new Uri("https://thatsmybis.com"), new Cookie("XSRF-TOKEN", xsrf));
        _cookieContainer.Add(new Uri("https://thatsmybis.com"), new Cookie("thats_my_bis_session", session));
    }
}