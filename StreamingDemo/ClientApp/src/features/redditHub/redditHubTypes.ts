
export interface RedditApiResponse {
  kind: string
  data: RedditApiResponseData[]
}

export interface RedditApiResponseData {
  approved_at_utc: any
  subreddit: string
  selftext: string
  author_fullname: string
  saved: boolean
  mod_reason_title: any
  gilded: number
  clicked: boolean
  title: string
  link_flair_richtext: LinkFlairRichtext[]
  subreddit_name_prefixed: string
  hidden: boolean
  pwls?: number
  link_flair_css_class?: string
  downs: number
  thumbnail_height?: number
  top_awarded_type: any
  hide_score: boolean
  name: string
  quarantine: boolean
  link_flair_text_color?: string
  upvote_ratio: number
  author_flair_background_color: any
  subreddit_type: string
  ups: number
  total_awards_received: number
  media_embed: MediaEmbed
  thumbnail_width?: number
  author_flair_template_id: any
  is_original_content: boolean
  user_reports: any[]
  secure_media?: SecureMedia
  is_reddit_media_domain: boolean
  is_meta: boolean
  category: any
  secure_media_embed: SecureMediaEmbed
  link_flair_text?: string
  can_mod_post: boolean
  score: number
  approved_by: any
  is_created_from_ads_ui: boolean
  author_premium: boolean
  thumbnail: string
  edited: boolean
  author_flair_css_class: any
  author_flair_richtext: any[]
  gildings: Gildings
  content_categories: any
  is_self: boolean
  mod_note: any
  created: number
  link_flair_type: string
  wls?: number
  removed_by_category: any
  banned_by: any
  author_flair_type: string
  domain: string
  allow_live_comments: boolean
  selftext_html?: string
  likes: any
  suggested_sort?: string
  banned_at_utc: any
  url_overridden_by_dest?: string
  view_count: any
  archived: boolean
  no_follow: boolean
  is_crosspostable: boolean
  pinned: boolean
  over_18: boolean
  all_awardings: any[]
  awarders: any[]
  media_only: boolean
  can_gild: boolean
  spoiler: boolean
  locked: boolean
  author_flair_text: any
  treatment_tags: any[]
  visited: boolean
  removed_by: any
  num_reports: any
  distinguished: any
  subreddit_id: string
  author_is_blocked: boolean
  mod_reason_by: any
  removal_reason: any
  link_flair_background_color?: string
  id: string
  is_robot_indexable: boolean
  report_reasons: any
  author: string
  discussion_type?: string
  num_comments: number
  send_replies: boolean
  whitelist_status?: string
  contest_mode: boolean
  mod_reports: any[]
  author_patreon_flair: boolean
  author_flair_text_color: any
  permalink: string
  parent_whitelist_status?: string
  stickied: boolean
  url: string
  subreddit_subscribers: number
  created_utc: number
  num_crossposts: number
  media?: Media
  is_video: boolean
  post_hint?: string
  preview?: Preview
  link_flair_template_id?: string
  is_gallery?: boolean
  media_metadata?: MediaMetadata
  gallery_data?: GalleryData
  crosspost_parent_list?: CrosspostParentList[]
  crosspost_parent?: string
  poll_data?: PollData
  author_cakeday?: boolean
}

interface LinkFlairRichtext {
  a?: string
  e: string
  u?: string
  t?: string
}

interface MediaEmbed {
  content?: string
  width?: number
  scrolling?: boolean
  height?: number
}

interface SecureMedia {
  oembed: Oembed
  type: string
}

interface Oembed {
  provider_url: string
  title: string
  html: string
  thumbnail_width: number
  height: number
  width: number
  version: string
  author_name: string
  provider_name: string
  thumbnail_url: string
  type: string
  thumbnail_height: number
  author_url: string
}

interface SecureMediaEmbed {
  content?: string
  width?: number
  scrolling?: boolean
  media_domain_url?: string
  height?: number
}

interface Gildings {}

interface Media {
  oembed: Oembed
  type: string
}

