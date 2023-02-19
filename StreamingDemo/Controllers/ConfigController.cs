using Microsoft.AspNetCore.Mvc;
using StreamingDemo.Data;

namespace StreamingDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly RedditApiReader _redditApi;

        public ConfigController(IConfiguration config, RedditApiReader redditApiReader) : base()
        {
            _config = config;

            _redditApi = redditApiReader;
        }

        [HttpGet("status")]
        public async Task<JsonResult> Status()
        {
            var result = _redditApi.Status;

            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<JsonResult> AuthorizeUrl()
        {
            var result = _redditApi.AuthorizeUrl;

            return new JsonResult(result);
        }
    }
}
