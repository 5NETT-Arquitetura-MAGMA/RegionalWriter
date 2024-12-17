using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RegionalWriter.Model.View;
using RegionalWriter.Queue;
using System.Text;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices(async (context, services) =>
    {
        var configuration = context.Configuration;
        string rabbitUser = configuration["Queue:User"] ?? "guest";
        string rabbitPassword = configuration["Queue:Password"] ?? "guest";
        string rabbitHost = configuration["Queue:Host"] ?? "localhost";
        // Criação da conexão RabbitMQ
        var factory = new ConnectionFactory()
        {
            HostName = rabbitHost,
            UserName = rabbitUser,
            Password = rabbitPassword
        };

        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        // Declaração da fila (garantir que exista)
        await channel.QueueDeclareAsync(queue: "regional_delete",
                              durable: true,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);
        await channel.QueueDeclareAsync(queue: "regional_update",
                          durable: true,
                          exclusive: false,
                          autoDelete: false,
                          arguments: null);
        await channel.QueueDeclareAsync(queue: "regional_create",
                          durable: true,
                          exclusive: false,
                          autoDelete: false,
                          arguments: null);
        Console.WriteLine(" [*] Aguardando mensagens. Pressione CTRL+C para sair.");

        var consumerCreate = new AsyncEventingBasicConsumer(channel);
        var consumerUpdate = new AsyncEventingBasicConsumer(channel);
        var consumerDelete = new AsyncEventingBasicConsumer(channel);

        consumerCreate.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var dto = JsonConvert.DeserializeObject<RegionalDto>(message);
                if (dto != null)
                {
                    var ctx = new RegionalCreateConsumer(configuration);
                    await ctx.Execute(dto);
                }
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Falha ao inserir contato. Mensagem de erro: {ex.Message} - Local do erro: {ex.StackTrace}");
            }
        };

        consumerUpdate.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var dto = JsonConvert.DeserializeObject<RegionalUpdateDto>(message);
                if (dto != null)
                {
                    var ctx = new RegionalUpdateConsumer(configuration);
                    await ctx.Execute(dto);
                }
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Falha ao inserir contato. Mensagem de erro: {ex.Message} - Local do erro: {ex.StackTrace}");
            }
        };

        consumerDelete.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var dto = JsonConvert.DeserializeObject<RegionalDeleteDto>(message);
                if (dto != null)
                {
                    var ctx = new RegionalDeleteConsumer(configuration);
                    await ctx.Execute(dto);
                }
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Falha ao inserir contato. Mensagem de erro: {ex.Message} - Local do erro: {ex.StackTrace}");
            }
        };

        await channel.BasicConsumeAsync(queue: "regional_create",
                                autoAck: true,
                                consumer: consumerCreate);
        await channel.BasicConsumeAsync(queue: "regional_update",
                                autoAck: true,
                                consumer: consumerUpdate);

        await channel.BasicConsumeAsync(queue: "regional_delete",
                                        autoAck: true,
                                        consumer: consumerDelete);

        Console.WriteLine("Consumidor iniciado.");
    });

var host = builder.Build();

// Inicia o host
await host.RunAsync();