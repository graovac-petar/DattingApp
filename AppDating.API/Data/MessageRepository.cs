﻿using AppDating.API.DTO;
using AppDating.API.Helpers;
using AppDating.API.Interfaces;
using AppDating.API.Model.Domain;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace AppDating.API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        public void AddMessage(Message message)
        {
            context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            context.Messages.Remove(message);
        }

        public async Task<Message?> GetMessage(int id)
        {
            return await context.Messages.FindAsync(id);
        }

        public async Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = context.Messages.OrderByDescending(m => m.MessageSent).AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.Recipient.UserName == messageParams.Username && u.RecipientDeleted == false && u.RecipientDeleted == false),
                "Outbox" => query.Where(u => u.Sender.UserName == messageParams.Username && u.SenderDeleted == false && u.SenderDeleted == false),
                _ => query.Where(u => u.Recipient.UserName == messageParams.Username && u.DateRead == null && u.RecipientDeleted == false && u.RecipientDeleted == false)
            };

            var messages = query.ProjectTo<MessageDTO>(mapper.ConfigurationProvider);

            return await PagedList<MessageDTO>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            var messages = await context.Messages
                .Include(x => x.Sender).ThenInclude(x => x.Photos)
                .Include(x => x.Recipient).ThenInclude(x => x.Photos)
                .Where(m => m.Recipient.UserName == currentUsername && m.RecipientDeleted == false
                    && m.Sender.UserName == recipientUsername
                    || m.Recipient.UserName == recipientUsername && m.Sender.UserName == currentUsername
                    && m.SenderDeleted == false)
                .OrderBy(m => m.MessageSent)
                .ToListAsync();

            var unreadMessages = messages.Where(m => m.DateRead == null && m.Recipient.UserName == currentUsername).ToList();

            if (unreadMessages.Any())
            {
                unreadMessages.ForEach(m => m.DateRead = DateTime.UtcNow);
                await context.SaveChangesAsync();
            }

            return mapper.Map<IEnumerable<MessageDTO>>(messages);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}
