import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { VictoryChart, VictoryVoronoiContainer, VictoryGroup, VictoryTooltip, VictoryLine, VictoryScatter, VictoryZoomContainer, VictoryAxis, VictoryBrushContainer, Tuple, DomainTuple, VictoryArea, VictoryStack, VictoryTheme, VictoryBar, VictoryLabel } from 'victory';
import { selectNewPostStats, selectRedditInit, selectStatus, subscribeToNewPosts, unsubscribeToNewPosts } from '../redditHub/redditHubSlice';
import { useAppDispatch, useAppSelector } from '../../hooks';
import { RedditPageState } from './redditPageSlice';
import styles from './RedditVisualization.module.css';
import redditLogo from './reddit.svg';

interface RedditVisualizationProps {
    pageState: RedditPageState;
}

export const RedditVisualization: React.FC<RedditVisualizationProps> = ({ pageState }) => {
    const dispatch = useAppDispatch();
    const apiStatus = useAppSelector(selectStatus);

    useEffect(() => {
        if (apiStatus != 'idle') {
            return;
        }

        if (!document.hidden) {
            dispatch(subscribeToNewPosts());
        }

        const visibilityChangeHandler = (ev: Event) => {
            if (document.hidden) {
                dispatch(unsubscribeToNewPosts());
            } else {
                dispatch(subscribeToNewPosts());
            }
        };

        document.addEventListener("visibilitychange", visibilityChangeHandler);

        return () => {
            document.removeEventListener("visibilitychange", visibilityChangeHandler);
            dispatch(unsubscribeToNewPosts());
        };
    }, [apiStatus]);

    const newPostStats = useSelector(selectNewPostStats);

    const [chartData, setChartData] = React.useState<{ label: string, count: number }[]>([]);

    useEffect(() => {
        let mappedStats = Object.entries(newPostStats.subredditCounts).map(([label, count]) => ({ label, count }));
        mappedStats.sort((a, b) => b.count - a.count);
        setChartData(mappedStats.slice(0, 10));
    }, [newPostStats]);

    return (
        <div className={styles.container}>
            <div>
                <VictoryChart
                    width={1400}
                    height={900}
                    domainPadding={50}
                    // animate={{ duration: 500 }}
                    style={{
                        background: {
                            //fill: '#DEDEDE',
                            fill: 'transparent',
                        },
                    }}
                >
                    <VictoryAxis dependentAxis
                        style={{
                            // axis: {
                            //     stroke: 'white'
                            // },
                            // tickLabels: {
                            //     fill: 'white'
                            // },
                        }} />
                    <VictoryAxis
                        tickValues={chartData.map(({ label }) => label)}
                        axisLabelComponent={<VictoryLabel dy={15} />}
                        tickLabelComponent={<VictoryLabel angle={15} />}
                        // label='subreddit'
                        style={{
                            axis: {
                                // stroke: 'white'
                            },
                            tickLabels: {
                                // fill: 'white',
                                fontSize: '12px',
                            },
                            axisLabel: {
                                padding: 20,
                                // fill: 'white'
                            }
                        }} />
                    <VictoryBar data={chartData}
                        x='label'
                        y='count'
                        // sortOrder='descending'
                        // sortKey='count'
                        width={30}
                        // key='label'
                        // style={{
                        //     labels: {
                        //         color: "#0F0",
                        //     },
                        // }}
                        //labels={({ datum }) => datum.count}
                        //labelComponent={<VictoryLabel  />}
                    />
                </VictoryChart>
            </div>
            <div>
                {/* 
                <VictoryChart
                    //theme={VictoryTheme.material}
                    animate={{ duration: 1000 }}
                    domain={{ x: [0, 100], y: [0, 40] }}
                >
                    <VictoryStack
                        colorScale={'blue'}
                    >
                    </VictoryStack>
                </VictoryChart>
                 */}
                {/* {statistics.map((data, i) => { //Object.entries(postsByTime).map((data, i) => {
                            return (
                                <VictoryArea
                                    key={i}
                                    data={getData(data)}
                                    interpolation={'basis'}
                                    labels={[data.name, 'test']}
                                />
                            );
                        })} */}
                {/*                 
                <VictoryChart width={600} height={470}
                    scale={{ x: 'time', y: 'linear' }}
                    domainPadding={10}
                    containerComponent={
                        <VictoryZoomContainer
                            zoomDimension='x'
                            zoomDomain={zoomDomain}
                            onZoomDomainChange={handleZoom}
                        />
                    }
                >
                    <VictoryAxis
                        tickFormat={(x) => +((x - redditInit) / 1000).toFixed(1)}
                        style={{
                            axis: {
                                stroke: 'white'  //CHANGE COLOR OF X-AXIS
                            },
                            tickLabels: {
                                fill: 'white' //CHANGE COLOR OF X-AXIS LABELS
                            },
                            grid: {
                                stroke: 'white', //CHANGE COLOR OF X-AXIS GRID LINES
                                strokeDasharray: '7',
                            }
                        }}
                    />
                    <VictoryLine
                        style={{
                            data: { stroke: 'tomato' }
                        }}
                        data={postCountData}
                        x='timestamp'
                        y='postCount'
                    />
                </VictoryChart>
                <VictoryChart
                    padding={{ top: 0, left: 50, right: 50, bottom: 30 }}
                    width={600} height={100}
                    scale={{ x: 'time' }}
                    domainPadding={110}
                    containerComponent={
                        <VictoryBrushContainer
                            brushDimension='x'
                            brushDomain={zoomDomain}
                            onBrushDomainChange={handleZoom}
                        />
                    }
                >
                    <VictoryAxis
                        tickFormat={(x) => +((x - redditInit) / 1000).toFixed(1)}
                        style={{
                            axis: {
                                stroke: 'white'  //CHANGE COLOR OF X-AXIS
                            },
                            tickLabels: {
                                fill: 'white' //CHANGE COLOR OF X-AXIS LABELS
                            },
                            grid: {
                                stroke: 'white', //CHANGE COLOR OF X-AXIS GRID LINES
                                strokeDasharray: '7',
                            }
                        }}
                    />
                    <VictoryLine
                        style={{
                            data: { stroke: 'tomato' }
                        }}
                        data={postCountData}
                        x='timestamp'
                        y='postCount'
                    />
                </VictoryChart> */}
                {/*
                <VictoryChart
                    containerComponent={<VictoryVoronoiContainer />}
                >
                    <VictoryGroup
                        color='#c43a31'
                        labels={({ datum }) => `y: ${datum.y}`}
                        labelComponent={
                            <VictoryTooltip
                                style={{ fontSize: 10 }}
                            />
                        }
                        //data={postData.map(m => { x: m.})}
                        data={[
                            { x: 1, y: -3 },
                            { x: 2, y: 5 },
                            { x: 3, y: 3 },
                            { x: 4, y: 0 },
                            { x: 5, y: -2 },
                            { x: 6, y: -2 },
                            { x: 7, y: 5 }
                        ]}
                    >
                        <VictoryLine />
                        <VictoryScatter
                            size={({ active }) => active ? 5 : 3}
                        />
                    </VictoryGroup>
                    <VictoryGroup
                        labels={({ datum }) => `y: ${datum.y}`}
                        labelComponent={
                            <VictoryTooltip
                                style={{ fontSize: 10 }}
                            />
                        }
                        data={[
                            { x: 1, y: 3 },
                            { x: 2, y: 1 },
                            { x: 3, y: 2 },
                            { x: 4, y: -2 },
                            { x: 5, y: -1 },
                            { x: 6, y: 2 },
                            { x: 7, y: 3 }
                        ]}
                    >
                        <VictoryLine />
                        <VictoryScatter
                            size={({ active }) => active ? 5 : 3}
                        />
                    </VictoryGroup>
                </VictoryChart>
                    */ }
            </div>
        </div>
    );
};


/*
for (const record of action.payload) {
    //parse fieldTotals
    for (const fieldName of RedditApiNumbFields) {
        if (fieldName in record
            && record[fieldName] != null
            && !isNaN(record[fieldName])) {
            state.fieldTotals[fieldName] += +record[fieldName];
        }
    }
    for (const fieldName of RedditApiBoolFields) {
        if (fieldName in record
            && record[fieldName] != null
            && record[fieldName] === true) {
            state.fieldTotals[fieldName] += 1;
        }
    }
    //parse stringFieldTotals - instances of specific strings (subreddit, author, etc)
    for (const fieldName of RedditApiTextFields) {
        if (fieldName in record
            && record[fieldName] != null
            && record[fieldName].length > 0) {
            if (!(record[fieldName] in state.stringFieldTotals[fieldName])) {
                state.stringFieldTotals[fieldName] = {
                    ...state.stringFieldTotals[fieldName],
                    [record[fieldName]]: 0,
                };
            }
            state.stringFieldTotals[fieldName][record[fieldName]]++;
        }
    }
}
*/