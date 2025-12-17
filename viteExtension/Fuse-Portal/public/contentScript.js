
// contentScript.js

chrome.runtime.onMessage.addListener((msg, sender, sendResponse) => {
    if (msg.action === "parseUniversityPortal") {
        // Relay message to the page
        window.dispatchEvent(new CustomEvent('portalMessage', { detail: msg }));
        sendResponse({ success: true });
    }
});
