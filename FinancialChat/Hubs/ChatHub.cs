using FinancialChat.Services;
using Microsoft.AspNetCore.SignalR;

namespace FinancialChat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IStockApi _stockApi;

        public ChatHub(IStockApi stockApi)
        {
            _stockApi = stockApi;
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
            if (message.Contains("/stock="))
            {
                var stock_code = message.Split("/stock=")[1].Split(' ').FirstOrDefault();
                if (!string.IsNullOrEmpty(stock_code))
                {
                    var stock = await _stockApi.GetStock(stock_code);
                    if (stock != null)
                    {
                        var botMessage = $"{stock.Split(',')[0]} quote is ${stock.Split(',')[6]} per share.";
                        await Clients.All.SendAsync("ReceiveMessage", "BOT", botMessage);
                    }
                }
            }
        }
    }
}
