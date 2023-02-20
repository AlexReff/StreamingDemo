import { createAsyncThunk, createSlice, PayloadAction } from '@reduxjs/toolkit';
import { IRedditApiPostData, RedditApiBoolFields, RedditApiNumbFields, RedditApiTextFields } from '../redditHub/redditHubTypes';

export interface RedditPageState {
    // which fields to render
    selectedFields: (keyof IRedditApiPostData)[];
    // whether the graph should continue to update as new information streams in
    liveUpdate: boolean;
}

export const initialState: RedditPageState = {
    selectedFields: [
        ...RedditApiNumbFields,//.reduce((obj, key) => ({ ...obj, [key]: 0 }), {}),
        ...RedditApiBoolFields,//.reduce((obj, key) => ({ ...obj, [key]: 0 }), {}),
        ...RedditApiTextFields,//.reduce((obj, key) => ({ ...obj, [key]: 0 }), {}),
    ],
    liveUpdate: true,
};

export const redditPageSlice = createSlice({
    name: 'redditPage',
    initialState,
    // The `reducers` field lets us define reducers and generate associated actions
    reducers: {
        // Redux Toolkit allows us to write "mutating" logic in reducers. It
        // doesn't actually mutate the state because it uses the Immer library,
        // which detects changes to a "draft state" and produces a brand new
        // immutable state based off those changes
        toggleLiveUpdate(state) {
            state.liveUpdate = !state.liveUpdate;
        },
        addSelectedField(state, action: PayloadAction<keyof IRedditApiPostData>) {
            state.selectedFields.push(action.payload);
        },
        removeSelectedField(state, action: PayloadAction<keyof IRedditApiPostData>) {
            const idx = state.selectedFields.indexOf(action.payload);
            if (idx !== -1) {
                state.selectedFields.splice(idx, 1);
            }
        },
    },
    // The `extraReducers` field lets the slice handle actions defined elsewhere,
    // including actions generated by createAsyncThunk or in other slices.
    // extraReducers: (builder) => builder
    //     .addCase(receiveRedditHubData, (state, action) => {
    //         //
    //     })
    // .addCase(connectRedditPage.fulfilled, (state) => {
    //     //state.connection = connection;
    // })
    // .addCase(connectRedditPage.rejected, (state, action) => {
    //     // state.connection = null;
    //     // state.error = action.payload as string;
    // })
});

// export const { redditPageDisconnect, receiveRedditPageConfig } = redditPageSlice.actions;

// export const selectStatus = (state: RootState) => state.redditPage.status;
// export const selectData = (state: RootState) => state.redditPage.data;

export default redditPageSlice.reducer;