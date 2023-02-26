
namespace StreamingDemo.Data.RedditApi.Interfaces
{
    public interface IRedditHttpClient
    {
        Task<IEnumerable<IPostData>> GetNewPosts();
    }
}
