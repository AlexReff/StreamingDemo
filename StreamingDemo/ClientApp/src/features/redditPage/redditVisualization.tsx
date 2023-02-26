import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { RedditHubStatus, selectNewPostStats, selectRedditInit, selectStatus, subscribeToNewPosts, unsubscribeToNewPosts } from '../redditHub/redditHubSlice';
import { useAppDispatch, useAppSelector } from '../../hooks';
import { RedditPageState } from './redditPageSlice';
import styles from './RedditVisualization.module.css';
import redditLogo from './reddit.svg';
import { CartesianGrid, Legend, Line, LineChart, ResponsiveContainer, Tooltip, XAxis, YAxis } from 'recharts';
import { getForegroundColor } from '../utillity';

interface RedditVisualizationProps {
    pageState: RedditPageState;
}

export const RedditVisualization: React.FC<RedditVisualizationProps> = ({ pageState }) => {
    const appDispatch = useAppDispatch();
    const apiStatus = useAppSelector(selectStatus);
    const newPostStats = useAppSelector(selectNewPostStats);

    useEffect(() => {
        if (apiStatus != RedditHubStatus.idle) {
            return;
        }

        if (!document.hidden) {
            appDispatch(subscribeToNewPosts());
        }

        const visibilityChangeHandler = (ev: Event) => {
            if (document.hidden) {
                appDispatch(unsubscribeToNewPosts());
            } else {
                appDispatch(subscribeToNewPosts());
            }
        };

        document.addEventListener("visibilitychange", visibilityChangeHandler);

        return () => {
            document.removeEventListener("visibilitychange", visibilityChangeHandler);
            appDispatch(unsubscribeToNewPosts());
        };
    }, [apiStatus]);

    const [chartData, setChartData] = React.useState<{ _timestamp: number, [key: string]: number }[]>([]);

    useEffect(() => {
        const subStats = Object.entries(newPostStats.subredditCounts);
        if (subStats.length == 0) {
            return;
        }
        setChartData((data) => {
            return [
                ...data,
                {
                    _timestamp: new Date().getTime(),
                    ...Object.fromEntries(subStats),
                },
            ];
        });
    }, [newPostStats]);

    const topSubreddits = useMemo<string[]>(() => {
        return Object.entries(newPostStats.subredditCounts)
            .sort((a, b) => b[1] - a[1])
            .slice(0, 10)
            .map((val) => val[0]);
    }, [newPostStats]);

    return (
        <div className={styles.container}>
            <div>
                {chartData.length > 1 && (
                    <ResponsiveContainer width="100%" height={700}>
                        <LineChart width={730} height={250} data={chartData}
                            margin={{ top: 5, right: 30, left: 20, bottom: 5 }}>
                            <CartesianGrid strokeDasharray="3 3" />
                            <XAxis
                                dataKey="_timestamp"
                                scale={"utc"}
                                type="number"
                                domain={[() => chartData[0]._timestamp, 'dataMax']}
                                tickFormatter={(val) => new Date(val).toLocaleTimeString()}
                            />
                            <YAxis />
                            <Tooltip />
                            <Legend />
                            {topSubreddits.map((val, idx) => (
                                <Line key={val} type="monotone" dataKey={val} stroke={getForegroundColor(val, getComputedStyle(document.body).backgroundColor)} />
                            ))}
                        </LineChart>
                    </ResponsiveContainer>
                )}
            </div>
        </div>
    );
};
