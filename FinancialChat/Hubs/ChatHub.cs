using MassTransit;
using Microsoft.AspNetCore.SignalR;
using StockBot.Messages;

namespace FinancialChat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private ISendEndpoint? _sendEndpoint;
        public ChatHub(ISendEndpointProvider sendEndpointProvider) => _sendEndpointProvider = sendEndpointProvider;

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
            if (message.Contains("/stock="))
            {
                var stock_code = message.Split("/stock=")[1].Split(' ').FirstOrDefault();
                if (!string.IsNullOrEmpty(stock_code))
                {
                    _sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("rabbitmq://localhost/stock"));
                    await _sendEndpoint.Send<IGetStock>(new { StockCode = stock_code });
                }
            }
        }
    }
}
