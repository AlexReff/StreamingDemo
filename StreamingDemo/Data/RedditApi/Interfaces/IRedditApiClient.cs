using System.Threading.Channels;

namespace StreamingDemo.Data.RedditApi.Interfaces
{
    public interface IRedditApiClient
    {
        ManagedActiveChannel<IEnumerable<IPostData>> NewPosts { get; }
    }
}
