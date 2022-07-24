using System;

namespace StockBot.Messages
{
    public class IGetStock
    {
        public string StockCode { get; set; }
        public DateTime Datetime { get; set; }
    }
}