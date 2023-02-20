import React, { useMemo, useReducer } from "react";
import { RedditVisualization } from "./redditVisualization";
import redditLogo from "./reddit.svg";
import { RedditSidebar } from "./redditSidebar";
import redditPageSlice, { initialState } from "./redditPageSlice";
import styles from './RedditPage.module.css';

interface RedditPageProps {
    //
}

export const RedditPage: React.FC<RedditPageProps> = ({ }) => {
    const [reducerState, dispatch] = useReducer(redditPageSlice, initialState);
    const now = useMemo(() => new Date(), []);
    return (
        <div className={styles.container}>
            <div className={styles.wrapper}>
                <div className={styles.header}>
                    <div className={styles.headerLeft}>
                        <div className={styles.animatedBgWrapper}>
                            <img className={styles.headerImage} width={60} height={60} src={redditLogo} />
                            <div className={styles.animatedBgElementMask}>
                                <div className={styles.animatedBgElements}>
                                    <div className={styles.animatedBg} />
                                    <div className={styles.animatedBg} />
                                    <div className={styles.animatedBg} />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div className={styles.headerRight}>
                        <h1 className={styles.headerText}>SignalR Visualization Demo</h1>
                    </div>
                </div>
                <div className={styles.belowHeader}>
                    {/* 
                    <div className={styles.sidebar}>
                        <RedditSidebar />
                    </div>
                     */}
                    <div className={styles.content}>
                        <div>
                            <RedditVisualization />
                        </div>
                        <div>
                            <h1>Streaming new posts, grouped by subreddit</h1>
                            <p>See README for known issues</p>
                            <p>Work in progress</p>
                        </div>
                    </div>
                </div>
                <div className={styles.footer}>
                    <div>DEVELOPMENT</div>
                    <div>Creative Commons Attribution 4.0 International Public License</div>
                    <div>alex@reff.dev</div>
                </div>
            </div>
        </div>
    );
};
