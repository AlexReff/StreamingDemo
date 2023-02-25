using StreamingDemo.Data.RedditApi.Interfaces;
using StreamingDemo.Data.RedditApi.Models;
using System.Threading.Channels;

namespace StreamingDemo.Data.RedditApi
{
    public class RedditApiClient : IRedditApiClient
    {
        private IRedditHttpClient _httpClient;
        private readonly ILogger<RedditApiClient> _logger;

        //New Posts
        private readonly IntervalFuncCaller<IEnumerable<PostData>> _newPostInterval;
        private readonly Channel<PostData> _newPostsChannel;

        private readonly object _newPostsLock = new object();
        private bool _newPostsRunning;
        public ChannelReader<PostData> NewPosts => _newPostsChannel.Reader;

        public RedditApiClient(ILogger<RedditApiClient> logger, IRedditHttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;

            //New Posts
            _newPostsChannel = Channel.CreateUnbounded<PostData>();
            _newPostInterval = new IntervalFuncCaller<IEnumerable<PostData>>(GetNewPosts, 1, WriteNewPosts);
        }

        private async Task AwaitHttpRequestLimit()
        {
            while (true)
            {
                lock (_newPostsLock)
                {
                    if (!_newPostsRunning)
                    {
                        _newPostsRunning = true;
                        _newPostInterval.Start();
                    }
                }
            }
        }

        public void StopNewPosts()
        {
            if (_newPostsRunning)
            {
                lock (_newPostsLock)
                {
                    if (_newPostsRunning)
                    {
                        _newPostsRunning = false;
                        _newPostInterval.Stop();
                    }
                }
            }
        }

        public async Task<IEnumerable<PostData>> GetNewPosts()
        {
            try
            {
                var response = await _httpClient.HttpClient.GetAsync("https://oauth.reddit.com/r/all/new?limit=100");

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadFromJsonAsync<NewPostResponse>();

                if (content?.data?.children?.Count() > 0)
                {
                    return content.data.children.Select(m => m.data);
                }

            return Enumerable.Empty<PostData>();
        }

        public async Task WriteNewPosts(IEnumerable<PostData> postData)
        {
            Parallel.ForEach(postData, post =>
            {
                _newPostsChannel.Writer.TryWrite(post);
            });
        }
    }
}