interface Preview {
  images: Image[]
  enabled: boolean
}

interface Image {
  source: Source
  resolutions: Resolution[]
  variants: Variants
  id: string
}

interface Source {
  url: string
  width: number
  height: number
}

interface Resolution {
  url: string
  width: number
  height: number
}

interface Variants {
  gif?: Gif
  mp4?: Mp4
}

interface Gif {
  source: Source
  resolutions: Resolution2[]
}

interface Resolution2 {
  url: string
  width: number
  height: number
}

interface Mp4 {
  source: Source
  resolutions: Resolution[]
}


interface MediaMetadata {
  
}

interface GalleryData {
  items: Item[]
}

interface Item {
  media_id: string
  id: number
}

interface CrosspostParentList {
  approved_at_utc: any
  subreddit: string
  selftext: string
  author_fullname: string
  saved: boolean
  mod_reason_title: any
  gilded: number
  clicked: boolean
  title: string
  link_flair_richtext: any[]
  subreddit_name_prefixed: string
  hidden: boolean
  pwls?: number
  link_flair_css_class?: string
  downs: number
  thumbnail_height: number
  top_awarded_type: any
  hide_score: boolean
  name: string
  quarantine: boolean
  link_flair_text_color: string
  upvote_ratio: number
  author_flair_background_color: any
  ups: number
  total_awards_received: number
  media_embed: MediaEmbed
  thumbnail_width: number
  author_flair_template_id: any
  is_original_content: boolean
  user_reports: any[]
  secure_media?: SecureMedia
  is_reddit_media_domain: boolean
  is_meta: boolean
  category: any
  secure_media_embed: SecureMediaEmbed
  link_flair_text?: string
  can_mod_post: boolean
  score: number
  approved_by: any
  is_created_from_ads_ui: boolean
  author_premium: boolean
  thumbnail: string
  edited: boolean
  author_flair_css_class: any
  author_flair_richtext: any[]
  gildings: Gildings
  post_hint: string
  content_categories: any
  is_self: boolean
  subreddit_type: string
  created: number
  link_flair_type: string
  wls?: number
  removed_by_category: any
  banned_by: any
  author_flair_type: string
  domain: string
  allow_live_comments: boolean
  selftext_html: any
  likes: any
  suggested_sort: any
  banned_at_utc: any
  url_overridden_by_dest: string
  view_count: any
  archived: boolean
  no_follow: boolean
  is_crosspostable: boolean
  pinned: boolean
  over_18: boolean
  preview: Preview
  all_awardings: any[]
  awarders: any[]
  media_only: boolean
  link_flair_template_id?: string
  can_gild: boolean
  spoiler: boolean
  locked: boolean
  author_flair_text: any
  treatment_tags: any[]
  visited: boolean
  removed_by: any
  mod_note: any
  distinguished: any
  subreddit_id: string
  author_is_blocked: boolean
  mod_reason_by: any
  num_reports: any
  removal_reason: any
  link_flair_background_color: string
  id: string
  is_robot_indexable: boolean
  report_reasons: any
  author: string
  discussion_type: any
  num_comments: number
  send_replies: boolean
  whitelist_status?: string
  contest_mode: boolean
  mod_reports: any[]
  author_patreon_flair: boolean
  author_flair_text_color: any
  permalink: string
  parent_whitelist_status?: string
  stickied: boolean
  url: string
  subreddit_subscribers: number
  created_utc: number
  num_crossposts: number
  media?: Media
  is_video: boolean
}

interface RedditVideo {
  bitrate_kbps: number
  fallback_url: string
  height: number
  width: number
  scrubber_media_url: string
  dash_url: string
  duration: number
  hls_url: string
  is_gif: boolean
  transcoding_status: string
}

interface PollData {
  prediction_status: any
  total_stake_amount: any
  voting_end_timestamp: number
  options: Option[]
  vote_updates_remained: any
  is_prediction: boolean
  resolved_option_id: any
  user_won_amount: any
  user_selection: any
  total_vote_count: number
  tournament_id: any
}

interface Option {
  text: string
  id: string
}
