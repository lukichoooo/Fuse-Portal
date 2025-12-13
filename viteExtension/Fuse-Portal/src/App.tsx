/// <reference types="chrome" />
import './App.css';

function App() {
    const openChatbot = () => chrome.tabs.create({ url: "https://fuseportal.ai/chat" });
    const openPortal = () => chrome.tabs.create({ url: "https://fuseportal.ai" });

    // chrome.runtime.sendMessage({ action: "runScript" });

    return (
        <div id="root">
            {/* Header hamburger */}
            <div className="header">
                <div className="hamburger" title="Menu">
                    <span></span>
                    <span></span>
                    <span></span>
                </div>
            </div>

            <img src="fuse-portal-logo-dark.png" alt="Fuse Portal Logo" className="logo" />

            <div className="btn-group">
                <button onClick={openChatbot}>Open Chatbot</button>
                <button onClick={openPortal}>Open Portal</button>
            </div>
        </div>
    );
}

export default App;
