using FinancialChat.Domain;
using MassTransit;
using StockBot.Service.Consumers;

namespace StockBot.Service
{
    public static class ServicesExtensions
    {
        public static void AddStockMassTransit(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(cfg =>
            {
                cfg.AddConsumer<GetStockConsumer>();
                cfg.UsingRabbitMq(ConfigureBus);
            });
            services.AddHostedService<Worker>();
        }

        static void ConfigureBus(IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator configurator)
        {
            var configuration = context.GetService<IConfiguration>();
            var busConfig = configuration.GetSection("BusHostConfiguration").Get<BusHostConfiguration>();
            var url = $"{busConfig.ServiceUri.TrimEnd('/')}";

            configurator.Host(new Uri(url), hst =>
            {
                hst.Password(busConfig.Password);
                hst.Username(busConfig.Username);
            });
            configurator.ReceiveEndpoint(busConfig.QueueName, endpointCfg =>
            {
                endpointCfg.PrefetchCount = busConfig.PrefetchCount;
                endpointCfg.ConcurrentMessageLimit = busConfig.ConcurrencyLimit;
                endpointCfg.UseRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(5)));
                endpointCfg.ConfigureConsumers(context);
            });

            //configurator.UseExceptionLogger(configuration);
        }
    }
}
