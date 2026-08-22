using System.Reflection.Metadata;
using RagApi.Infrastructure.Models;

public class DocumentRepository
{
    // Tầng giao tiếp app và db
    private readonly AppDbContext _context;
    public DocumentRepository(AppDbContext context)
    {
        _context = context;
    }

    // lưu metadata vào bên trong postgres
    public async Task<Documents> SaveDocumentMetadataAsync(string fileName, long fileSize, Guid userId, List<string> chunkTexts)
    {
        Documents newDocument = new Documents { 
            Filename = fileName, 
            FileSize = fileSize, 
            UserId = userId, 
            Status = "completed",
            CreatedAt = DateTime.UtcNow,
            TotalChunks = chunkTexts.Count
        };

        _context.Documents.Add(newDocument);

        for(int i = 0; i < chunkTexts.Count; i++)
        {
            _context.Chunks.Add
            (
                // Huy - part 1
                new Chunks
                {
                    DocumentId = newDocument.DocumentId,
                    ChunkTitle = $"{fileName} - part {i + 1}",
                    ChunkText =  chunkTexts[i]
                }
            );
        }


        await _context.SaveChangesAsync();

        return newDocument;

    }
}


