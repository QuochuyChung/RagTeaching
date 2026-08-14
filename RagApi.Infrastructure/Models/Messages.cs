namespace RagApi.Infrastructure.Models;
public class Messages
{
    public Guid MessageId {get; set;} = Guid.NewGuid();
    public string RoleChat {get; set;} = string.Empty;
    public string Content {get; set;} = string.Empty;
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
    public string CitationMessage {get; set;} = string.Empty;   
    public int MessageIndex {get; set;}
    public Guid ConversationId {get; set;}
    public Conversations? Conversations = null!;
}