using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;

namespace Movies.Api.Sdk.Consumer;

public class AuthTokenProvider
{
    private static readonly SemaphoreSlim Lock = new(1, 1);
    private readonly HttpClient _httpClient;

    private string _cachedToken = string.Empty;

    public AuthTokenProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetTokenAsync(CancellationToken token = default)
    {
        if (!string.IsNullOrEmpty(_cachedToken))
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(_cachedToken);
            var expiryTimeText = jwt.Claims.Single(claim => claim.Type == "exp").Value;
            var expiryTime = UnixTimeStampToDateTime(int.Parse(expiryTimeText));
            if (expiryTime > DateTime.UtcNow) return _cachedToken;
        }

        await Lock.WaitAsync(token);

        var response = await _httpClient.PostAsJsonAsync("https://localhost:5003/token", new
        {
            userid = "d8566de3-b1a6-4a9b-b842-8e3887a82e43",
            email = "nick@nickchapsas.com",
            customClaims = new Dictionary<string, object>
            {
                { "admin", true },
                { "trusted_member", true }
            }
        }, token);

        var newToken = await response.Content.ReadAsStringAsync(token);
        _cachedToken = newToken;
        Lock.Release();
        return newToken;
    }

    private static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dateTime;
    }
}