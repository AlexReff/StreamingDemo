import { IRedditApiPostData } from "./redditHubTypes";

export enum RedditHubMessageType {
    Connect,
    Disconnect,
    Connected,
    Subscribe,
    Unsubscribe,
    Data,
    Error,
}

export enum RedditHubChannels {
    NewPosts,
}

export interface MessageDefault {
    messageType:
    | RedditHubMessageType.Disconnect
    | RedditHubMessageType.Connected;
}

export interface MessageError {
    messageType: RedditHubMessageType.Error;
    error: any;
}

export interface MessageConnect {
    messageType: RedditHubMessageType.Connect;
    url: string;
}

export interface MessageDataNewPosts {
    messageType: RedditHubMessageType.Data;
    dataType: RedditHubChannels.NewPosts;
    data: IRedditApiPostData[];
}

export interface MessageSubscribe {
    messageType: RedditHubMessageType.Subscribe | RedditHubMessageType.Unsubscribe;
    channel: RedditHubChannels;
}

export type RedditHubMessage =
    MessageDefault
    | MessageError
    | MessageConnect
    | MessageDataNewPosts
    | MessageSubscribe;

