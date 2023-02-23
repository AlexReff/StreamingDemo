using Microsoft.AspNetCore.SignalR;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using System.Threading;
using StreamingDemo.Models;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using StreamingDemo.Data.RedditApi;

namespace StreamingDemo.Hubs
{
    public class RedditHub : Hub
    {
        private readonly ILogger<RedditHub> _logger;
        private readonly RedditApiClient _redditApi;

        public RedditHub(ILogger<RedditHub> logger, RedditApiClient RedditApiClient)
        {
            _logger = logger;
            _redditApi = RedditApiClient;
        }

        public ChannelReader<RedditApiPostData> NewPosts()
        {
            _redditApi.StartNewPosts();
            return _redditApi.NewPosts;
        }
    }
}
