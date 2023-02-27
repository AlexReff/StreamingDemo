using StreamingDemo.Data.RedditApi.Interfaces;

namespace StreamingDemo.Data.RedditApi
{
    public class RedditHttpClient : IRedditHttpClient
    {
        public IHttpClientWrapper HttpClient { get; }

        public RedditHttpClient(IHttpClientWrapper httpClient)
        {
            HttpClient = httpClient;
        }
    }
}
