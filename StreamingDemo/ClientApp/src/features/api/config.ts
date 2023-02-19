import { Middleware } from 'redux'
import { RootState } from '../../store'

export const apiConfigMiddleWare: Middleware<
    {}, // do not modify the dispatch return value
    RootState
> = storeApi => next => action => {
    // const state = storeApi.getState();
    switch (action.type) {
        case 'CONFIG_STATUS':
            fetch('/api/config/status')
                .then((response) => response.json())
                .then((data) => {
                    //storeApi.dispatch({ type: 'SET_DATA', payload: data });
                    console.log(data);
                    if (data != null && data == "MissingToken") {
                        fetch('/api/config/AuthorizeUrl')
                            .then((response) => response.json())
                            .then((otherData) => {
                                //storeApi.dispatch({ type: 'SET_OTHER_DATA', payload: otherData });
                                console.log(otherData);
                            })
                            .catch((error) => {
                                console.error('Error fetching otherData:', error);
                            });
                    }
                })
                .catch((error) => {
                    console.error('Error fetching data:', error);
                });
            break;
        default:
            return next(action);
    }
}
