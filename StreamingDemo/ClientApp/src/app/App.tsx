import React, { useEffect } from 'react';
import { Router, Link } from "@reach/router"
import { ToastContainer, toast } from 'react-toastify';
import { Layout } from './Layout';
import { NoMatch } from '../routes/noMatch';
import { Home } from '../features/home/home';

import 'react-toastify/dist/ReactToastify.css';
import '@fontsource/roboto/400.css';

import './App.css';

function App() {
    return (
        <Layout>
            <ToastContainer />
            <Router>
                <Home path="/" />
                <NoMatch default />
            </Router>
        </Layout>
    );
}

export default App;
