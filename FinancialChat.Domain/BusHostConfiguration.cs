namespace FinancialChat.Domain
{
    public class BusHostConfiguration
    {
        public string ServiceUri { get; set; }
        public string VirtualHost { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string QueueName { get; set; }
        public int ConcurrencyLimit { get; set; } = 8;
        public ushort PrefetchCount { get; set; } = 64;
    }
}