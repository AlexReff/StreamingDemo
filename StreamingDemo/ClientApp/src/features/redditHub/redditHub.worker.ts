import { HubConnection, HubConnectionBuilder, LogLevel, ISubscription, HubConnectionState } from "@microsoft/signalr";
import { IntervalFunction } from "../utillity/helpers";
import { RedditHubChannels, RedditHubMessage, RedditHubMessageType, MessageError, MessageConnect, MessageSubscribe } from "./redditHub.worker.types";
import { IRedditApiPostData } from "./redditHubTypes";

let connection: HubConnection | null = null;
let newPostReader: ISubscription<IRedditApiPostData>;

const activeSubscriptions: Set<RedditHubChannels> = new Set<RedditHubChannels>();
const newPostStack: IRedditApiPostData[][] = [];

const post = self.postMessage;

self.onmessage = (messageString: MessageEvent<string>) => {
    let message: RedditHubMessage | null = null;
    try {
        message = JSON.parse(messageString.data);
    } catch (ex) {
        //
    }
    if (message == null) {
        return;
    }
    switch (message.messageType) {
        case RedditHubMessageType.Connect:
            connect((message as MessageConnect).url).then(() => {
                activeSubscriptions.clear();
                postMessage(JSON.stringify({ messageType: RedditHubMessageType.Connected }));
            });
            break;
        case RedditHubMessageType.Disconnect:
            disconnect().then(() => {
                activeSubscriptions.clear();
            });
            break;
        case RedditHubMessageType.Subscribe:
            subscribe((message as MessageSubscribe).channel).then(() => {
                // console.log("Worker Subscribed", message);
            });
            break;
        case RedditHubMessageType.Unsubscribe:
            unsubscribe((message as MessageSubscribe).channel).then(() => {
                // console.log("Worker Unsubscribed", message);
            });
            break;
        default:
            return;
    }
};

const parseNewPostStack = new IntervalFunction(async () => {
    if (newPostStack.length == 0) {
        return;
    }
    const batch = newPostStack.shift();
    if (batch != null && batch.length > 0) {
        post(JSON.stringify({
            messageType: RedditHubMessageType.Data,
            dataType: RedditHubChannels.NewPosts, data: batch,
        }));
    }
}, 1000);


const connect = async (url: string) => {
    if (!url) {
        throw new Error("Connect called without url");
    }
    if (connection != null) {
        await connection.stop();
    }

    connection = new HubConnectionBuilder()
        .withUrl(`https://${location.host}${url}`)
        .withAutomaticReconnect()
        .configureLogging(LogLevel.Trace)
        .build();

    connection.onclose((error) => {
        if (error) {
            post({
                messageType: RedditHubMessageType.Error,
                error,
            } as MessageError);
        }
    });

    connection.on('error', (error) => {
        post(JSON.stringify({
            messageType: RedditHubMessageType.Error,
            error,
        }));
    });

    /*
    connection.on('config', (configMessage: 'success' | 'error' | 'empty') => {
        // console.error(configMessage);
        //dispatch(redditHubSlice.actions.receiveRedditHubConfig(configMessage));
    });
    */

    try {
        await connection.start().then(() => {
            post(JSON.stringify({ messageType: RedditHubMessageType.Connected }));
        });
    } catch (reason) {
        console.error(reason);
        throw reason;
    }
};

const disconnect = async () => {
    if (connection == null) {
        return;
    }
    await connection.stop();
};

const subscribe = async (channel: RedditHubChannels) => {
    switch (channel) {
        case RedditHubChannels.NewPosts:
            subscribeNewPosts().then(() => {
                activeSubscriptions.add(channel);
            });
            break;
        default:
            return;
    }
};

const subscribeNewPosts = async () => {
    if (connection == null || connection.state != HubConnectionState.Connected) {
        throw new Error("Subscribe called while not Connected");
    }
    if (newPostReader != null) {
        return;
    }

    var newPostStream = connection.stream<IRedditApiPostData[]>('NewPosts');

    parseNewPostStack.start();

    // Read data from the stream
    newPostReader = newPostStream.subscribe({
        next: (message) => {
            newPostStack.push(message);
        },
        complete: () => {
            parseNewPostStack.stop();
            console.log('Stream completed.');
        },
        error: (err) => {
            console.error(err);
        },
    });
};

const unsubscribe = async (channel: RedditHubChannels) => {
    switch (channel) {
        case RedditHubChannels.NewPosts:
            parseNewPostStack.stop();
            unsubscribeNewPosts().then(() => {
                activeSubscriptions.delete(channel);
            });
            break;
        default:
            return;
    }
};

const unsubscribeNewPosts = async () => {
    if (newPostReader == null) {
        return;
    }
    newPostReader.dispose();
};

export default {};
