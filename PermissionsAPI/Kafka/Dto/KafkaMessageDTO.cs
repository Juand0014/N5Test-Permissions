namespace PermissionsAPI.Kafka.Dto;

public class KafkaMessageDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public KafkaMessageDTO(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }
}

