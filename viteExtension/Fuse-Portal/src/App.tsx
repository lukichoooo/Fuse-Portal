/// <reference types="chrome" />
import { useState, useEffect } from 'react';
import './App.css';

function App() {
    const [isConnected, setIsConnected] = useState(false);
    const [isLoading, setIsLoading] = useState(false);
    const [status, setStatus] = useState('');
    const [currentPageTitle, setCurrentPageTitle] = useState('');

    useEffect(() => {
        checkConnection();
        getCurrentPageInfo();
    }, []);

    const getCurrentPageInfo = async () => {
        const [tab] = await chrome.tabs.query({ active: true, currentWindow: true });
        if (tab?.title) {
            setCurrentPageTitle(tab.title);
        }
    };

    const checkConnection = () => {
        chrome.tabs.query({}, (tabs) => {
            const portalTab = tabs.find(tab => tab.url?.includes('localhost:5173'));
            setIsConnected(!!portalTab);
        });
    };

    const parseAndSend = async () => {
        setIsLoading(true);
        setStatus('Parsing page...');

        try {
            // Get current tab
            const [tab] = await chrome.tabs.query({ active: true, currentWindow: true });

            if (!tab?.id) {
                setStatus('Error: No active tab');
                setIsLoading(false);
                return;
            }

            // Check if we can access this page
            if (tab.url?.startsWith('chrome://') || tab.url?.startsWith('chrome-extension://')) {
                setStatus('Error: Cannot parse browser pages');
                setIsLoading(false);
                return;
            }

            // Execute script to get page text
            const results = await chrome.scripting.executeScript({
                target: { tabId: tab.id },
                func: () => {
                    return document.body.innerText;
                }
            });

            if (!results || !results[0] || !results[0].result) {
                setStatus('Error: Could not extract text');
                setIsLoading(false);
                return;
            }

            const pageText = results[0].result as string;
            setStatus(`Extracted ${pageText.length} characters. Sending...`);

            // Find portal tab
            const allTabs = await chrome.tabs.query({});
            const portalTab = allTabs.find(t => t.url?.includes('localhost:5173'));

            if (!portalTab?.id) {
                setStatus('Error: Portal not open');
                setIsLoading(false);
                return;
            }

            // Send to portal
            chrome.tabs.sendMessage(
                portalTab.id,
                {
                    action: 'parseUniversityPortal',
                    pageText: pageText
                },
                (response) => {
                    if (chrome.runtime.lastError) {
                        setStatus('Error: ' + chrome.runtime.lastError.message);
                        setIsLoading(false);
                    } else if (response?.success) {
                        setStatus('✓ Sent successfully!');
                        setIsLoading(false);
                        setTimeout(() => setStatus(''), 2000);
                    } else {
                        setStatus('Error: Failed to send');
                        setIsLoading(false);
                    }
                }
            );

        } catch (error: any) {
            console.error('Parse error:', error);
            setStatus('Error: ' + error.message);
            setIsLoading(false);
        }
    };

    return (
        <div id="root">
            <div className="header">
                <h2>Fuse Portal Parser</h2>
            </div>

            <img src="/ruby.png" alt="Ruby logo" className="logo" />

            <div className="status-indicator">
                <div className={`dot ${isConnected ? 'connected' : 'disconnected'}`}></div>
                <span>{isConnected ? 'Portal Connected' : 'Portal Not Found'}</span>
            </div>

            <div className="btn-group">
                <button
                    onClick={parseAndSend}
                    disabled={!isConnected || isLoading}
                    className="primary-btn"
                >
                    {isLoading ? 'Processing...' : 'Parse University Portal'}
                </button>

                <button
                    onClick={checkConnection}
                    className="secondary-btn"
                >
                    Refresh Connection
                </button>
            </div>

            {status && (
                <div className={`status-message ${status.includes('Error') ? 'error' : 'success'}`}>
                    {status}
                </div>
            )}

            {!isConnected && (
                <div className="info-box">
                    <p>Please open <strong>localhost:5173/parser</strong> to use this extension.</p>
                </div>
            )}

            {isConnected && currentPageTitle && (
                <div className="current-page-info">
                    <p className="label">Current page:</p>
                    <p className="page-title">{currentPageTitle}</p>
                    <p className="hint">This page will be parsed when you click the button above.</p>
                </div>
            )}
        </div>
    );
}

export default App;
