using StreamingDemo.Data.RedditApi.Interfaces;
using StreamingDemo.Data.RedditApi.Models;
using System.Threading.Channels;

namespace StreamingDemo.Data.RedditApi
{
    public class RedditApiClient : IRedditApiClient
    {
        private readonly IRedditHttpClient _httpClient;
        private readonly ILogger<IRedditApiClient> _logger;

        //New Posts
        private readonly IntervalFuncCaller<IEnumerable<IPostData>> _newPostInterval;
        private readonly Channel<IEnumerable<IPostData>> _newPostsChannel;

        private readonly object _newPostsLock = new object();
        private bool _newPostsRunning;

        public ChannelReader<IEnumerable<IPostData>> NewPosts => _newPostsChannel.Reader;
        public bool NewPostsActive => _newPostsRunning;

        public RedditApiClient(ILogger<IRedditApiClient> logger, IRedditHttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;

            //New Posts
            _newPostsChannel = Channel.CreateUnbounded<IEnumerable<IPostData>>();
            _newPostInterval = new IntervalFuncCaller<IEnumerable<IPostData>>(_httpClient.GetNewPosts, 1, WriteNewPosts);
        }

        public void SetNewPostsActive(bool active)
        {
            lock (_newPostsLock)
            {
                if (active == _newPostsRunning)
                {
                    // attempting to activate while already active,
                    // or deactivating while already inactive
                    return;
                }
                if (active)
                {
                    _newPostsRunning = true;
                    _newPostInterval.Start();
                    _logger.LogInformation("RedditApi Start collecting new posts");
                }
                else
                {
                    _newPostsRunning = false;
                    _newPostInterval.Stop();
                    _logger.LogInformation("RedditApi Stop collecting new posts");
                }
            }
        }

        public async Task WriteNewPosts(IEnumerable<IPostData> postData)
        {
            await _newPostsChannel.Writer.WriteAsync(postData);
        }
    }
}
