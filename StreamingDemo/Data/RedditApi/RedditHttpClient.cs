using Microsoft.Extensions.Options;
using StreamingDemo.Data.RedditApi.Interfaces;
using System.Net.Http;

namespace StreamingDemo.Data.RedditApi
{
    public class RedditHttpClient : IRedditHttpClient
    {
        private readonly HttpClient _httpClient;
        public HttpClient HttpClient => _httpClient;

        public RedditHttpClient(IRedditApiConfig config, HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
    }
}
