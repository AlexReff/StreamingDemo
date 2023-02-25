using StreamingDemo.Models;
using System.Threading.Channels;

namespace StreamingDemo.Data.RedditApi
{
    public partial class RedditApiClient
    {
        private readonly IntervalFuncCaller<IEnumerable<RedditApiPostData>> _newPostInterval;
        private readonly Channel<RedditApiPostData> _newPostsChannel;

        private bool _newPostsRunning;
        public ChannelReader<RedditApiPostData> NewPosts => _newPostsChannel.Reader;

        public RedditApiClient()
        {
            _newPostsChannel = Channel.CreateUnbounded<RedditApiPostData>();
            _newPostInterval = new IntervalFuncCaller<IEnumerable<RedditApiPostData>>(GetNewPosts, 1, WriteNewPosts);
        }

        public void StartNewPosts()
        {
            if (!_newPostsRunning)
            {
                _newPostsRunning = true;
                _newPostInterval.Start();
            }
        }

        public async Task<IEnumerable<RedditApiPostData>> GetNewPosts()
        {
            try
            {
                await AwaitHttpRequestLimit();

                var response = await _httpClient.GetAsync("/r/all/new?limit=100");

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadFromJsonAsync<RedditApiPostListResponse>();

                if (content?.data?.children?.Count() > 0)
                {
                    return content.data.children.Select(m => m.data);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetNewPosts - Unable to retrieve new posts");
                throw;
            }

            return Enumerable.Empty<RedditApiPostData>();
        }

        public async Task WriteNewPosts(IEnumerable<RedditApiPostData> postData)
        {
            Parallel.ForEach(postData, post =>
            {
                _newPostsChannel.Writer.TryWrite(post);
            });
        }
    }
}
