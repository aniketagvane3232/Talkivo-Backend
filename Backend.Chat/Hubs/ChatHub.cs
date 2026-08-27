using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Backend.Chat.Hubs
{
    public class ChatHub : Hub
    {
        // Store connectionId -> username
        private static ConcurrentDictionary<string, string> _users = new();

        public async Task SetUserName(string userName)
        {
            _users[Context.ConnectionId] = userName;
            await Clients.All.SendAsync("SystemMessage", $"{userName} has joined the chat");
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (_users.TryRemove(Context.ConnectionId, out var userName))
            {
                await Clients.All.SendAsync("SystemMessage", $"{userName} has left the chat");
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
