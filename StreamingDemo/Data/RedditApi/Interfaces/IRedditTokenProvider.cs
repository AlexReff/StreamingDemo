
namespace StreamingDemo.Data.RedditApi.Interfaces
{
    public interface IRedditTokenProvider
    {
        Task<IAccessToken> GetTokenAsync(bool forceRefresh);
    }
}
