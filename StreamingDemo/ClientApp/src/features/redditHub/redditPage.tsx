import React from "react";
import { RedditVisualization } from "./redditVisualization";
import styles from './RedditHub.module.css';
import redditLogo from "./reddit.svg";
import { RedditSidebar } from "./redditSidebar";

interface RedditPageProps {
    //
}

export const RedditPage: React.FC<RedditPageProps> = ({ }) => {
    return (
        <div className={styles.container}>
            <div className={styles.wrapper}>
                <div className={styles.header}>
                    <img width={60} height={60} src={redditLogo} />
                    SignalR Visualization Demo
                </div>
                <div className={styles.belowHeader}>
                    <div className={styles.sidebar}>
                        <RedditSidebar />
                    </div>
                    <div className={styles.content}>
                        <div>
                            <RedditVisualization />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};
