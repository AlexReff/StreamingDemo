import { HubConnection, HubConnectionBuilder, IStreamResult, LogLevel } from '@microsoft/signalr';
import { createAsyncThunk, createSelector, createSlice, PayloadAction } from '@reduxjs/toolkit';
import { toast } from 'react-toastify';
import { RootState } from '../../store';
import { MessageDataNewPosts, RedditHubChannels, RedditHubMessage, RedditHubMessageType } from './redditHub.worker.types';
import { IRedditApiPostData } from './redditHubTypes';

const hubWorker = new Worker(new URL('./redditHub.worker.ts', import.meta.url));

export enum RedditHubStatus {
    idle,
    loading,
    failed,
    invalid,
    disconnected,
};

export interface RedditHubState {
    newPostStats: {
        totalCount: number;
        subredditCounts: Record<string, number>;
    },
    lastUpdate: number,
    initDate: number,
    status: RedditHubStatus;
}

const initNow = new Date();

const initialState: RedditHubState = {
    newPostStats: {
        totalCount: 0,
        subredditCounts: {},
    },
    lastUpdate: 0,
    initDate: initNow.getTime(),
    status: RedditHubStatus.disconnected,
};

let connection: HubConnection | null = null;

// The function below is called a thunk and allows us to perform async logic. It
// can be dispatched like a regular action: `dispatch(incrementAsync(10))`. This
// will call the thunk with the `dispatch` function as the first argument. Async
// code can then be executed and other actions can be dispatched. Thunks are
// typically used to make async requests.
export const connectRedditHub = createAsyncThunk<void, string>(
    'redditHub/connectAction',
    async (url, { dispatch }) => {
        hubWorker.onmessage = (messageString: MessageEvent<string>) => {
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
                case RedditHubMessageType.Connected:
                    dispatch(setConnected(true));
                    break;
                case RedditHubMessageType.Data:
                    switch ((message as MessageDataNewPosts).dataType) {
                        case RedditHubChannels.NewPosts:
                            dispatch(receiveNewPosts((message as MessageDataNewPosts).data));
                            break;
                        default:
                            return;
                    }
                    break;
                default:
                    return;
            }
        };

        hubWorker.postMessage(JSON.stringify({ messageType: RedditHubMessageType.Connect, url }));
    }
);

export const redditHubSlice = createSlice({
    name: 'redditHub',
    initialState,
    // The `reducers` field lets us define reducers and generate associated actions
    reducers: {
        // Redux Toolkit allows us to write "mutating" logic in reducers. It
        // doesn't actually mutate the state because it uses the Immer library,
        // which detects changes to a "draft state" and produces a brand new
        // immutable state based off those changes
        redditHubDisconnect: (state) => {
            if (state.status != RedditHubStatus.idle || connection == null) {
                return;
            }

            hubWorker.postMessage(JSON.stringify({ messageType: RedditHubMessageType.Disconnect }));

            setConnected(false);
        },
        setConnected: (state, action: PayloadAction<boolean>) => {
            if (action.payload) {
                state.status = RedditHubStatus.idle;
                // toast("Connected", {
                //     toastId: "connect"
                // });
            } else {
                state.status = RedditHubStatus.disconnected;
                // toast("Disconnected", {
                //     toastId: "disconnect"
                // });
            }
        },
        receiveNewPosts: (state, action: PayloadAction<IRedditApiPostData[]>) => {
            if (action.payload == null || action.payload.length == 0) {
                return;
            }

            state.lastUpdate = new Date().getTime();

            state.newPostStats.totalCount += action.payload.length;

            for (const post of action.payload) {
                state.newPostStats.subredditCounts[post.subreddit] = (state.newPostStats.subredditCounts[post.subreddit] ?? 0) + 1;
            }
        },
        receiveRedditHubConfig: (state, action: PayloadAction<'success' | 'error' | 'empty'>) => {
            switch (action.payload) {
                case 'empty':
                    state.status = RedditHubStatus.invalid;
                    break;
                case 'success':
                    state.status = RedditHubStatus.idle;
                    break;
                case 'error':
                    state.status = RedditHubStatus.failed;
                    break;
                default:
                    break;
            }
        },
        setRedditHubError: (state, action: PayloadAction<string>) => {
            // state.isConnected = false;
            // state.error = action.payload;
        },
        subscribeToNewPosts: (state) => {
            hubWorker.postMessage(JSON.stringify({
                messageType: RedditHubMessageType.Subscribe,
                channel: RedditHubChannels.NewPosts,
            }));
        },
        unsubscribeToNewPosts: (state) => {
            hubWorker.postMessage(JSON.stringify({
                messageType: RedditHubMessageType.Subscribe,
                channel: RedditHubChannels.NewPosts,
            }));
        },
    },
    // The `extraReducers` field lets the slice handle actions defined elsewhere,
    // including actions generated by createAsyncThunk or in other slices.
    extraReducers: (builder) => builder
    // .addCase(connectRedditHub.fulfilled, (state) => {
    //     //state.connection = connection;
    // })
    // .addCase(connectRedditHub.rejected, (state, action) => {
    //     // state.connection = null;
    //     // state.error = action.payload as string;
    // }),
});

export const { redditHubDisconnect, setConnected, receiveNewPosts, subscribeToNewPosts, unsubscribeToNewPosts } = redditHubSlice.actions;

export const selectStatus = (state: RootState) => state.redditHub.status;
export const selectRedditInit = (state: RootState) => state.redditHub.initDate;
export const selectLastUpdate = (state: RootState) => state.redditHub.lastUpdate;
export const selectNewPostStats = (state: RootState) => state.redditHub.newPostStats;

export default redditHubSlice.reducer;
