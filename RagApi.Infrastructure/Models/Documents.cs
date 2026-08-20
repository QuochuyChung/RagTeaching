namespace RagApi.Infrastructure.Models;

public class Documents
{
    public Guid DocumentId {get; set;} = Guid.NewGuid();
    public string Filename {get; set;} = string.Empty;
    public int TotalChunks {get; set;}
    public string Status {get; set;} = string.Empty;
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
    public decimal FileSize{get; set;}
    public ICollection<Chunks> Chunks = new List<Chunks>();
    public Guid UserId {get; set;} = Guid.NewGuid();
    public Users? Users {get; set;} = null!;

}
