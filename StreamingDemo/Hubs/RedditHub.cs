using Microsoft.AspNetCore.SignalR;
using System.Threading.Channels;
using StreamingDemo.Data.RedditApi;
using StreamingDemo.Data.RedditApi.Models;
using StreamingDemo.Data.RedditApi.Interfaces;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Linq;

namespace StreamingDemo.Hubs
{
    public class RedditHub : Hub
    {
        private readonly ILogger<RedditHub> _logger;
        private readonly IRedditApiClient _redditApi;

        private static readonly object _newPostsLock = new object();
        private static int _newPostCount = 0;

        public RedditHub(ILogger<RedditHub> logger, RedditApiClient redditApiClient)
        {
            _logger = logger;
            _redditApi = redditApiClient;
        }

        public async IAsyncEnumerable<PostData> NewPosts([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            lock (_newPostsLock)
            {
                _newPostCount++;
                if (_newPostCount == 1)
                {
                    _redditApi.StartNewPosts();
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
                    _redditApi.StopNewPosts();
                }
            }
        }
    }
}
