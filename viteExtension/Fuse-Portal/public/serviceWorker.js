console.log("Fuse Portal service worker is alive ✅");


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
