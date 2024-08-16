
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;

var kernel = Kernel.CreateBuilder()
                    .AddOllamaTextEmbeddingGeneration("nomic-embed-text")
                    .Build();

#pragma warning disable SKEXP0001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
var embeddingService = kernel.Services.GetRequiredService<ITextEmbeddingGenerationService>();
var result = await embeddingService.GenerateEmbeddingAsync("你是一位资深.Net开发人员，精通各种.Net框架");
Console.WriteLine(string.Join(",", result.ToArray()));
#pragma warning restore SKEXP0001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。