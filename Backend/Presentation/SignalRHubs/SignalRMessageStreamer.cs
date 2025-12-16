using Core.Interfaces.Convo;
using Microsoft.AspNetCore.SignalR;

namespace Presentation.SignalRHubs
{
    public class SignalRMessageStreamer(
            IHubContext<ChatHub> hub
            ) : IMessageStreamer
    {
        private readonly IHubContext<ChatHub> _hub = hub;

        public Task StreamAsync(string chatId, string chunk)
        {
            return _hub.Clients.Group(chatId)
                .SendAsync("messageReceived", chatId, chunk);
        }
    }
}

