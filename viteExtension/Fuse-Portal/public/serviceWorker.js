console.log("Ruby service worker is alive ✅");


chrome.runtime.onMessage.addListener((msg, sender) => {
    if (msg.action === "runScript") {
        console.log("⚙️ Running script on current tab...");
        chrome.tabs.query({ active: true, currentWindow: true }, (tabs) => {
            const tab = tabs[0];
            if (tab?.id) {
                chrome.scripting.executeScript({
                    target: { tabId: tab.id },
                    func: () => alert("Hello from Fuse Portal!"),
                });
            }
        });
    }
});

// chrome.scripting.executeScript({
//     target: { tabId: tab.id },
//     func: () => {
//         const html = document.documentElement.outerHTML;
//         const title = document.title;
//         const text = document.body.innerText.slice(0, 5000); // cap size
//
//         chrome.runtime.sendMessage({
//             action: "pageParsed",
//             html,
//             title,
//             text
//         });
//     }
// });
