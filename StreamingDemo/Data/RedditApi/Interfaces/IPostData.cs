namespace StreamingDemo.Data.RedditApi.Interfaces
{
    public interface IPostData
    {
        public string subreddit { get; }
        public string thumbnail { get; }
        public string domain { get; }
        public bool is_original_content { get; }
        public bool is_meta { get; }
        public bool is_self { get; }
        public bool is_video { get; }
        public bool over_18 { get; }
        public bool spoiler { get; }

        public string id { get; }
        public string title { get; }
    }
}
