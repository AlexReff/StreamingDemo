import { RouteComponentProps, Redirect } from "@reach/router";
import React from "react";

interface NoMatchProps {
    //
}

export const NoMatch: React.FC<NoMatchProps & RouteComponentProps> = ({ }) => {
    return (
        <Redirect to="/" />
    );
};
