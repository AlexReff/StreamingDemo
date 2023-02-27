namespace StreamingDemo.Data.RedditApi.Interfaces
{
    public interface IRedditHttpClient
    {
        IHttpClientWrapper HttpClient { get; }
    }
}
