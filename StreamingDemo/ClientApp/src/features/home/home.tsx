import { RouteComponentProps } from '@reach/router';
import React, { useCallback, useEffect, useLayoutEffect, useRef, useState } from 'react';
import { ColorRing } from 'react-loader-spinner';
import { selectStatus } from '../redditHub/redditHubSlice';
import { RedditPage } from '../redditHub/redditPage';
import { useAppSelector } from '../../hooks';
import styles from './Home.module.css';
import classNames from 'classnames';
import { useTransition } from 'transition-hook';

interface HomeProps {
    //
}

export const Home: React.FC<HomeProps & RouteComponentProps> = ({ }) => {
    const status = useAppSelector(selectStatus);
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
        </div >
    );
};