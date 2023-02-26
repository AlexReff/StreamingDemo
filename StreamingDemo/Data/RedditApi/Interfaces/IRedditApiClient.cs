using System.Threading.Channels;

namespace StreamingDemo.Data.RedditApi.Interfaces
{
    public interface IRedditApiClient
    {
        //New Posts
        ChannelReader<IEnumerable<IPostData>> NewPosts { get; }
        bool NewPostsActive { get; }

        void SetNewPostsActive(bool active);
        Task WriteNewPosts(IEnumerable<IPostData> postData);
    }
}
