import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { createAsyncThunk, createSelector, createSlice, PayloadAction } from '@reduxjs/toolkit';
import { toast } from 'react-toastify';
import { RootState } from '../../store';
import { IRedditApiPostData } from './redditHubTypes';

export interface RedditHubState {
    data: {
        record: IRedditApiPostData,
        receivedDate: number,
    }[];
    initDate: number,
    status: 'idle' | 'loading' | 'failed' | 'invalid' | 'disconnected';
}

const initNow = new Date();

const initialState: RedditHubState = {
    data: [],
    initDate: initNow.getTime(),
    status: 'disconnected',
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
        console.log("CONNECTING TO HUB");
        connection = new HubConnectionBuilder()
            .withUrl(url)
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Trace)
            .build();

        connection.on('data', (data) => {
            // console.log(data);
            // Dispatch a Redux action with the received data
            dispatch(redditHubSlice.actions.receiveRedditHubData(data));
        });

        connection.on('error', (errorMessage) => {
            console.error(errorMessage);
            //dispatch(setRedditHubError(errorMessage));
        });

        connection.on('config', (configMessage: 'success' | 'error' | 'empty') => {
            // console.error(configMessage);
            dispatch(redditHubSlice.actions.receiveRedditHubConfig(configMessage));
        });

        try {
            await connection.start();

            toast("Connected", {
                toastId: "Connected"
            });

            //TODO: identify why this fails, finish front end error notification
            //connection.invoke('GetStatus');

            // Dispatch a Redux action to indicate that the connection was successful
            dispatch(redditHubSlice.actions.setRedditHubConnected());


        } catch (reason) {
            console.error(reason);
            // dispatch(setRedditHubError(reason.toString()));
        }
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
            if (state.status != 'idle' || connection == null) {
                return;
            }

            state.status = 'disconnected';

            const stopRequest = connection.stop();
            stopRequest.finally(() => {
                connection = null;
                toast("Disconnected", {
                    toastId: "disconnect"
                });
            });
        },
        receiveRedditHubData: (state, action: PayloadAction<IRedditApiPostData[]>) => {
            //state.data = state.data.concat(action.payload);

            if (action == null || action.payload == null || action.payload.length === 0) {
                return;
            }

            const localNow = new Date();

            const datedElements = action.payload.map((record) => ({
                receivedDate: localNow.getTime(),
                record,
            }));

            state.data.push(...datedElements);
            state.status = 'idle';
        },
        receiveRedditHubConfig: (state, action: PayloadAction<'success' | 'error' | 'empty'>) => {
            switch (action.payload) {
                case 'empty':
                    state.status = 'invalid';
                    break;
                case 'success':
                    state.status = 'idle';
                    break;
                case 'error':
                    state.status = 'failed';
                    break;
                default:
                    break;
            }
        },
        setRedditHubConnected(state) {
            state.status = 'idle';
        },
        setRedditHubError(state, action: PayloadAction<string>) {
            // state.isConnected = false;
            // state.error = action.payload;
        },
    },
    // The `extraReducers` field lets the slice handle actions defined elsewhere,
    // including actions generated by createAsyncThunk or in other slices.
    extraReducers: (builder) => builder
        .addCase(connectRedditHub.fulfilled, (state) => {
            //state.connection = connection;
        })
        .addCase(connectRedditHub.rejected, (state, action) => {
            // state.connection = null;
            // state.error = action.payload as string;
        }),
});

export const { redditHubDisconnect, receiveRedditHubConfig, receiveRedditHubData } = redditHubSlice.actions;

export const selectStatus = (state: RootState) => state.redditHub.status;
export const selectRedditPosts = (state: RootState) => state.redditHub.data;
export const selectRedditInit = (state: RootState) => state.redditHub.initDate;

export const selectPostsByTime = createSelector(
    selectRedditPosts,
    (records) => records.reduce((acc, { record, receivedDate }) => {
        const posts = acc[receivedDate] || [];
        posts.push(record);
        acc[receivedDate] = posts;
        return acc;
    }, {} as Record<number, IRedditApiPostData[]>)
);

export const selectPostCountTimeData = createSelector(
    selectPostsByTime,
    (records) => Object.entries(records).map(([timestamp, posts]) => ({
        timestamp: Number(timestamp),
        postCount: posts.length,
    }))
);

export const selectLastUpdateDate = createSelector(
    selectPostsByTime,
    (records) => Math.max(...Object.keys(records).map((val) => +val))
);

export const selectNumUpdates = createSelector(
    selectPostsByTime,
    (records) => Object.keys(records).length
);

export default redditHubSlice.reducer;
