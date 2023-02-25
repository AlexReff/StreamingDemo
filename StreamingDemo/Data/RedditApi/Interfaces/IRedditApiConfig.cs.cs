namespace StreamingDemo.Data.RedditApi.Interfaces
{
    public interface IRedditApiConfig
    {
        string AppId { get; }
        string AppSecret { get; }
        string UserAgent { get; }
    }
}
