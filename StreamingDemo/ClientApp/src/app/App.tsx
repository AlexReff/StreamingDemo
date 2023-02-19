import React, { useEffect } from 'react';
import { Router, Link } from "@reach/router"
import { Layout } from './Layout';
import { NoMatch } from '../routes/noMatch';
import { Home } from '../routes/home';
import { ReceiveToken } from '../routes/receiveToken';

import './App.css';

function App() {
    return (
        <Layout>
            <Router>
                <Home path="/" />
                <ReceiveToken path="/token" />
                <NoMatch default />
            </Router>
        </Layout>
    );
}

export default App;
