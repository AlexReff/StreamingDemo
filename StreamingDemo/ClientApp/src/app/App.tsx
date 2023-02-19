import React, { useEffect } from 'react';
import { Router, Link } from "@reach/router"
import { Layout } from './Layout';
import { NoMatch } from '../routes/noMatch';
import { Home } from '../features/home/home';

import './App.css';

function App() {
    return (
        <Layout>
            <Router>
                <Home path="/" />
                <NoMatch default />
            </Router>
        </Layout>
    );
}

export default App;
