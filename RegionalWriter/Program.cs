using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RegionalWriter.Queue;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;
        string rabbitUser = configuration["Queue:User"] ?? "guest";
        string rabbitPassword = configuration["Queue:Password"] ?? "guest";
        string rabbitHost = configuration["Queue:Host"] ?? "localhost";

        services.AddMassTransit(x =>
        {
            // Registra os consumidores
            x.AddConsumer<RegionalCreateConsumer>();
            x.AddConsumer<RegionalUpdateConsumer>();
            x.AddConsumer<RegionalDeleteConsumer>();

            // Configura o RabbitMQ
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitHost, "/", h =>
                {
                    h.Username(rabbitUser);
                    h.Password(rabbitPassword);
                });

                // Configura os endpoints e consumidores
                cfg.ReceiveEndpoint("regional_create", e =>
                {
                    e.Durable = true;
                    e.UseRawJsonSerializer();

                    e.ConfigureConsumer<RegionalCreateConsumer>(context);
                });
                cfg.ReceiveEndpoint("regional_update", e =>
                {
                    e.Durable = true;
                    e.UseRawJsonSerializer();

                    e.ConfigureConsumer<RegionalUpdateConsumer>(context);
                });
                cfg.ReceiveEndpoint("regional_delete", e =>
                {
                    e.Durable = true;
                    e.UseRawJsonSerializer();
                    e.ConfigureConsumer<RegionalDeleteConsumer>(context);
                });
            });
        });

        // Adiciona o Hosted Service do MassTransit
        services.AddMassTransitHostedService();
    });

var host = builder.Build();

// Inicia o host
await host.RunAsync();