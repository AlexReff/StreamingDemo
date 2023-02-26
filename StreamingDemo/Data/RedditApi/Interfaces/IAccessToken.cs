namespace StreamingDemo.Data.RedditApi.Interfaces
{
    public interface IAccessToken
    {
        string Token { get; }
        DateTimeOffset Expiration { get; }
    }
}
