import { combineReducers, configureStore, ThunkAction, Action } from '@reduxjs/toolkit';
import counterReducer from './features/counter/counterSlice';
import { redditHubDefaultConnectAction, redditHubMiddleWare } from './features/redditHub/redditHubMiddleware';

const rootReducer = combineReducers({
    counter: counterReducer,
});

export const store = configureStore({
    reducer: rootReducer,
    middleware: (getDefaultMiddleware) =>
        getDefaultMiddleware()
            .concat(redditHubMiddleWare)
            // .concat(apiConfigMiddleWare)
});

// store.dispatch({ type: 'CONFIG_STATUS' });
// store.dispatch(redditHubDefaultConnectAction);

export type AppDispatch = typeof store.dispatch;
export type RootState = ReturnType<typeof rootReducer>;
export type AppThunk<ReturnType = void> = ThunkAction<
    ReturnType,
    RootState,
    unknown,
    Action<string>
>;
