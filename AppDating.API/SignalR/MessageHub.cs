﻿using AppDating.API.DTO;
using AppDating.API.Extensions;
using AppDating.API.Interfaces;
using AppDating.API.Model.Domain;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace AppDating.API.SignalR
{
    public class MessageHub : Hub
    {
        private readonly IMessageRepository messageRepository;
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;

        public MessageHub(IMessageRepository messageRepository, IUserRepository userRepository, IMapper mapper)
        {
            this.messageRepository = messageRepository;
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext?.Request.Query["user"].ToString();

            if (string.IsNullOrEmpty(otherUser) || Context.User == null) throw new HubException("Cannot get other user");
            string groupName = GetGroupName(Context.User.GetUsername(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var messages = await messageRepository.GetMessageThread(Context.User.GetUsername(), otherUser!);

            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDTO createMessageDTO)
        {
            var username = Context.User?.GetUsername();

            if (username == createMessageDTO.RecipientUsername || string.IsNullOrEmpty(username)) throw new HubException("Cannot message yourself");

            var sender = await userRepository.GetUserByUsernameAsync(username);
            var recipient = await userRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername);

            if (recipient == null || sender == null || sender.UserName == null || recipient.UserName == null) throw new HubException("Cannot send message now");

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDTO.Content
            };

            messageRepository.AddMessage(message);

            if (await messageRepository.SaveAllAsync())
            {
                var group = GetGroupName(sender.UserName, recipient.UserName);
                await Clients.Group(group).SendAsync("NewMessage", mapper.Map<MessageDTO>(message));
            }
        }

        private string GetGroupName(string caller, string? other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}
