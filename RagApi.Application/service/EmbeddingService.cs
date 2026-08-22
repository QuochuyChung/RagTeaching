namespace RagApi.service;

using OllamaSharp;
using OllamaSharp.Models;

public class EmbeddingService
{
    private readonly OllamaApiClient _ollamaClient;
    public EmbeddingService(OllamaApiClient ollamaClient)
    {
        _ollamaClient = ollamaClient;
    }

    public async Task<float[]> EmbedAsync(string text)
    {
        var response = await _ollamaClient.EmbedAsync(new EmbedRequest
        {
           Model = "nomic-embed-text",
           Input = new List<string> {text}
        });

        return response.Embeddings[0];
    }
}