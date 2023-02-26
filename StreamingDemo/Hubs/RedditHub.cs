using Microsoft.AspNetCore.SignalR;
using StreamingDemo.Data.RedditApi.Interfaces;
using System.Runtime.CompilerServices;

namespace StreamingDemo.Hubs
{
    public class RedditHub : Hub
    {
        private readonly ILogger<RedditHub> _logger;
        private readonly IRedditApiClient _redditApi;

        public RedditHub(ILogger<RedditHub> logger, IRedditApiClient redditApiClient)
        {
            _logger = logger;
            _redditApi = redditApiClient;
        }

        public async IAsyncEnumerable<IEnumerable<IPostData>> NewPosts([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            _logger.LogInformation("RedditHub:NewPosts activated");
            while (!cancellationToken.IsCancellationRequested)
            {
                await foreach (var posts in _redditApi.NewPosts.GetData(cancellationToken))
                {
                    _logger.LogInformation("RedditHub:NewPosts returning results");
                    yield return posts;
                }
            }
            _logger.LogInformation("RedditHub:NewPosts ended");
        }
    }
}
