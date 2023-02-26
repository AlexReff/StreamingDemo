using StreamingDemo.Data.RedditApi.Interfaces;
using StreamingDemo.Data.RedditApi.Models;

namespace StreamingDemo.Data.RedditApi
{
    public class RedditApiClient : IRedditApiClient
    {
        private readonly IRedditHttpClient _httpClient;
        private readonly ILogger<IRedditApiClient> _logger;
        
        public ManagedActiveChannel<IEnumerable<IPostData>> NewPosts { get; } 

        public RedditApiClient(ILogger<IRedditApiClient> logger, IRedditHttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;

            NewPosts = new ManagedActiveChannel<IEnumerable<IPostData>>(
                async () => await GetNewPosts(),
                TimeSpan.FromSeconds(1)
            );

            _logger.LogInformation("RedditApiClient initialized");
        }

        protected async Task<IEnumerable<IPostData>> GetNewPosts()
        {
            try
            {
                var response = await _httpClient.HttpClient.GetAsync("/r/all/new?limit=100");

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
