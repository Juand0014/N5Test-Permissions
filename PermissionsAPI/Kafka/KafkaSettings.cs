namespace PermissionsAPI.Kafka
{
    public class KafkaSettings
    {
        public required string BootstrapServers { get; set; }
        public required string Topic { get; set; }
    }
}
