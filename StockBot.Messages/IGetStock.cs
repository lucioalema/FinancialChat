using System;

namespace StockBot.Messages
{
    public class IGetStock
    {
        public string UserFrom { get; set; }
        public string UserTo { get; set; }
        public string StockCode { get; set; }
        public DateTime Datetime { get; set; }
    }
}