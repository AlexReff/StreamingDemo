import { RouteComponentProps } from '@reach/router';
import React, { useCallback, useEffect, useLayoutEffect, useRef, useState } from 'react';
import { ColorRing } from 'react-loader-spinner';
import { connectRedditHub, redditHubDisconnect, selectStatus } from '../redditHub/redditHubSlice';
import { RedditPage } from '../redditPage/redditPage';
import styles from './Home.module.css';
import classNames from 'classnames';
import { useTransition } from 'transition-hook';
import { useSelector } from 'react-redux';

interface HomeProps {
    //
}

// connects the app to SignalR and displays the visualization page
export const Home: React.FC<HomeProps & RouteComponentProps> = ({ }) => {
    const status = useSelector(selectStatus);

    //transition animation hooks
    const { stage: stageSpinner, shouldMount: shouldMountSpinner } = useTransition(status == 'disconnected', 1000);
    const { stage: stagePage, shouldMount: shouldMountPage } = useTransition(status == 'idle', 1000);

    return (
        <div>
            {shouldMountSpinner && (
                <div className={styles.loadingSpinnerWrapper}>
                    <ColorRing
                        visible={true}
                        height={64}
                        width={64}
                        ariaLabel='blocks-loading'
                        wrapperStyle={{}}
                        wrapperClass={classNames({
                            [styles.loadingSpinner]: true,
                            [styles.loadingSpinnerEnter]: stageSpinner == 'enter',
                        })}
                        colors={['#D61355', '#F94A29', '#FCE22A', '#30E3DF', '#301E67']}
                    />
                </div>
            )}
            {shouldMountPage && (
                <div className={classNames({
                    [styles.redditPageWrapper]: true,
                    [styles.redditPageWrapperEnter]: stagePage == 'enter',
                })}>
                    <RedditPage />
                </div>
            )}
        </div>
    );
};
