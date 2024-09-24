using Confluent.Kafka;
using Microsoft.Extensions.Options;
using PermissionsAPI.Kafka.Interfaces;
using System;
using System.Threading.Tasks;

namespace PermissionsAPI.Kafka;

public class KafkaProducer : IKafkaProducer
{
    private readonly string _bootstrapServers;

    public KafkaProducer(IOptions<KafkaSettings> kafkaSettings)
    {
        _bootstrapServers = kafkaSettings.Value.BootstrapServers;
    }

    public async Task SendMessageAsync(string topic, string key, string value)
    {
        var config = new ProducerConfig { BootstrapServers = _bootstrapServers };

        using (var producer = new ProducerBuilder<string, string>(config).Build())
        {
            try
            {
                var message = new Message<string, string> { Key = key, Value = value };
                var deliveryResult = await producer.ProduceAsync(topic, message);

                Console.WriteLine($"Mensaje enviado a Kafka: {deliveryResult.Value} con offset {deliveryResult.Offset}");
            }
            catch (ProduceException<string, string> e)
            {
                Console.WriteLine($"Error al enviar mensaje: {e.Error.Reason}");
            }
        }
    }
}
