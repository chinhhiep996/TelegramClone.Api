using Microsoft.AspNetCore.SignalR;
using TelegramClone.Api.Data;
using TelegramClone.Api.Models;

namespace TelegramClone.Api.Hubs
{
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;

        public ChatHub(AppDbContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string sender, string receiver, string message)
        {
            var newMessage = new Message
            {
                Sender = sender,
                Receiver = receiver,
                Content = message,
            };

            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            await Clients.User(receiver).SendAsync("ReceiveMessage", sender, message);
            await Clients.User(sender).SendAsync("ReceiveMessage", sender, message);
        }

        public override async Task OnConnectedAsync()
        {
            var username = Context.GetHttpContext()?.Request.Query["username"];
            if (!string.IsNullOrEmpty(username))
            {
                Context.Items["username"] = username;
            }

            await base.OnConnectedAsync();
        }
    }
}
