using Common;

namespace Microsoft.SemanticKernel;

public static class KernelBuilderExtensions
{
    public static IKernelBuilder AddOllamaChatCompletion(this IKernelBuilder builder, string modelId)
    {
        // apiKey随便填写，ollama会忽略此参数
        builder.AddOpenAIChatCompletion(modelId, "useless", httpClient: new HttpClient(new OllamaHttpClientHandler()));
        return builder;
    }

    public static IKernelBuilder AddOllamaTextEmbeddingGeneration(this IKernelBuilder builder, string modelId)
    {
        #pragma warning disable SKEXP0010

        // apiKey随便填写，ollama会忽略此参数
        builder.AddOpenAITextEmbeddingGeneration(modelId, "useless", httpClient: new HttpClient(new OllamaHttpClientHandler()));
        return builder;
    }
}
