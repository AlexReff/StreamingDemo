using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using StreamingDemo.Data.RedditApi.Models;
using StreamingDemo.Data.RedditApi.Interfaces;
using System.Threading;

namespace StreamingDemo.Data.RedditApi
{
    public class RedditApiAuthenticationHandler : DelegatingHandler
    {
        private DateTimeOffset _expires;
        private IAccessToken? _accessToken;

        private readonly IRedditApiConfig _config;
        //private readonly IRedditTokenProvider _tokenProvider;

        private readonly HttpClient _httpClient;
        private readonly SemaphoreSlim _semaphore;

        public RedditApiAuthenticationHandler(IRedditApiConfig config/*, IRedditTokenProvider tokenProvider*/)
        {
            _config = config;
            //_tokenProvider = tokenProvider;
            _accessToken = null;

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(config.UserAgent);

            _semaphore = new SemaphoreSlim(1);
        }

        /*
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_accessToken == null)
            {
                _accessToken = await _tokenProvider.GetTokenAsync(true);
            }
            if (string.IsNullOrEmpty(_accessToken.Token))
            {
                _accessToken = await _tokenProvider.GetTokenAsync(true);
            }
            if (string.IsNullOrEmpty(_accessToken.Token))
            {
                throw new Exception("Unable to retrieve AccessToken");
            }

            if (!request.Headers.Contains("Authorization"))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken.Token);
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var token = await _tokenProvider.GetTokenAsync(true);

                if (string.IsNullOrWhiteSpace(token.Token))
                {
                    return response;
                }
                
                _accessToken = token;

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken.Token);
                response = await base.SendAsync(request, cancellationToken);
            }

            return response;
        }
        */

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_accessToken == null)
            {
                _accessToken = await GetTokenAsync(true);
            }
            if (string.IsNullOrEmpty(_accessToken.Token))
            {
                _accessToken = await GetTokenAsync(true);
            }
            if (string.IsNullOrEmpty(_accessToken.Token))
            {
                throw new Exception("Unable to retrieve AccessToken");
            }

            if (!request.Headers.Contains("Authorization"))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken.Token);
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var token = await GetTokenAsync(true);

                if (string.IsNullOrWhiteSpace(token.Token))
                {
                    return response;
                }

                _accessToken = token;

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken.Token);
                response = await base.SendAsync(request, cancellationToken);
            }

            return response;
        }

        protected async Task<IAccessToken> GetTokenAsync(bool forceRefresh = false)
        {
            if (string.IsNullOrEmpty(_config.AppId) || string.IsNullOrEmpty(_config.AppSecret))
            {
                throw new ApplicationException("Missing AppID and/or AppSecret");
            }

            if (forceRefresh
                || _accessToken == null
                || string.IsNullOrWhiteSpace(_accessToken.Token)
                || DateTime.Now >= _expires)
            {
                await _semaphore.WaitAsync();

                if (forceRefresh
                    || _accessToken == null
                    || string.IsNullOrWhiteSpace(_accessToken.Token)
                    || DateTime.Now >= _expires)
                {
                    var result = await RetrieveAccessToken();

                    if (result != null
                        && !string.IsNullOrEmpty(result.access_token))
                    {
                        _accessToken = new AccessToken(result.access_token);
                        _expires = DateTimeOffset.FromUnixTimeSeconds(result.expires_in);
                    }

                    if (_accessToken == null || string.IsNullOrWhiteSpace(_accessToken.Token))
                    {
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
                var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_config.AppId}:{_config.AppSecret}"));
                var currentAuth = _httpClient.DefaultRequestHeaders.Authorization;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                FormUrlEncodedContent content = new FormUrlEncodedContent(postData);

                response = await _httpClient.PostAsync("https://www.reddit.com/api/v1/access_token", content);

                _httpClient.DefaultRequestHeaders.Authorization = currentAuth;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "RetrieveAccessToken Unable to retrieve access token");
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
                    //_logger.LogError(ex, "RetrieveAccessToken Unexpected response from server");
                    throw;
                }
            }

            return data!;
        }
    }
}
