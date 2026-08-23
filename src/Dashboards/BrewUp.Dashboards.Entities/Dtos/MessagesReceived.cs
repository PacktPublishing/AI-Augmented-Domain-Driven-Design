using BrewUp.Shared.ReadModel;

namespace BrewUp.Dashboards.Entities.Dtos;

public class MessagesReceived : DtoBase
{
    public string EntityName { get; private set; } = string.Empty;
    public DateTime ReceivedAt { get; private set; }
    
    protected MessagesReceived() { }
    
    public static MessagesReceived Create(Guid id, string entityName) => new (id,  entityName);
    
    private MessagesReceived(Guid id, string entityName)
    {
        Id = id.ToString();
        EntityName = entityName;
        ReceivedAt = DateTime.UtcNow;
    }
}