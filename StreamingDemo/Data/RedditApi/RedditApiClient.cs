using Microsoft.AspNetCore.SignalR;
using StreamingDemo.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Channels;

namespace StreamingDemo.Data.RedditApi
{
    public class RedditApiClient : HttpClient
    {
        private HttpClient _httpClient;

        private readonly ILogger<RedditApiClient> _logger;
        private readonly RateLimiter _rateLimiter;

        private readonly string _redditAppId;
        private readonly string _redditAppSecret;

        private readonly IntervalFuncCaller<IEnumerable<RedditApiPostData>> _newPostInterval;

        private bool _newPostsRunning;
        private readonly Channel<RedditApiPostData> _newPostsChannel;
        public ChannelReader<RedditApiPostData> NewPosts => _newPostsChannel.Reader;

        public RedditApiClient(IConfiguration config, ILogger<RedditApiClient> logger, RedditTokenProvider tokenProvider)
        {
            _logger = logger;

            _redditAppId = config["Reddit:AppId"];
            _redditAppSecret = config["Reddit:AppSecret"];

            _httpClient = new HttpClient(new RedditApiAuthenticationHandler(_redditAppId, _redditAppSecret, tokenProvider));
            _httpClient.BaseAddress = new Uri("https://oauth.reddit.com");
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(config["Reddit:UserAgent"]);

            _rateLimiter = new RateLimiter(60, TimeSpan.FromMinutes(1));

            _newPostsChannel = Channel.CreateUnbounded<RedditApiPostData>();
            _newPostInterval = new IntervalFuncCaller<IEnumerable<RedditApiPostData>>(GetNewPosts, 1, WriteNewPosts);
        }

        public void StartNewPosts()
        {
            if (!_newPostsRunning)
            {
                _newPostsRunning = true;
                _newPostInterval.Start();
            }
        }

        public async Task<IEnumerable<RedditApiPostData>> GetNewPosts()
        {
            try
            {
                var response = await _rateLimiter.ExecuteAsync(async () =>
                {
                    return await _httpClient.GetAsync("/r/all/new?limit=100");
                });

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadFromJsonAsync<RedditApiPostListResponse>();

                if (content?.data?.children?.Count() > 0)
                {
                    return content.data.children.Select(m => m.data);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetNewPosts - Unable to retrieve new posts");
                throw;
            }

            return Enumerable.Empty<RedditApiPostData>();
        }

        public async Task WriteNewPosts(IEnumerable<RedditApiPostData> postData)
        {
            Parallel.ForEach(postData, post =>
            {
                _newPostsChannel.Writer.TryWrite(post);
            });
        }
    }
}
