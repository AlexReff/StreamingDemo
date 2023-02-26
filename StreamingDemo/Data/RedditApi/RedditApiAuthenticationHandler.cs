using System.Net.Http.Headers;
using StreamingDemo.Data.RedditApi.Interfaces;

namespace StreamingDemo.Data.RedditApi
{
    public class RedditApiAuthenticationHandler : DelegatingHandler
    {
        private IAccessToken? _accessToken;

        private readonly IRedditTokenProvider _tokenProvider;

        public RedditApiAuthenticationHandler(IRedditTokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
            _accessToken = null;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_accessToken == null || string.IsNullOrEmpty(_accessToken.Token))
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
    }
}
