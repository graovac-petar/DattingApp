using AppDating.API.DTO;
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
        private readonly IHubContext<PresenceHub> presenceHub;

        public MessageHub(IMessageRepository messageRepository, IUserRepository userRepository, IMapper mapper, IHubContext<PresenceHub> presenceHub)
        {
            this.messageRepository = messageRepository;
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.presenceHub = presenceHub;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext?.Request.Query["user"].ToString();

            if (string.IsNullOrEmpty(otherUser) || Context.User == null) throw new HubException("Cannot get other user");
            string groupName = GetGroupName(Context.User.GetUsername(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await AddToGroup(groupName);

            var messages = await messageRepository.GetMessageThread(Context.User.GetUsername(), otherUser!);

            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await RemoveFromMessageGroup();
            await base.OnDisconnectedAsync(exception);
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

            var groupName = GetGroupName(sender.UserName, recipient.UserName);
            var group = await messageRepository.GetMessageGroup(groupName);

            if (group != null && group.Connections.Any(x => x.Username == recipient.UserName))
            {
                message.DateRead = DateTime.UtcNow;
            }
            else
            {
                var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);
                if (connections != null && connections?.Count != null)
                {
                    await presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", new { username = sender.UserName, knownAs = sender.KnownAs });
                }
            }
            messageRepository.AddMessage(message);

            if (await messageRepository.SaveAllAsync())
            {
                await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDTO>(message));
            }
        }

        private async Task<bool> AddToGroup(string groupName)
        {
            var username = Context.User?.GetUsername() ?? throw new Exception("Cannot get username");
            var group = await messageRepository.GetMessageGroup(groupName);
            var connection = new Connection { ConnectionId = Context.ConnectionId, Username = username };

            if (group == null)
            {
                group = new Group { Name = groupName };
                messageRepository.AddGroup(group);
            }

            group.Connections.Add(connection);

            return await messageRepository.SaveAllAsync();

        }

        private async Task RemoveFromMessageGroup()
        {
            var connection = await messageRepository.GetConnection(Context.ConnectionId);
            if (connection != null)
            {
                messageRepository.RemoveConnection(connection);
                await messageRepository.SaveAllAsync();
            }
        }

        private string GetGroupName(string caller, string? other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}
