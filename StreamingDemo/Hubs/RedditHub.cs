using Microsoft.AspNetCore.SignalR;
using StreamingDemo.Data.RedditApi;
using StreamingDemo.Data.RedditApi.Interfaces;
using System.Runtime.CompilerServices;

namespace StreamingDemo.Hubs
{
    public class RedditHub : Hub
    {
        private readonly ILogger<RedditHub> _logger;
        private readonly IRedditApiClient _redditApi;

        private static readonly object _newPostsLock = new object();
        private static int _newPostCount = 0;

        public RedditHub(ILogger<RedditHub> logger, IRedditApiClient redditApiClient)
        {
            _logger = logger;
            _redditApi = redditApiClient;
        }

        public async IAsyncEnumerable<IEnumerable<IPostData>> NewPosts([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            lock (_newPostsLock)
            {
                _newPostCount++;
                if (_newPostCount == 1)
                {
                    _redditApi.SetNewPostsActive(true);
                }
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                yield return await _redditApi.NewPosts.ReadAsync();
            }

            lock (_newPostsLock)
            {
                _newPostCount--;
                if (_newPostCount == 0)
                {
                    _redditApi.SetNewPostsActive(false);
                }
            }
        }
    }
}
