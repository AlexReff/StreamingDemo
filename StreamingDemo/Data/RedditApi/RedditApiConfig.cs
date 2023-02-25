using StreamingDemo.Data.RedditApi.Interfaces;

namespace StreamingDemo.Data.RedditApi
{
    public class RedditApiConfig : IRedditApiConfig
    {
        public string AppId { get; }
        public string AppSecret { get; }
        public string UserAgent { get; }

        public RedditApiConfig(ConfigurationManager config)
        {
            AppId = config["Reddit:AppId"];
            AppSecret = config["Reddit:AppSecret"];
            UserAgent = config["Reddit:UserAgent"];
        }
    }
}
