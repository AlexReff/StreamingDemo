using Microsoft.AspNetCore.SignalR;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using StreamingDemo.Data.RedditApi;
using StreamingDemo.Data.RedditApi.Models;
using StreamingDemo.Data.RedditApi.Interfaces;

namespace StreamingDemo.Hubs
{
    public class RedditHub : Hub
    {
        private readonly ILogger<RedditHub> _logger;
        private readonly IRedditApiClient _redditApi;

        public RedditHub(ILogger<RedditHub> logger, RedditApiClient redditApiClient)
        {
            _logger = logger;
            _redditApi = redditApiClient;
        }

        public ChannelReader<PostData> NewPosts()
        {
            _redditApi.StartNewPosts();
            return _redditApi.NewPosts;
        }
    }
}
