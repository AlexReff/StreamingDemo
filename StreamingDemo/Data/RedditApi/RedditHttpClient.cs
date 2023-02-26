using StreamingDemo.Data.RedditApi.Interfaces;
using StreamingDemo.Data.RedditApi.Models;

namespace StreamingDemo.Data.RedditApi
{
    public class RedditHttpClient : IRedditHttpClient
    {
        private readonly ILogger<IRedditHttpClient> _logger;
        private readonly HttpClient _httpClient;

        public RedditHttpClient(ILogger<IRedditHttpClient> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<IPostData>> GetNewPosts()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://oauth.reddit.com/r/all/new?limit=100");

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadFromJsonAsync<NewPostResponse>();

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

            return Enumerable.Empty<IPostData>();
        }
    }
}
