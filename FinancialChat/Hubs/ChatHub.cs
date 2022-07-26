using FinancialChat.Data;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using StockBot.Messages;

namespace FinancialChat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private ISendEndpoint? _sendEndpoint;
        private readonly ApplicationDbContext _context;
        private static Dictionary<string, List<string>> ConnectedUsers = new Dictionary<string, List<string>>();

        public ChatHub(ISendEndpointProvider sendEndpointProvider, ApplicationDbContext context)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _context = context;
        }

        public async override Task OnConnectedAsync()
        {
            var userName = Context.User.Identity.Name ?? "BOT";
            var userId = Context.UserIdentifier ?? Context.ConnectionId;
            List<string> userIds;

            //store the userid to the list.
            if (!ConnectedUsers.TryGetValue(userName, out userIds))
            {
                userIds = new List<string>();
                userIds.Add(userId);

                ConnectedUsers.Add(userName, userIds);
            }
            else
            {
                userIds.Add(userId);
            }
            await base.OnConnectedAsync();
        }

        public async override Task OnDisconnectedAsync(Exception? ex)
        {
            var userName = Context.User.Identity.Name ?? "BOT";
            var userId = Context.UserIdentifier ?? Context.ConnectionId;
            List<string> userIds;

            //remove userid from the List
            if (ConnectedUsers.TryGetValue(userName, out userIds))
            {
                userIds.Remove(userId);
            }
            await base.OnDisconnectedAsync(ex);
        }

        public async Task SendMessage(string user, string message, DateTime? datetime = null)
        {
            var userIds = ConnectedUsers.GetValueOrDefault(user);
            userIds.AddRange(ConnectedUsers.GetValueOrDefault(Context.User.Identity.Name));

            if (userIds.Count > 0)
            {
                datetime ??= DateTime.Now;
                _context.Messages.Add(new Domain.Messages 
                {
                    UserFrom = Context.User.Identity.Name,
                    UserTo = user,
                    Message = message,
                    DateTime = datetime.Value
                });
                _context.SaveChanges();
                await Clients
                    .Users(userIds)
                    .SendAsync("ReceiveMessage", Context.User.Identity.Name, message, datetime.Value.ToLongTimeString());
                if (message.Contains("/stock="))
                {
                    var stock_code = message.Split("/stock=")[1].Split(' ').FirstOrDefault();
                    if (!string.IsNullOrEmpty(stock_code))
                    {
                        _sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("rabbitmq://localhost/stock"));
                        await _sendEndpoint
                            .Send<IGetStock>(new { 
                                UserFrom = Context.User.Identity.Name, 
                                UserTo = user, 
                                StockCode = stock_code, 
                                datetime = DateTime.Now
                            });
                    }
                }
            }
            else
            {
                throw new ArgumentNullException("User invalid");
            }
        }

        public async Task SendBotMessage(string userFrom, string userTo, string message, DateTime datetime)
        {
            var userIds = ConnectedUsers.GetValueOrDefault(userFrom);
            userIds.AddRange(ConnectedUsers.GetValueOrDefault(userTo));
            if (userIds.Count >= 2)
            {
                await Clients
                    .Users(userIds)
                    .SendAsync("ReceiveMessage", "BOT", message, datetime.ToLongTimeString() ?? DateTime.Now.ToLongTimeString());
            }
            else
            {
                throw new ArgumentNullException("User invalid");
            }
        }
    }
}
