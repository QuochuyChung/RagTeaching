namespace RagApi.Infrastructure.Models;
public class Conversations
{
    public Guid ConversationId {get; set;} = Guid.NewGuid();
    public string Title {get; set;} = string.Empty;
    public int TokenCounts {get; set;}
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
    public Guid UserId {get; set;} = Guid.NewGuid();
    public ICollection<Messages> Messages {get; set;} = new List<Messages>();
    public Users? Users = null!;
} 