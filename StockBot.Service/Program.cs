using Microsoft.Extensions.Options;
using Refit;
using StockBot.Service;
using StockBot.Service.Configuration;
using StockBot.Service.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddOptions<AppSettingsConfiguration>()
        .Configure<IConfiguration>((opt, cfg) =>
        {
            cfg?.GetSection(nameof(AppSettingsConfiguration))?.Bind(opt);
        });

        services.AddRefitClient<IStockApi>()
        .ConfigureHttpClient((sp, client) =>
        {
            var e = sp.GetService<IOptions<AppSettingsConfiguration>>()?.Value?.StockApiUrl;
            client.BaseAddress = new Uri(e);
            client.Timeout = TimeSpan.FromMinutes(5);
        });

        services.AddStockMassTransit(hostContext.Configuration);
    })
    .Build();

await host.RunAsync();
