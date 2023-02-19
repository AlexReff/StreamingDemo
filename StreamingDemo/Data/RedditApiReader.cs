using Microsoft.AspNetCore.SignalR;
using Reddit;
using Reddit.AuthTokenRetriever;
using Reddit.AuthTokenRetriever.EventArgs;
using RestSharp.Extensions;
using RestSharp.Validation;
using StreamingDemo.Hubs;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using uhttpsharp;

namespace StreamingDemo.Data
{
    public enum ApiReaderStatus
    {
        Initialized,
        MissingIdOrSecret,
        MissingToken,
        Connected,
        Error,
    }

    public class RedditApiReader
    {
        // Reddit API limits to 30 updates / minute
        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(2);

        //private readonly SemaphoreSlim _readerStateLock = new SemaphoreSlim(1, 1);
        //private readonly SemaphoreSlim _updateStockPricesLock = new SemaphoreSlim(1, 1);

        //private readonly ConcurrentDictionary<string, int> _stocks = new ConcurrentDictionary<string, int>();

        //private static readonly Lazy<RedditApiReader> _instance = new Lazy<RedditApiReader>(() => new RedditApiReader());
        //private List<string> _data = new List<string>();

        private readonly ILogger<RedditApiReader> _logger;

        private readonly IConfiguration _config;
        private readonly IHubContext<RedditHub> _hub;
        private readonly HttpClient _httpClient;

        private readonly string _redditAppId;
        private readonly string _redditAppSecret;
        //private readonly int _serverPort;
        public readonly string _redditCallbackUrl;
        
        private static string? _redditAccessToken;
        private static string? _redditRefreshToken;
        private static string? _redditAuthorizeURL;

        private static RedditClient? _redditClient;
        //private static bool? _configSuccess;
        private static ApiReaderStatus _status;

        public RedditApiReader(IConfiguration config, IHubContext<RedditHub> hub, ILogger<RedditApiReader> logger)
        {
            _status = ApiReaderStatus.Initialized;

            _config = config;
            _hub = hub;
            _logger = logger;

            _httpClient = new HttpClient();

            //int.TryParse(_config["Server:Port"], out _serverPort);

            _redditAppId = _config["Reddit:AppId"];
            _redditAppSecret = _config["Reddit:AppSecret"];
            _redditAccessToken = _config["Reddit:AccessToken"];
            _redditRefreshToken = _config["Reddit:RefreshToken"];

            _redditCallbackUrl = _config["Server:CallbackUrl"];

            Init();
        }

        //private void AuthTokenRetrieverLib_AuthSuccess(object? sender, AuthSuccessEventArgs e)
        //{
        //    _redditAccessToken = e.AccessToken;
        //    _redditRefreshToken = e.RefreshToken;

        //    _logger.LogTrace($"RedditApiReader::AuthTokenRetrieverLib_AuthSuccess - AccessToken: {_redditAccessToken} | RefreshToken: {_redditRefreshToken}");
        //}

        private void Init()
        {
            if (string.IsNullOrWhiteSpace(_redditAppId) || string.IsNullOrWhiteSpace(_redditAppSecret))
            {
                _status = ApiReaderStatus.MissingIdOrSecret;
                return;
            }

            if (string.IsNullOrWhiteSpace(_redditAccessToken) || string.IsNullOrWhiteSpace(_redditRefreshToken))
            {
                //try to retrieve the token
                _status = ApiReaderStatus.MissingToken;

                _redditAuthorizeURL = $"https://www.reddit.com/api/v1/authorize?client_id={_redditAppId}&response_type=code&state={Guid.NewGuid().ToString("N")}&duration=permanent&scope=read&redirect_uri={HttpUtility.UrlEncode(_redditCallbackUrl)}";
            }
            else
            {
                // all 3 required fields are provided, attempt to connect
                InitializeRedditClient();
                ValidateStatus();
            }
        }

        private void InitializeRedditClient()
        {
            _redditClient = new RedditClient(appId: _redditAppId, appSecret: _redditAppSecret, accessToken: _redditAccessToken, refreshToken: _redditRefreshToken);
            _logger.LogTrace($"RedditApiReader::InitializeRedditClient - Reddit:AppId: {_redditAppId} | Reddit:AppSecret: {_redditAppSecret} | Reddit:AccessToken: {_redditAccessToken} | Reddit:RefreshToken: {_redditRefreshToken}");
        }

        private void ValidateStatus()
        {
            try
            {
                var count = _redditClient!.FrontPage.Count;

                _status = ApiReaderStatus.Connected;
                _redditAuthorizeURL = null;
            }
            catch (Exception ex)
            {
                _status = ApiReaderStatus.Error;
                _logger.LogTrace($"RedditApiReader::ValidateStatus - Reddit:AppId: {_redditAppId} | Reddit:AppSecret: {_redditAppSecret} | Reddit:AccessToken: {_redditAccessToken} | Reddit:RefreshToken: {_redditRefreshToken}");
                _logger.LogCritical(ex, "RedditApiReader::ValidateStatus - Failed to retrieve FrontPage Count");
            }
        }

        public ApiReaderStatus Status
        {
            get
            {
                return _status;
            }
        }

        public string? AuthorizeUrl
        {
            get
            {
                return _redditAuthorizeURL;
            }
        }

        public Dictionary<string, string> RetrieveAccessTokens(string code)
        {
            var postData = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", _redditCallbackUrl }
            };

            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_redditAppId}:{_redditAppSecret}"));

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

            var content = new FormUrlEncodedContent(postData);

            HttpResponseMessage response = await _httpClient.PostAsync("https://www.reddit.com/api/v1/access_token", content);
        }

        //public IObservable<object> StreamData()
        //{
        //    return _subject;
        //}

    }
}
