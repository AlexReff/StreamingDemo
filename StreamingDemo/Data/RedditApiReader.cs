using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using StreamingDemo.Hubs;
using StreamingDemo.Models;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Web;

namespace StreamingDemo.Data
{
    public enum RedditApiReaderStatus
    {
        Initialized,
        MissingSecrets,
        Ready,
        Error,
    }

    public class RedditApiReader
    {
        // Reddit API limits to 60 updates / minute
        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(3);

        private readonly SemaphoreSlim _accessTokenSemaphore = new SemaphoreSlim(1);
        private static readonly object _fetchingNewLock = new object();

        private readonly ILogger<RedditApiReader> _logger;

        private readonly IConfiguration _config;
        private readonly IHubContext<RedditHub> _hubContext;
        private readonly HttpClient _httpClient;
        private CancellationTokenSource _cancellationTokenSource;

        private readonly string _redditAppId;
        private readonly string _redditAppSecret;
        
        private static string? _redditAccessToken;

        private static RedditApiReaderStatus _status;
        private static bool _fetchingNew = false;

        public RedditApiReaderStatus Status
        {
            get
            {
                return _status;
            }
        }

        public RedditApiReader(IConfiguration config, IHubContext<RedditHub> hubContext, ILogger<RedditApiReader> logger)
        {
            _status = RedditApiReaderStatus.Initialized;

            _config = config;
            _hubContext = hubContext;
            _logger = logger;

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://oauth.reddit.com");
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("reffdev-streaming-demo/v0.1.0 (by /u/reffdev)");

            _redditAppId = _config["Reddit:AppId"];
            _redditAppSecret = _config["Reddit:AppSecret"];

            Init();
        }

        private void Init()
        {
            if (string.IsNullOrWhiteSpace(_redditAppId) || string.IsNullOrWhiteSpace(_redditAppSecret))
            {
                _status = RedditApiReaderStatus.MissingSecrets;
                return;
            }

            if (string.IsNullOrWhiteSpace(_redditAccessToken))
            {
                var token = RetrieveAccessToken();
                token.Wait();

                _redditAccessToken = token.Result;
            }

            if (!string.IsNullOrWhiteSpace(_redditAccessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _redditAccessToken);
                _status = RedditApiReaderStatus.Ready;
            }
            else
            {
                _status = RedditApiReaderStatus.Error;
            }
        }

        public async Task StartStream()
        {
            if (_status != RedditApiReaderStatus.Ready)
            {
                return;
            }

            lock (_fetchingNewLock)
            {
                if (!_fetchingNew)
                {
                    _fetchingNew = true;
                    _cancellationTokenSource = new CancellationTokenSource();
                    Task.Run(async () => await RetrieveRandomPosts(_cancellationTokenSource.Token));
                }
            }
        }

        public async Task StopStream()
        {
            if (_fetchingNew)
            {
                _fetchingNew = false;
                _cancellationTokenSource.Cancel();
            }
        }

        public async Task RetrieveRandomPosts(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    response = await _httpClient.GetAsync("/r/all/new?limit=100");

                    if (response == null || !response.IsSuccessStatusCode)
                    {
                        break;
                    }

                    //string strRes = await response.Content.ReadAsStringAsync();
                    var content = await response.Content.ReadFromJsonAsync<RedditApiPostListResponse>();

                    if (content != null)
                    {
                        var message = content.data.children.Select(m => m.data);

                        await _hubContext.Clients.All/*.Group("DataUpdates")*/.SendAsync("data", message);
                    }
                }
                catch (Exception ex)
                {
                    // Handle exception or log error
                    _logger.LogError(ex, "RedditApiReader::RetrieveNewPosts - Unable to retrieve new posts");
                    break;
                }

                await Task.Delay(_updateInterval, cancellationToken);
            }

            if (response != null)
            {
                _logger.LogInformation($"RedditApiReader::RetrieveNewPosts Last response: {response.StatusCode} {response.ReasonPhrase}, {response.IsSuccessStatusCode}, {response.Content}");
            }
        }

        private async Task<string> RetrieveAccessToken()
        {
            if (!string.IsNullOrWhiteSpace(_redditAccessToken))
            {
                return _redditAccessToken;
            }

            await _accessTokenSemaphore.WaitAsync();

            if (!string.IsNullOrWhiteSpace(_redditAccessToken))
            {
                _accessTokenSemaphore.Release();
                return _redditAccessToken;
            }

            var postData = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" }
            };

            HttpResponseMessage response = null;

            try
            {
                var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_redditAppId}:{_redditAppSecret}"));

                var currentAuth = _httpClient.DefaultRequestHeaders.Authorization;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                FormUrlEncodedContent content = new FormUrlEncodedContent(postData);

                response = await _httpClient.PostAsync("https://www.reddit.com/api/v1/access_token", content);

                _httpClient.DefaultRequestHeaders.Authorization = currentAuth;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RedditApiReader::RetrieveAccessToken - Unable to retrieve access token");
            }

            if (response != null)
            {
                RedditAccessTokenResponse? data = null;

                try
                {
                    string cont = await response.Content.ReadAsStringAsync();
                    data = await response.Content.ReadFromJsonAsync<RedditAccessTokenResponse>();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "RedditApiReader::RetrieveAccessToken - Unexpected response from server");
                }

                if (data != null && !string.IsNullOrWhiteSpace(data.access_token))
                {
                    _redditAccessToken = data.access_token;
                }
            }

            if (string.IsNullOrWhiteSpace(_redditAccessToken))
            {
                _logger.LogTrace($"RedditApiReader::RetrieveAccessToken - Failed to retrieve access token for appId: ${_redditAppId} | secret: ${_redditAppSecret}");
                _logger.LogError("RedditApiReader::RetrieveAccessToken - Unable to retrieve Access Token");
            }

            _accessTokenSemaphore.Release();
            return _redditAccessToken;
        }
    }
}
