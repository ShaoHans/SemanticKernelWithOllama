using Microsoft.SemanticKernel;

namespace S02.CustomHttpClientHandler;

public static class KernelBuilderExtensions
{
    public static IKernelBuilder AddOllamaChatCompletion(this IKernelBuilder builder, string modelId)
    {
        // apiKey随便填写，ollama会忽略此参数
        builder.AddOpenAIChatCompletion(modelId, "useless", httpClient: new HttpClient(new OllamaHttpClientHandler()));
        return builder;
    }
}
