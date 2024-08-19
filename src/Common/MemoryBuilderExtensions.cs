using Common;

using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace Microsoft.SemanticKernel.Memory;

public static class MemoryBuilderExtensions
{
#pragma warning disable SKEXP0001 
    public static MemoryBuilder WithOllamaTextEmbeddingGeneration(this MemoryBuilder memoryBuilder, string modelId)
#pragma warning restore SKEXP0001 
    {
        #pragma warning disable SKEXP0010
        memoryBuilder.WithOpenAITextEmbeddingGeneration(modelId, "useless", httpClient: new HttpClient(new OllamaHttpClientHandler()));
        return memoryBuilder;
    }
}
