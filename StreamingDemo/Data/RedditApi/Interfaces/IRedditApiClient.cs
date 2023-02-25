using StreamingDemo.Data.RedditApi.Models;
using System.Threading.Channels;

namespace StreamingDemo.Data.RedditApi.Interfaces
{
    public interface IRedditApiClient
    {
        //New Posts
        ChannelReader<PostData> NewPosts { get; }
        void StartNewPosts();
        void StopNewPosts();
        Task<IEnumerable<PostData>> GetNewPosts();
        Task WriteNewPosts(IEnumerable<PostData> postData);
    }
}
