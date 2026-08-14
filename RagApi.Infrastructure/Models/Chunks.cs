using System.Security.Cryptography.X509Certificates;

namespace RagApi.Infrastructure.Models;

public class Chunks
{
    public Guid ChunkId {get; set;} = Guid.NewGuid();
    public string ChunkTitle {get; set;} = string.Empty;
    public string ChunkText {get; set;}  = string.Empty;
    public Guid DocumentId {get; set;} = Guid.NewGuid();
    public Documents? Documents = null!;
}