using System.Runtime.CompilerServices;
using Grpc.Net.Client.Balancer;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Npgsql.Internal;
using Qdrant.Client;
using Qdrant.Client.Grpc;

public class QdrantRepository
{
    private readonly QdrantClient _qdrantClient;
    private const string Collection = "knowledge";

    public QdrantRepository(QdrantClient qdrantClient)
    {
        _qdrantClient = qdrantClient;
    }

    public async Task EnsureCollectionAsync()
    {
        // kiểm tra collection có tồn tại hay không
        bool existsCollection = await _qdrantClient.CollectionExistsAsync(Collection);

        if (existsCollection)
        {
            return;
        }

        await _qdrantClient.CreateCollectionAsync(Collection, new VectorParams{ Size = 768, Distance = Distance.Cosine});
    }

    public async Task UpsertAsync(string text, float[] vector, string fileName, int index)
    {
        PointStruct point = new PointStruct
        {
            Id = Guid.NewGuid(),
            Vectors = vector,
            Payload =
            {
                ["text"] = text,
                ["filename"] = fileName,
                ["chunk_index"] = index
            }
        };

        await _qdrantClient.UpsertAsync(Collection, points: new List<PointStruct>() {point});
    }

}