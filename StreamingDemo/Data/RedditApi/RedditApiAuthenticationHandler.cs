using System.Net.Http.Headers;
using System.Net.Http;
using StreamingDemo.Models;
using System.Text;

namespace StreamingDemo.Data.RedditApi
{
    public class RedditApiAuthenticationHandler : DelegatingHandler
    {
        private readonly RedditTokenProvider _tokenProvider;

        private readonly string _redditAppId;
        private readonly string _redditAppSecret;

        private RedditAccessTokenResponse? _redditAccessToken;

        public RedditApiAuthenticationHandler(string appId, string appSecret, RedditTokenProvider tokenProvider) : base(new HttpClientHandler())
        {
            if (string.IsNullOrEmpty(appId))
            {
                throw new ArgumentNullException(nameof(appId));
            }
            if (string.IsNullOrEmpty(appSecret))
            {
                throw new ArgumentNullException(nameof(appSecret));
            }

            _redditAppId = appId;
            _redditAppSecret = appSecret;
            _tokenProvider = tokenProvider;

            _tokenProvider.SetCredentials(appId, appSecret);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_redditAccessToken == null || string.IsNullOrEmpty(_redditAccessToken.access_token))
            {
                _redditAccessToken = await _tokenProvider.GetTokenAsync();
            }
            if (!request.Headers.Contains("Authorization"))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _redditAccessToken.access_token);
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var token = await _tokenProvider.GetTokenAsync(true);

                if (string.IsNullOrWhiteSpace(token.access_token))
                {
                    return response;
                }
                
                _redditAccessToken = token;

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _redditAccessToken.access_token);
                response = await base.SendAsync(request, cancellationToken);
            }

            return response;
        }
    }
}
