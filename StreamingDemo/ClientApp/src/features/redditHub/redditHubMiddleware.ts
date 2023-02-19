import { Middleware } from 'redux'
import { RootState } from '../../store'
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';

export let redditHubConnection: HubConnection;

export const redditHubDefaultConnectAction = {
    type: 'SIGNALR_CONNECT',
    payload: {
        url: '/hub',
        // eventName: 'newPosts',
        connectType: 'SIGNALR_CONNECT_SUCCESS',
        successType: 'NEW_POSTS_RECEIVED',
        errorType: 'NEW_POSTS_ERROR',
    },
};

export const redditHubMiddleWare: Middleware<
    {}, // do not modify the dispatch return value
    RootState
> = storeApi => next => action => {
    // const state = storeApi.getState();
    switch (action.type) {
        case 'SIGNALR_CONNECT':
            // Create a SignalR hub connection
            redditHubConnection = new HubConnectionBuilder()
                .withUrl(action.payload.url)
                .withAutomaticReconnect()
                .configureLogging(LogLevel.Trace)
                .build();

            redditHubConnection.on('tokenUrl', (data) => {
                //received a tokenUrl to navigate to
                window.location = data;
                // Dispatch a Redux action with the received data
                // next({ type: action.payload.successType, payload: data });
            });
            
            redditHubConnection.on('data', (data) => {
                //received a tokenUrl to navigate to
                console.log(data);
                // Dispatch a Redux action with the received data
                // next({ type: action.payload.successType, payload: data });
            });

            // Attach a handler to the 'error' event
            redditHubConnection.on('error', (errorMessage) => {
                console.error(errorMessage);
                // Dispatch a Redux action with the error message
                // next({ type: action.payload.errorType, payload: errorMessage });
            });

            // Attach a handler to the 'error' event
            redditHubConnection.on('config', (configMessage: 'success' | 'error' | 'empty') => {
                console.error(configMessage);
                switch (configMessage){
                    case 'empty':
                        //should receive 'tokenUrl' and redirect to that
                        break;
                    case 'success':
                        //
                        break;
                    case 'error':
                    default:
                        break;
                }
                // Dispatch a Redux action with the error message
                // next({ type: action.payload.errorType, payload: errorMessage });
            });

            // Start the SignalR hub connection
            redditHubConnection.start().catch((reason) => {
                next({ type: action.payload.errorType, payload: reason });
            }).then(() => {
                redditHubConnection.invoke('Initialize');
                // Dispatch a Redux action to indicate that the connection was successful
                next({ type: action.payload.connectType });
            });
            break;

        case 'SIGNALR_DISCONNECT':
            // Cleanup the SignalR hub connection
            redditHubConnection.stop();
            break;

        case 'CONFIG_TOKEN':
            break;

        default:
            return next(action);
    }
}
