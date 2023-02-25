using Newtonsoft.Json;

namespace StreamingDemo.Data.RedditApi.Models
{
    public class NewPostResponse
    {
        public string kind { get; set; }
        public NewData data { get; set; }
    }

    public class NewData
    {
        public IEnumerable<PostListEntry> children { get; set; }
    }

    public class PostListEntry
    {
        public string kind { get; set; }
        public PostData data { get; set; }
    }

    public class PostData
    {
        public string subreddit { get; set; }
        public string thumbnail { get; set; }
        public string domain { get; set; }
        public bool is_original_content { get; set; }
        public bool is_meta { get; set; }
        public bool is_self { get; set; }
        public bool is_video { get; set; }
        public bool over_18 { get; set; }
        public bool spoiler { get; set; }

        [JsonIgnore]
        public string id { get; set; }
        [JsonIgnore]
        public string subreddit_name_prefixed { get; set; }
        [JsonIgnore]
        public string subreddit_type { get; set; }
        [JsonIgnore]
        public string title { get; set; }
        [JsonIgnore]
        public string name { get; set; }
        [JsonIgnore]
        public string url { get; set; }
        [JsonIgnore]
        public double upvote_ratio { get; set; }
        [JsonIgnore]
        public string selftext { get; set; }
        [JsonIgnore]
        public string link_flair_text { get; set; }
        [JsonIgnore]
        public string author { get; set; }
        [JsonIgnore]
        public string permalink { get; set; }
        [JsonIgnore]
        public string author_flair_background_color { get; set; }
        [JsonIgnore]
        public object created_utc { get; set; }
        [JsonIgnore]
        public object created { get; set; }
        [JsonIgnore]
        public int gilded { get; set; }
        [JsonIgnore]
        public int score { get; set; }
        [JsonIgnore]
        public int ups { get; set; }
        [JsonIgnore]
        public int downs { get; set; }
        [JsonIgnore]
        public bool clicked { get; set; }
        [JsonIgnore]
        public bool is_created_from_ads_ui { get; set; }
        [JsonIgnore]
        public bool is_reddit_media_domain { get; set; }
        [JsonIgnore]
        public bool locked { get; set; }
        [JsonIgnore]
        public bool author_premium { get; set; }
        [JsonIgnore]
        public bool archived { get; set; }
        [JsonIgnore]
        public bool pinned { get; set; }
        [JsonIgnore]
        public bool contest_mode { get; set; }
        [JsonIgnore]
        public bool stickied { get; set; }
        /*
        public int dist { get; set; }
        public string modhash { get; set; }
        public string geo_filter { get; set; }
        public List<object> children { get; set; }
        public object before { get; set; }
        public object approved_at_utc { get; set; }
        public string author_fullname { get; set; }
        public bool saved { get; set; }
        public object mod_reason_title { get; set; }
        public List<object> link_flair_richtext { get; set; }
        public bool hidden { get; set; }
        public int? pwls { get; set; }
        public string link_flair_css_class { get; set; }
        public int? thumbnail_height { get; set; }
        public object top_awarded_type { get; set; }
        public bool hide_score { get; set; }
        public bool quarantine { get; set; }
        public string link_flair_text_color { get; set; }
        public int upvote_ratio { get; set; }
        public int total_awards_received { get; set; }
        public object media_embed { get; set; }
        public int? thumbnail_width { get; set; }
        public string author_flair_template_id { get; set; }
        public List<object> user_reports { get; set; }
        public object secure_media { get; set; }
        public object category { get; set; }
        public object secure_media_embed { get; set; }
        public bool can_mod_post { get; set; }
        public object approved_by { get; set; }
        public bool edited { get; set; }
        public string author_flair_css_class { get; set; }
        public List<object> author_flair_richtext { get; set; }
        public object gildings { get; set; }
        public object content_categories { get; set; }
        public object mod_note { get; set; }
        public string link_flair_type { get; set; }
        public int? wls { get; set; }
        public object removed_by_category { get; set; }
        public object banned_by { get; set; }
        public string author_flair_type { get; set; }
        public bool allow_live_comments { get; set; }
        public string selftext_html { get; set; }
        public object likes { get; set; }
        public string suggested_sort { get; set; }
        public object banned_at_utc { get; set; }
        public object view_count { get; set; }
        public bool no_follow { get; set; }
        public bool is_crosspostable { get; set; }
        public List<object> all_awardings { get; set; }
        public List<object> awarders { get; set; }
        public bool media_only { get; set; }
        public string link_flair_template_id { get; set; }
        public bool can_gild { get; set; }
        public string author_flair_text { get; set; }
        public List<object> treatment_tags { get; set; }
        public bool visited { get; set; }
        public object removed_by { get; set; }
        public object num_reports { get; set; }
        public string distinguished { get; set; }
        public string subreddit_id { get; set; }
        public bool author_is_blocked { get; set; }
        public object mod_reason_by { get; set; }
        public object removal_reason { get; set; }
        public string link_flair_background_color { get; set; }
        public bool is_robot_indexable { get; set; }
        public object report_reasons { get; set; }
        public object discussion_type { get; set; }
        public int num_comments { get; set; }
        public bool send_replies { get; set; }
        public string whitelist_status { get; set; }
        public object poll_data { get; set; }
        public bool author_patreon_flair { get; set; }
        public string author_flair_text_color { get; set; }
        public string parent_whitelist_status { get; set; }
        public List<object> mod_reports { get; set; }
        public int subreddit_subscribers { get; set; }
        public int num_crossposts { get; set; }
        public object media { get; set; }
        public string post_hint { get; set; }
        public string url_overridden_by_dest { get; set; }
        public object preview { get; set; }
        public object media_metadata { get; set; }
        public bool? is_gallery { get; set; }
        public object gallery_data { get; set; }
        */
    }
}
