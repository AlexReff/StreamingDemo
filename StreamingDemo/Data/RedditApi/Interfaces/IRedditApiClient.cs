namespace StreamingDemo.Data.RedditApi.Interfaces
{
    public interface IRedditApiClient
    {
        IManagedActiveChannel<IEnumerable<IPostData>> NewPosts { get; }
    }
}
