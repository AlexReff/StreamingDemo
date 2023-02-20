using Microsoft.AspNetCore.SignalR;
using StreamingDemo.Data;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using System.Threading;

namespace StreamingDemo.Hubs
{
    public class RedditHub : Hub
    {
        private readonly ILogger<RedditHub> _logger;
        private readonly RedditApiReader _redditApi;

        private static int _usersConnected = 0;

        public RedditHub(ILogger<RedditHub> logger, RedditApiReader redditApiReader)
        {
            _logger = logger;
            _redditApi = redditApiReader;
        }

        public override async Task OnConnectedAsync()
        {
            // Start reading data when the first client connects
            if (_usersConnected++ == 0)
            {
                await _redditApi.StartStream();
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Stop reading data when the last client disconnects
            if (_usersConnected-- == 1)
            {
                await _redditApi.StopStream();
            }

            if (exception != null)
            {
                _logger.LogError(exception, "RedditHub::OnDisconnectedAsync Exception");
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task GetStatus()
        {
            switch(_redditApi.Status)
            {
                case RedditApiReaderStatus.Ready:
                    await Clients.Caller.SendAsync("config", "success").ConfigureAwait(false);
                    break;
                case RedditApiReaderStatus.MissingSecrets:
                    await Clients.Caller.SendAsync("config", "empty").ConfigureAwait(false);
                    break;
                case RedditApiReaderStatus.Error:
                    await Clients.Caller.SendAsync("config", "error").ConfigureAwait(false);
                    break;
                case RedditApiReaderStatus.Initialized:
                default:
                    //await Clients.Caller.SendAsync("tokenUrl", _redditApi.AuthorizeUrl).ConfigureAwait(false);
                    break;
            }
        }

        public async Task StartNewPosts()
        {
            await _redditApi.StartStream();
        }

        public async Task StopNewPosts()
        {
            await _redditApi.StopStream();
        }
    }
}
