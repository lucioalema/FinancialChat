using FinancialChat.Domain;
using FinancialChat.Hubs;
using MassTransit;
using StockBot.Messages;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

//MassTransit / RabbitMq
builder.Services.AddMassTransit(mt =>
{
    mt.UsingRabbitMq((context, cfg) =>
    {
        var configuration = context.GetService<IConfiguration>();
        var busConfig = configuration.GetSection("BusHostConfiguration").Get<BusHostConfiguration>();
        var url = $"{busConfig.ServiceUri.TrimEnd('/')}";
        cfg.Host(new Uri(url), h =>
        {
            h.Username(busConfig.Username);
            h.Password(busConfig.Password);
        });

        MessageDataDefaults.ExtraTimeToLive = TimeSpan.FromDays(1);
        MessageDataDefaults.Threshold = 2000;
        MessageDataDefaults.AlwaysWriteToRepository = false;
    });

    mt.AddRequestClient<IGetStock>();
});

builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapHub<ChatHub>("/chatHub");

app.Run();
