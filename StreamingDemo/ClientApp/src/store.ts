import { combineReducers, configureStore, ThunkAction, Action } from '@reduxjs/toolkit';
import counterReducer from './features/counter/counterSlice';
import redditHubReducer, { connectRedditHub } from './features/redditHub/redditHubSlice';

const rootReducer = combineReducers({
    counter: counterReducer,
    redditHub: redditHubReducer,
});

export const store = configureStore({
    reducer: rootReducer,
    // middleware: (getDefaultMiddleware) =>
    //     getDefaultMiddleware()
    //         .concat(redditHubMiddleWare)
            // .concat(apiConfigMiddleWare)
});

store.dispatch(connectRedditHub("/hub"));

export type AppDispatch = typeof store.dispatch;
export type RootState = ReturnType<typeof rootReducer>;
export type AppThunk<ReturnType = void> = ThunkAction<
    ReturnType,
    RootState,
    unknown,
    Action<string>
>;
