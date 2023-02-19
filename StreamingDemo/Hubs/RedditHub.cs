using Microsoft.AspNetCore.SignalR;
using Reddit;
using StreamingDemo.Data;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;

namespace StreamingDemo.Hubs
{
    public class RedditHub : Hub
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private readonly RedditApiReader _redditApi;

        private readonly string _redditCallbackUrl;

        private CancellationTokenSource _cancellationTokenSource;

        public RedditHub(IConfiguration config, RedditApiReader redditApiReader)
        {
            _config = config;
            _redditApi = redditApiReader;

            _redditCallbackUrl = _config["Server:CallbackUrl"];

            _cancellationTokenSource = new CancellationTokenSource();
            _httpClient = new HttpClient();
        }

        public override async Task OnConnectedAsync()
        {
            // Start reading data when the first client connects
            //await SubscribeToData();

            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            // Stop reading data when the last client disconnects
            //if (Clients.All.SendAsync 1)
            //{
            //    _cancellationTokenSource.Cancel();
            //    _cancellationTokenSource.Dispose();
            //    _cancellationTokenSource = new CancellationTokenSource();
            //}

            return base.OnDisconnectedAsync(exception);
        }

        public async Task Initialize()
        {
            switch(_redditApi.Status)
            {
                case ApiReaderStatus.Connected:
                    await Clients.Caller.SendAsync("config", "success").ConfigureAwait(false);
                    break;
                case ApiReaderStatus.MissingIdOrSecret:
                    await Clients.Caller.SendAsync("config", "empty").ConfigureAwait(false);
                    break;
                case ApiReaderStatus.Error:
                    await Clients.Caller.SendAsync("config", "error").ConfigureAwait(false);
                    break;
                case ApiReaderStatus.Initialized:
                case ApiReaderStatus.MissingToken:
                default:
                    await Clients.Caller.SendAsync("tokenUrl", _redditApi.AuthorizeUrl).ConfigureAwait(false);
                    break;
            }
        }

        public async Task CodeReceived(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentException(nameof(code));
            }

            _redditApi.RetrieveAccessTokens(code);
        }

        public async Task StartNewPosts(string subreddit)
        {
            try
            {
                // Continuously check the top posts every two seconds
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    await Task.Delay(2000); // Wait for two seconds

                    // Build the URL for the Reddit API endpoint
                    string url = $"https://www.reddit.com/r/all/new.json?limit=100000";

                    // Set the user agent to comply with the Reddit API rules
                    _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");

                    // Make the API request and get the response
                    HttpResponseMessage response = await _httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        // Parse the response and extract the top posts
                        string responseContent = await response.Content.ReadAsStringAsync();
                        //dynamic responseData = Newtonsoft.Json.JsonConvert.DeserializeObject(responseContent);
                        //string topPosts = responseData.data.children;

                        // Send the top posts to the client
                        await Clients.Caller.SendAsync("topPosts", responseContent);
                    }
                    else
                    {
                        // If the API request fails, send an error message to the client
                        await Clients.Caller.SendAsync("error", "Failed to get top posts from Reddit API.");
                    }
                }
            }
            catch (Exception ex)
            {
                // If an exception occurs, send an error message to the client
                await Clients.Caller.SendAsync("error", ex.Message);
            }
        }

        public void StopNewPosts()
        {
            // Cancel the continuous top posts checking loop
            _cancellationTokenSource.Cancel();
        }
    }
}
