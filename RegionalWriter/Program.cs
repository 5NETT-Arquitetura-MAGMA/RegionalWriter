using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Prometheus;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RegionalWriter.Model.View;
using RegionalWriter.Queue;
using System.Diagnostics;
using System.Text;

var metricsServer = new KestrelMetricServer(port: 8080);
metricsServer.Start();

var consumedMessages = Metrics.CreateCounter("rabbitmq_consumed_messages_total", "Total de mensagens consumidas", new CounterConfiguration
{
    LabelNames = ["queue"]
});
var failedMessages = Metrics.CreateCounter("rabbitmq_failed_messages_total", "Total de mensagens que falharam", new CounterConfiguration
{
    LabelNames = ["queue"]
});
var messageProcessingTime = Metrics.CreateHistogram("rabbitmq_message_processing_duration_seconds", "Duração do processamento de mensagens", new HistogramConfiguration
{
    LabelNames = ["queue"],
    Buckets = Histogram.ExponentialBuckets(0.01, 2, 10)
});
var queues = new[] { "regional_create", "regional_update", "regional_delete" };

foreach (var queue in queues)
{
    consumedMessages.WithLabels(queue).Inc(0); // Incrementa em zero para registrar a métrica
    failedMessages.WithLabels(queue).Inc(0);   // Para mensagens com falha
    messageProcessingTime.WithLabels(queue).Observe(0); // Inicializa histogramas
}
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
        await channel.QueueDeclareAsync(queue: "regional_delete", durable: true, exclusive: false, autoDelete: false, arguments: null);
        await channel.QueueDeclareAsync(queue: "regional_update", durable: true, exclusive: false, autoDelete: false, arguments: null);
        await channel.QueueDeclareAsync(queue: "regional_create", durable: true, exclusive: false, autoDelete: false, arguments: null);
        Console.WriteLine(" [*] Aguardando mensagens. Pressione CTRL+C para sair.");

        var consumerCreate = new AsyncEventingBasicConsumer(channel);
        var consumerUpdate = new AsyncEventingBasicConsumer(channel);
        var consumerDelete = new AsyncEventingBasicConsumer(channel);

        consumerCreate.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                await ProcessMessageAsync(ea, "regional_create", configuration);
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Falha ao inserir contato. Mensagem de erro: {ex.Message} - Local do erro: {ex.StackTrace}");
            }
        };

        consumerUpdate.ReceivedAsync += async (model, ea) =>
        {
            await ProcessMessageAsync(ea, "regional_update", configuration);
        };

        consumerDelete.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                await ProcessMessageAsync(ea, "regional_delete", configuration);
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Falha ao inserir contato. Mensagem de erro: {ex.Message} ");
            }
        };

        await channel.BasicConsumeAsync(queue: "regional_create", autoAck: true, consumer: consumerCreate);
        await channel.BasicConsumeAsync(queue: "regional_update", autoAck: true, consumer: consumerUpdate);
        await channel.BasicConsumeAsync(queue: "regional_delete", autoAck: true, consumer: consumerDelete);

        Console.WriteLine("Consumidor iniciado.");
        async Task ProcessMessageAsync(BasicDeliverEventArgs ea, string queueName, IConfiguration config)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                consumedMessages.WithLabels(queueName).Inc();

                switch (queueName)
                {
                    case "regional_create":
                        var createDto = JsonConvert.DeserializeObject<RegionalDto>(message);
                        if (createDto != null)
                        {
                            var ctx = new RegionalCreateConsumer(config);
                            await ctx.Execute(createDto);
                        }
                        break;

                    case "regional_update":
                        var updateDto = JsonConvert.DeserializeObject<RegionalUpdateDto>(message);
                        if (updateDto != null)
                        {
                            var ctx = new RegionalUpdateConsumer(config);
                            await ctx.Execute(updateDto);
                        }
                        break;

                    case "regional_delete":
                        var deleteDto = JsonConvert.DeserializeObject<RegionalDeleteDto>(message);
                        if (deleteDto != null)
                        {
                            var ctx = new RegionalDeleteConsumer(config);
                            await ctx.Execute(deleteDto);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar mensagem da fila {queueName}: {ex.Message} - Local do erro: {ex.StackTrace}");
                failedMessages.WithLabels(queueName).Inc();
            }
            finally
            {
                stopwatch.Stop();
                messageProcessingTime.WithLabels(queueName).Observe(stopwatch.Elapsed.TotalSeconds);
            }
        }
    });

var host = builder.Build();

await host.RunAsync();
metricsServer.Stop();