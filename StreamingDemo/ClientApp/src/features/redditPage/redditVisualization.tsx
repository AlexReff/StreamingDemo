import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { useSelector } from 'react-redux';
import { VictoryChart, VictoryVoronoiContainer, VictoryGroup, VictoryTooltip, VictoryLine, VictoryScatter, VictoryZoomContainer, VictoryAxis, VictoryBrushContainer, Tuple, DomainTuple, VictoryArea, VictoryStack, VictoryTheme, VictoryBar, VictoryLabel } from 'victory';
import { useAppSelector } from '../../hooks';
import { selectRedditInit, selectNumUpdates, selectPostsByTime, selectRedditPosts, selectLastUpdateDate, selectPostCountTimeData } from '../redditHub/redditHubSlice';
import { IRedditApiPostData } from '../redditHub/redditHubTypes';
import styles from './RedditVisualization.module.css';
import redditLogo from './reddit.svg';

interface RedditVisualizationProps {
    //
}

const statistics = [
    { name: 'meta', filterFn: (m: IRedditApiPostData) => m.is_meta },
    { name: 'self', filterFn: (m: IRedditApiPostData) => m.is_self },
    { name: 'video', filterFn: (m: IRedditApiPostData) => m.is_video },
    { name: 'over18', filterFn: (m: IRedditApiPostData) => m.over_18 },
    { name: 'spoiler', filterFn: (m: IRedditApiPostData) => m.spoiler }
];

export const RedditVisualization: React.FC<RedditVisualizationProps> = ({ }) => {
    const redditPosts = useSelector(selectRedditPosts);
    const redditInit = useSelector(selectRedditInit);
    const numUpdates = useSelector(selectNumUpdates);
    //const postCountsByTime = useSelector(selectPostCountTimeData);
    const postsByTime = useSelector(selectPostsByTime);
    const lastUpdateDate = useSelector(selectLastUpdateDate);
    // const redditTotals = useSelector(selectRedditTotals);
    // const redditStringTotals = useSelector(selectRedditStringTotals);

    // const [zoomDomain, setZoomDomain] = useState<{ x?: DomainTuple; y?: DomainTuple }>({ x: [0, 10], y: [0, 110] });
    // useEffect(() => {
    //     setZoomDomain({ x: [redditInit, lastUpdateDate] });
    //     // console.log('updated dates', lastUpdateDate);
    // }, [lastUpdateDate, redditInit]);
    //useState({ x: redditInit, y: new Date().getTime() });
    // const handleZoom = useCallback((domain: { x?: DomainTuple; y?: DomainTuple }) => setZoomDomain(domain), []);

    // const [sortedPostCountData, setSortedPostCountData] = useState<{
    //     received: number;
    //     records: IRedditApiPostData[];
    // }[]>([]);

    /*
    useEffect(() => {
        const result = [];
        const keys = Object.keys(postsByTime).map(m => +m).sort();
        for (const key of keys) {
            result.push({
                received: key,
                records: postsByTime[key],
            });
        }
        setSortedPostCountData(result);
        // console.log('updating posts', sorted);
    }, [postsByTime]);

    const getData = useCallback((data: {
        name: string;
        filterFn: (m: IRedditApiPostData) => boolean;
    }) => {
        const result = sortedPostCountData//Object.entries(sortedPostCountData)
            //.sort(([a, b]) => +a > +b ? 1 : +a == +b ? 0 : -1)
            .map((val, i) => {
                const adjustedTimestamp = Math.floor((val.received - redditInit) / 1000);
                return {
                    x: adjustedTimestamp,
                    y: val.records.filter(data.filterFn).length,
                };
            });
        console.log(result);
        return result;
    }, [sortedPostCountData, redditInit]);
    */

    // const getData = (data: any) => {
    //     return [

    //     ];
    // };

    const [chartData, setChartData] = React.useState<{ label: string, count: number }[]>([]);

    useEffect(() => {
        // Count the number of occurrences of each label
        const labelCounts: Record<string, number> = redditPosts.reduce(
            (acc, { record: { subreddit } }) => ({
                ...acc,
                [subreddit]: (acc[subreddit] ?? 0) + 1,
            }), {} as any);

        // Sort labels by count and take top 10
        const labels = Object.keys(labelCounts).sort(
            (a, b) => labelCounts[b] - labelCounts[a]
        ).slice(0, 10);

        // Prepare data for Victory chart
        const chartData = labels
            .map((label) => ({
                label,
                count: labelCounts[label],
            }))
            .sort((a, b) => a.count > b.count ? 1 : a.count == b.count ? 0 : -1)
            .reverse();


        setChartData(chartData);
    }, [redditPosts]);

    /*
    const mostPopularSubreddits = useMemo(() => {
        const groupedBySubreddit = redditPosts.reduce((acc, obj) => {
            const { subreddit } = obj.record;
            if (!acc[subreddit]) {
                acc[subreddit] = 1;
            } else {
                acc[subreddit]++;
            }
            return acc;
        }, {} as Record<string, number>);

        const result = Object.entries(groupedBySubreddit)
            .sort(([, a], [, b]) => a > b ? 1 : a == b ? 0 : -1)
            .reverse()
            .slice(0, 10)
            .map(([key, cnt]) => key);

        return result;
    }, [redditPosts]);
    */

    return (
        <div className={styles.container}>
            <div>
                <VictoryChart
                    width={1400}
                    height={900}
                    domainPadding={50}
                    animate={{ duration: 500 }}
                    style={{
                        background: {
                            fill: '#cecece',
                        },
                    }}
                >
                    <VictoryAxis dependentAxis
                        style={{
                            axis: {
                                stroke: 'white'
                            },
                            tickLabels: {
                                fill: 'white'
                            },
                        }} />
                    <VictoryAxis
                        tickValues={chartData.map(({ label }) => label)}
                        axisLabelComponent={<VictoryLabel dy={12} />}
                        label='subreddit'
                        style={{
                            axis: {
                                stroke: 'white'
                            },
                            tickLabels: {
                                fill: 'white'
                            },
                            axisLabel: {
                                padding: 20,
                                fill: 'white'
                            }
                        }} />
                    <VictoryBar data={chartData}
                        x='label'
                        y='count'
                        // x='subreddit'
                        // y={d => redditPosts.filter(item => item.record.subreddit === d.record.subreddit).length}
                        // maxDomain={{ x: 10 }}
                        sortOrder='descending'
                        sortKey={'count'}
                        width={30}
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