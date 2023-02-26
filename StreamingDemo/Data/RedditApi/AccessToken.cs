using StreamingDemo.Data.RedditApi.Interfaces;

namespace StreamingDemo.Data.RedditApi
{
    public class AccessToken : IAccessToken
    {
        private readonly string _token;
        private readonly DateTimeOffset _expiration;
        public string Token => _token;
        public DateTimeOffset Expiration => _expiration;

        public AccessToken(string? accessToken, DateTimeOffset expiration)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            _token = accessToken;
            _expiration = expiration;
        }
    }
}
