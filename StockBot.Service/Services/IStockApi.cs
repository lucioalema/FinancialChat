using Refit;

namespace StockBot.Service.Services
{
    public interface IStockApi
    {
        [Get("/q/l/?s={stock_code}&f=sd2t2ohlcv&h&e=csv")]
        public Task<string> GetStock(string stock_code);
    }
}
