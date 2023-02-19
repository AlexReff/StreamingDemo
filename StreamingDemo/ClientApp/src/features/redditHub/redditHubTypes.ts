
export interface RedditApiPostData {
    id: string;
    subreddit: string;
    subreddit_name_prefixed: string;
    subreddit_type: string;
    title: string;
    name: string;
    url: string;
    upvote_ratio: number;
    score: number;
    selftext: string;
    gilded: number;
    clicked: boolean;
    link_flair_text: string;
    thumbnail: string;
    is_original_content: boolean;
    is_created_from_ads_ui: boolean;
    is_reddit_media_domain: boolean;
    is_meta: boolean;
    is_self: boolean;
    is_video: boolean;
    domain: string;
    over_18: boolean;
    spoiler: boolean;
    locked: boolean;
    author_premium: boolean;
    archived: boolean;
    pinned: boolean;
    author: string;
    contest_mode: boolean;
    permalink: string;
    stickied: boolean;
    author_flair_background_color: string;
    created_utc: number;
    ups: number;
    downs: number;
}
