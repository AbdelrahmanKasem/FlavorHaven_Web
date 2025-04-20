﻿using Microsoft.AspNetCore.SignalR;
using RMSProjectAPI.Database;
using RMSProjectAPI.Database.Entity;

namespace RMSProjectAPI.Hubs
{
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;

        public ChatHub(AppDbContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string senderId, string receiverId, string messageText)
        {
            Guid sender = Guid.Parse(senderId);
            Guid receiver = Guid.Parse(receiverId);

            // Check if a chat already exists
            var chat = _context.Chats
                .FirstOrDefault(c =>
                    (c.User1ID == sender && c.User2ID == receiver) ||
                    (c.User1ID == receiver && c.User2ID == sender));

            // Create a new chat if none exists
            if (chat == null)
            {
                chat = new Chat
                {
                    User1ID = sender,
                    User2ID = receiver,
                    CreatedAt = DateTime.Now
                };

                _context.Chats.Add(chat);
                await _context.SaveChangesAsync();
            }

            // Create and store the message
            var message = new Message
            {
                ChatID = chat.ChatID,
                SenderID = sender,
                MessageText = messageText,
                SentAt = DateTime.Now
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // Send it to everyone
            await Clients.All.SendAsync("ReceiveMessage", senderId, messageText);
        }
    }
}
