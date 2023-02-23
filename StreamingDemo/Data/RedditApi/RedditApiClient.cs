
namespace StreamingDemo.Data.RedditApi
{
    public partial class RedditApiClient : HttpClient
    {
        private HttpClient _httpClient;

        private readonly ILogger<RedditApiClient> _logger;

        private readonly string _redditAppId;
        private readonly string _redditAppSecret;

        private DateTime _lastRequestTime;

        public RedditApiClient(IConfiguration config, ILogger<RedditApiClient> logger, RedditTokenProvider tokenProvider) : this()
        {
            _logger = logger;

            _redditAppId = config["Reddit:AppId"];
            _redditAppSecret = config["Reddit:AppSecret"];

            _httpClient = new HttpClient(new RedditApiAuthenticationHandler(_redditAppId, _redditAppSecret, tokenProvider));
            _httpClient.BaseAddress = new Uri("https://oauth.reddit.com");
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(config["Reddit:UserAgent"]);

            _lastRequestTime = DateTime.UtcNow;
        }

        private async Task AwaitHttpRequestLimit()
        {
            while (true)
            {
                var elapsedSeconds = (DateTime.UtcNow - _lastRequestTime).TotalSeconds;
                var secondsPerRequest = 1.0;

                if (elapsedSeconds >= secondsPerRequest)
                {
                    _lastRequestTime = DateTime.UtcNow;
                    break;
                }

                var timeToWait = TimeSpan.FromSeconds(secondsPerRequest - elapsedSeconds);
                await Task.Delay(timeToWait);
            }
        }
    }
}
