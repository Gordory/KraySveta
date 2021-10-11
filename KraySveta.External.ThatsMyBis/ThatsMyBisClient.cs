using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using KraySveta.External.ThatsMyBis.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace KraySveta.External.ThatsMyBis
{
    public interface IThatsMyBisClient
    {
        ValueTask<Roster> GetRosterAsync();

        ValueTask SyncRolesAsync();

        ValueTask UpdateRaidGroupAsync(RaidGroup raidGroup, Character[] characters, bool isMainGroup = false);
    }

    public class ThatsMyBisClient : IThatsMyBisClient
    {
        private const string BaseUrl = "https://thatsmybis.com";
        private const string GuildInfix = "3439/krai-sveta";

        private readonly CookieContainer _cookieContainer;
        private readonly HttpClientHandler _httpClientHandler;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ThatsMyBisClient(IConfiguration configuration)
        {
            _configuration = configuration;
            _cookieContainer = new CookieContainer();
            _httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                CookieContainer = _cookieContainer
            };
            _httpClient = new HttpClient(_httpClientHandler);

            SetupAuthConfig();
        }

        public async ValueTask<Roster> GetRosterAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/{GuildInfix}/roster");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            await using var contentStream = await response.Content.ReadAsStreamAsync();
            using var streamReader = new StreamReader(contentStream);

            const string commonSign = "var";
            const string charactersSign = "var characters";
            const string guildSign = "var guild";
            const string raidGroupsSign = "var raidGroups";

            string? line;
            Roster roster = new();
            while ((line = await streamReader.ReadLineAsync()) != null)
            {
                var commonSignIndex = line.IndexOf(commonSign, 0, Math.Min(50, line.Length), StringComparison.OrdinalIgnoreCase);
                if (commonSignIndex < 0)
                {
                    continue;
                }

                roster.Characters ??= DeserializeFromLine<Character[]>(line, charactersSign);
                roster.Guild ??= DeserializeFromLine<Guild>(line, guildSign);
                roster.RaidGroups ??= DeserializeFromLine<RaidGroup[]>(line, raidGroupsSign);
            }

            if (roster.Characters == null ||
                roster.Guild == null ||
                roster.RaidGroups == null)
            {
                throw new FormatException("Roster deserialization error. Report error to developer");
            }

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
            var csrfToken = _configuration["ThatsMyBis:CSRF"];

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

        private static T? DeserializeFromLine<T>(string line, string sign) where T : class
        {
            var signIndex = line.IndexOf(sign, StringComparison.OrdinalIgnoreCase);
            if (signIndex < 0)
            {
                return null;
            }

            const string equalsSign = "=";
            var equalsSignIndex = line.IndexOf(equalsSign, signIndex + sign.Length, StringComparison.OrdinalIgnoreCase);

            if (equalsSignIndex < 0)
            {
                return null;
            }

            var json = line.Substring(equalsSignIndex + 1).Trim(';', ' ');
            return JsonConvert.DeserializeObject<T>(json);
        }

        private void SetupAuthConfig()
        {
            var xsrf = _configuration["ThatsMyBis:XSRF"];
            var session = _configuration["ThatsMyBis:Session"];

            _cookieContainer.Add(new Uri("https://thatsmybis.com"), new Cookie("XSRF-TOKEN", xsrf));
            _cookieContainer.Add(new Uri("https://thatsmybis.com"), new Cookie("thats_my_bis_session", session));
        }
    }
}