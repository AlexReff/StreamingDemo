using System.Net.Http.Headers;
using System.Text;
using StreamingDemo.Data.RedditApi.Interfaces;
using StreamingDemo.Data.RedditApi.Models;

namespace StreamingDemo.Data.RedditApi
{
    public class RedditTokenProvider : IRedditTokenProvider
    {
        private HttpClient _httpClient;
        private SemaphoreSlim _semaphore;
        private AccessToken? _accessToken;

        private readonly ILogger<IRedditTokenProvider> _logger;
        private readonly string _credentials;

        private static readonly FormUrlEncodedContent _postData = new FormUrlEncodedContent(new Dictionary<string, string>()
        {
            { "grant_type", "client_credentials" }
        });

        public RedditTokenProvider(IRedditApiConfig config, ILogger<IRedditTokenProvider> logger, HttpClient httpClient)
        {
            if (config == null
                || string.IsNullOrEmpty(config.AppId)
                || string.IsNullOrEmpty(config.AppSecret))
            {
                throw new ArgumentNullException(nameof(config));
            }

            _logger = logger;
            _httpClient = httpClient;

            _credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{config.AppId}:{config.AppSecret}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

            _semaphore = new SemaphoreSlim(1);
        }

        public async Task<IAccessToken> GetTokenAsync(bool forceRefresh = false)
        {
            if (forceRefresh || TokenNeedsRefreshed())
            {
                await _semaphore.WaitAsync();

                if (forceRefresh || TokenNeedsRefreshed())
                {
                    var result = await RetrieveAccessToken();

                    if (result == null || string.IsNullOrEmpty(result.access_token))
                    {
                        _semaphore.Release();
                        _logger.LogError("RetrieveAccessToken - Unable to retrieve Access Token");
                        throw new ApplicationException("Unable to retrieve Access Token");
                    }

                    var expires = DateTimeOffset.FromUnixTimeSeconds(result.expires_in);
                    _accessToken = new AccessToken(result.access_token, expires);
                }

                _semaphore.Release();
            }

            return _accessToken!;
        }

        private bool TokenNeedsRefreshed()
        {
            return _accessToken == null
                || string.IsNullOrWhiteSpace(_accessToken.Token)
                || DateTime.Now >= _accessToken.Expiration;
        }

        private async Task<RedditAccessTokenResponse?> RetrieveAccessToken()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync("https://www.reddit.com/api/v1/access_token", _postData);
                
                RedditAccessTokenResponse? data = await response.Content.ReadFromJsonAsync<RedditAccessTokenResponse>();

                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RetrieveAccessToken Unable to retrieve access token");
                throw;
            }
        }
    }
}
