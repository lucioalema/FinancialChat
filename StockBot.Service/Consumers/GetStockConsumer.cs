using MassTransit;
using StockBot.Messages;
using StockBot.Service.Services;

namespace StockBot.Service.Consumers
{
    internal class GetStockConsumer : IConsumer<IGetStock>
    {
        private readonly IStockApi _stockApi;

        public GetStockConsumer(IStockApi stockApi)
        {
            _stockApi = stockApi;
        }
        public async Task Consume(ConsumeContext<IGetStock> context)
        {
            var stock = await _stockApi.GetStock(context.Message.StockCode);
            if (stock != null)
            {
                var botMessage = $"{stock.Split(',')[0]} quote is ${stock.Split(',')[6]} per share.";
                //await Clients.All.SendAsync("ReceiveMessage", "BOT", botMessage);
            }
        }
    }
}
