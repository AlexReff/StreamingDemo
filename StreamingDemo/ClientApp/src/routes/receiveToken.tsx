import { RouteComponentProps, useLocation } from "@reach/router";
import React, { useEffect } from "react";
import { redditHubConnection } from "../features/redditHub/redditHubMiddleware";

interface ReceiveTokenProps {
    //
}

export const ReceiveToken: React.FC<ReceiveTokenProps & RouteComponentProps> = ({ }) => {
    const loc = useLocation();
    useEffect(() => {
        const code = loc.search.substring(1).split('code=')[1].split('&')[0];
        if (code != null && code.length > 0) {
            redditHubConnection.invoke("CodeReceived", code);

            //redditHubConnection.on("")
        }
    }, []);
    return (
        <div>
            Retrieving your token...
        </div>
    );
};
