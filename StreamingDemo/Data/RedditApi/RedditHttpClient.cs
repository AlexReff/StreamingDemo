using StreamingDemo.Data.RedditApi.Interfaces;

namespace StreamingDemo.Data.RedditApi
{
    public class RedditHttpClient : IRedditHttpClient
    {
        public HttpClient HttpClient { get; }

        public RedditHttpClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }
    }
}
