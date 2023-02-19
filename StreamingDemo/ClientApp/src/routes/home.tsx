import { RouteComponentProps } from "@reach/router";
import React, { useEffect } from "react";
import { Counter } from "../features/counter/Counter";
import { redditHubDefaultConnectAction } from "../features/redditHub/redditHubMiddleware";
import logo from '../logo.svg';
import { store } from "../store";

interface HomeProps {
    //
}

export const Home: React.FC<HomeProps & RouteComponentProps> = ({ }) => {
    useEffect(() => {
        store.dispatch(redditHubDefaultConnectAction);
        console.log("Dispatched Hub Connection");
    }, []);
    return (
        <div>
            <img src={logo} className="App-logo" alt="logo" />
            <Counter />
        </div>
    );
};
