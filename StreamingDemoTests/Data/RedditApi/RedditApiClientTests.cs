using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using StreamingDemo.Data.RedditApi;
using StreamingDemo.Data.RedditApi.Interfaces;
using StreamingDemo.Data.RedditApi.Models;
using System.Net.Http.Json;
using Xunit;

namespace StreamingDemo.Tests.Data.RedditApi
{
    public class RedditApiClientTests
    {
        private readonly Mock<IRedditHttpClient> _httpClientMock;
        private readonly Mock<ILogger<IRedditApiClient>> _loggerMock;

        public RedditApiClientTests()
        {
            _httpClientMock = new Mock<IRedditHttpClient>();
            _loggerMock = new Mock<ILogger<IRedditApiClient>>();
        }

        [Fact]
        public async Task GetNewPosts_ReturnsData()
        {
            // Arrange
            PostData[] data = new[]
            {
                new PostData { id = "1", title = "First post" },
                new PostData { id = "2", title = "Second post" },
            };

            _httpClientMock
                .Setup(x => x.HttpClient.GetAsync("/r/all/new?limit=100"))
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK, 
                    Content = JsonContent.Create(new NewPostResponse()
                    {
                        data = new NewData
                        {
                            children = data.Select(x => new PostListEntry() { data = x }).ToArray()
                        }
                    })
                });

            RedditApiClient apiClient = new RedditApiClient(_loggerMock.Object, _httpClientMock.Object);

            // Act
            var result = await apiClient.GetNewPosts();

            // Assert
            Assert.Equal(data.Length, result.Count());
            Assert.Equal(data[0].id, result.First().id);
            Assert.Equal(data[1].title, result.Last().title);
        }

        [Fact]
        public async Task GetNewPosts_ThrowsException_WhenHttpClientFails()
        {
            // Arrange
            _httpClientMock
                .Setup(x => x.HttpClient.GetAsync("/r/all/new?limit=100"))
                .ThrowsAsync(new HttpRequestException());

            RedditApiClient apiClient = new RedditApiClient(_loggerMock.Object, _httpClientMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => apiClient.GetNewPosts());
        }
    }
}