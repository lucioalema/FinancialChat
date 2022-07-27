using MassTransit;
using Microsoft.AspNetCore.SignalR.Client;
using StockBot.Messages;
using StockBot.Service.Services;

namespace StockBot.Service.Consumers
{
    public class GetStockConsumer : IConsumer<GetStock>
    {
        private readonly IStockApi _stockApi;

        public GetStockConsumer(IStockApi stockApi)
        {
            _stockApi = stockApi;
        }
        public async Task Consume(ConsumeContext<GetStock> context)
        {
            var stock = await _stockApi.GetStock(context.Message.StockCode);
            if (!string.IsNullOrEmpty(stock))
            {
                var botMessage = $"{stock.Split(',')[0]} quote is ${stock.Split(',')[6]} per share.";
                var connection = new HubConnectionBuilder()
                    .WithUrl("https://localhost:7198/chatHub")
                    .Build();
                await connection.StartAsync();
                await connection.SendAsync("SendBotMessage", context.Message.UserFrom, context.Message.UserTo, botMessage, context.Message.Datetime);
                await connection.DisposeAsync();
            }
            else
                throw new ArgumentException($"There's no stock with code {context.Message.StockCode}");
        }
    }
}
