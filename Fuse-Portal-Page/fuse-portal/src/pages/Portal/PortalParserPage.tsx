import { useEffect, useState } from 'react';
import PortalService from '../../services/PortalService';
import './PortalParserPage.css';
import { useNavigate } from 'react-router-dom';

// Extend Window interface to include chrome API
declare global {
    interface Window {
        chrome?: {
            runtime?: {
                onMessage?: {
                    addListener: (callback: (message: any, sender: any, sendResponse: (response?: any) => void) => boolean | void) => void;
                    removeListener: (callback: any) => void;
                }
            }
        }
    }
}

export default function PortalParserPage() {
    const navigate = useNavigate();
    const [isLoading, setIsLoading] = useState(false);
    const [status, setStatus] = useState<'idle' | 'receiving' | 'uploading' | 'success' | 'error'>('idle');
    const [errorMessage, setErrorMessage] = useState('');
    const [receivedText, setReceivedText] = useState('');

    useEffect(() => {
        const listener = (e: any) => {
            const message = e.detail;
            if (message.action === 'parseUniversityPortal') {
                console.log('Received page text from extension');
                setStatus('receiving');
                setReceivedText(message.pageText);

                (async () => {
                    try {
                        setStatus('uploading');
                        setIsLoading(true);

                        await PortalService.uploadHtml(message.pageText);

                        setStatus('success');
                        setIsLoading(false);

                        setTimeout(() => navigate('/portal'), 800);

                    } catch (error: any) {
                        console.error('Upload failed:', error);
                        setStatus('error');
                        setErrorMessage(error.message || 'Failed to upload page text');
                        setIsLoading(false);
                    }
                })();
            }
        };

        window.addEventListener('portalMessage', listener);
        return () => window.removeEventListener('portalMessage', listener);
    }, [navigate]);


    const getStatusDisplay = () => {
        switch (status) {
            case 'receiving':
                return { text: 'Receiving page data...', color: '#0d6efd' };
            case 'uploading':
                return { text: 'Uploading to server...', color: '#0d6efd' };
            case 'success':
                return { text: 'Successfully processed!', color: '#198754' };
            case 'error':
                return { text: 'Upload failed', color: '#dc3545' };
            default:
                return { text: 'Waiting for extension...', color: '#6c757d' };
        }
    };

    const statusDisplay = getStatusDisplay();

    return (
        <div className="parser-container">
            <div className="parser-card">
                <div className="parser-header">
                    <h1>University Portal Parser</h1>
                    <p>Use the browser extension to parse university portal pages</p>
                </div>

                <div className="status-section">
                    <div
                        className={`status-badge ${status}`}
                        style={{ borderColor: statusDisplay.color }}
                    >
                        <div
                            className="status-indicator"
                            style={{ background: statusDisplay.color }}
                        />
                        <span style={{ color: statusDisplay.color }}>
                            {statusDisplay.text}
                        </span>
                    </div>

                    {isLoading && (
                        <div className="loading-spinner">
                            <div className="spinner"></div>
                        </div>
                    )}

                    {status === 'success' && (
                        <div className="success-message">
                            <svg width="48" height="48" viewBox="0 0 24 24" fill="none">
                                <circle cx="12" cy="12" r="10" stroke="#198754" strokeWidth="2" />
                                <path d="M8 12l2 2 4-4" stroke="#198754" strokeWidth="2" strokeLinecap="round" />
                            </svg>
                            <p>Page text successfully uploaded</p>
                        </div>
                    )}

                    {status === 'error' && (
                        <div className="error-message">
                            <svg width="48" height="48" viewBox="0 0 24 24" fill="none">
                                <circle cx="12" cy="12" r="10" stroke="#dc3545" strokeWidth="2" />
                                <path d="M15 9l-6 6m0-6l6 6" stroke="#dc3545" strokeWidth="2" strokeLinecap="round" />
                            </svg>
                            <p>{errorMessage}</p>
                        </div>
                    )}
                </div>

                {receivedText && status !== 'error' && (
                    <div className="preview-section">
                        <h3>Received Text Preview</h3>
                        <div className="text-preview">
                            {receivedText.substring(0, 500)}
                            {receivedText.length > 500 && '...'}
                        </div>
                        <p className="char-count">{receivedText.length.toLocaleString()} characters</p>
                    </div>
                )}

                <div className="instructions">
                    <h3>How to use:</h3>
                    <ol>
                        <li>Navigate to your university portal page</li>
                        <li>Click the Fuse Portal extension icon</li>
                        <li>Click "Parse University Portal" button</li>
                        <li>The page text will be automatically sent here and uploaded</li>
                    </ol>
                </div>
            </div>
        </div>
    );
}
