using StreamingDemo.Data.RedditApi.Interfaces;

namespace StreamingDemo.Data.RedditApi
{
    public class AccessToken : IAccessToken
    {
        private readonly string _token;
        public string Token => _token;

        public AccessToken(string? accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            _token = accessToken;
        }
    }
}
