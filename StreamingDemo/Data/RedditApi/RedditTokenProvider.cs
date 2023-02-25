using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Configuration;
using StreamingDemo.Data.RedditApi.Interfaces;
using StreamingDemo.Data.RedditApi.Models;
using Microsoft.Extensions.Options;

namespace StreamingDemo.Data.RedditApi
{
    public class RedditTokenProvider : IRedditTokenProvider
    {
        private HttpClient _httpClient;
        private SemaphoreSlim _semaphore;
        private DateTimeOffset _expires;

        private RedditAccessTokenResponse? _accessToken;

        private readonly ILogger<RedditTokenProvider> _logger;
        private readonly IRedditApiConfig _config;

        public RedditTokenProvider(IRedditApiConfig config, ILogger<RedditTokenProvider> logger, HttpClient httpClient)
        {
            _config = config;
            _logger = logger;
            //_httpClient = new HttpClient();
            //_httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(_config.UserAgent);
            _httpClient = httpClient;

            _semaphore = new SemaphoreSlim(1);
        }

        public async Task<IAccessToken> GetTokenAsync(bool forceRefresh = false)
        {
            if (string.IsNullOrEmpty(_config.AppId) || string.IsNullOrEmpty(_config.AppSecret))
            {
                throw new ApplicationException("Missing AppID and/or AppSecret");
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

            return new AccessToken(_accessToken.access_token);
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
                var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_config.AppId}:{_config.AppSecret}"));
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
