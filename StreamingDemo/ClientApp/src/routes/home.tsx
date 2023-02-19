import { RouteComponentProps } from "@reach/router";
import React, { useEffect } from "react";
import { Counter } from "../features/counter/Counter";
import logo from '../logo.svg';

interface HomeProps {
    //
}

export const Home: React.FC<HomeProps & RouteComponentProps> = ({ }) => {
    return (
        <div>
            <img src={logo} className="App-logo" alt="logo" />
            <Counter />
        </div>
    );
};
