using System.Threading.Tasks;

namespace PermissionsAPI.Kafka.Interfaces;

public interface IKafkaProducer
{
    Task SendMessageAsync(string topic, string key, string value);
}
