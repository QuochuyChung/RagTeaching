
namespace RagApi.Infrastructure.Models;
public class Users
{
    public Guid UserId {get; set;} = Guid.NewGuid();
    public string Username {get; set;} = string.Empty;

    // active, unactive 
    public string Status {get; set;} = string.Empty;
    public ICollection<Conversations> Conversations {get; set;} = new List<Conversations>();
    public ICollection<Documents> Documents {get; set;} = new List<Documents>();
}