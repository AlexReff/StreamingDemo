using StreamingDemo.Models;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Configuration;

namespace StreamingDemo.Data.RedditApi
{
    public class RedditTokenProvider
    {
        private HttpClient _httpClient;
        private SemaphoreSlim _semaphore;
        private DateTimeOffset _expires;

        private RedditAccessTokenResponse? _accessToken;
        private string? _appId;
        private string? _appSecret;

        private readonly ILogger<RedditApiClient> _logger;

        public RedditTokenProvider(IConfiguration config, ILogger<RedditApiClient> logger)
        {
            _logger = logger;

            _semaphore = new SemaphoreSlim(1);

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(config["Reddit:UserAgent"]);
        }

        public void SetCredentials(string appId, string appSecret)
        {
            _appId = appId;
            _appSecret = appSecret;
        }

        public async Task<RedditAccessTokenResponse> GetTokenAsync(bool forceRefresh = false)
        {
            if (string.IsNullOrEmpty(_appId) || string.IsNullOrEmpty(_appSecret))
            {
                throw new ApplicationException("Missing AppID and/or AppSecret. Call SetCredentials(appId, appSecret) first.");
            }

            if (forceRefresh
                || _accessToken == null
                || string.IsNullOrWhiteSpace(_accessToken.access_token)
                || DateTime.Now >= _expires)
            {
                await _semaphore.WaitAsync();

                if (forceRefresh
                    || _accessToken == null
                    || string.IsNullOrWhiteSpace(_accessToken.access_token)
                    || DateTime.Now >= _expires)
                {
                    var result = await RetrieveAccessToken();

                    if (result != null
                        && !string.IsNullOrEmpty(result.access_token))
                    {
                        _accessToken = result;
                        _expires = DateTimeOffset.FromUnixTimeSeconds(result.expires_in);
                    }

                    if (_accessToken == null || string.IsNullOrWhiteSpace(_accessToken.access_token))
                    {
                        _logger.LogError("RetrieveAccessToken - Unable to retrieve Access Token");
                        throw new ApplicationException("Unable to retrieve Access Token");
                    }
                }

                _semaphore.Release();
            }

            return _accessToken;
        }

        private async Task<RedditAccessTokenResponse> RetrieveAccessToken()
        {
            var postData = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" }
            };

            HttpResponseMessage response;

            try
            {
                var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_appId}:{_appSecret}"));
                var currentAuth = _httpClient.DefaultRequestHeaders.Authorization;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                FormUrlEncodedContent content = new FormUrlEncodedContent(postData);

                response = await _httpClient.PostAsync("https://www.reddit.com/api/v1/access_token", content);

                _httpClient.DefaultRequestHeaders.Authorization = currentAuth;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RetrieveAccessToken Unable to retrieve access token");
                throw;
            }

            RedditAccessTokenResponse? data = null;

            if (response != null)
            {
                try
                {
                    data = await response.Content.ReadFromJsonAsync<RedditAccessTokenResponse>();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "RetrieveAccessToken Unexpected response from server");
                    throw;
                }
            }

            return data!;
        }
    }
}
