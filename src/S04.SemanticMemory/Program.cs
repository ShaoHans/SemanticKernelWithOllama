
using Microsoft.SemanticKernel.Connectors.Sqlite;
using Microsoft.SemanticKernel.Memory;

const string MemoryCollectionName = "DotNetDoc";

// 使用内存
//var store = new VolatileMemoryStore();

// 使用sqlite
var store = await SqliteMemoryStore.ConnectAsync(Path.Combine(Directory.GetCurrentDirectory(), "DotNetDoc.db"));

var memory = new MemoryBuilder()
    .WithOllamaTextEmbeddingGeneration("nomic-embed-text")
    .WithMemoryStore(store)
    .Build();

await RunExampleAsync(memory);

async Task RunExampleAsync(ISemanticTextMemory memory)
{
    await StoreMemoryAsync(memory);

    await SearchMemoryAsync(memory, "如何开发AI程序?");

    await SearchMemoryAsync(memory, "如何构建WebApi服务端应用?");
}

async Task SearchMemoryAsync(ISemanticTextMemory memory, string query)
{
    Console.WriteLine("\nQuery: " + query + "\n");

    var memoryResults = memory.SearchAsync(MemoryCollectionName, query, limit: 2, minRelevanceScore: 0.5);

    int i = 0;
    await foreach (MemoryQueryResult memoryResult in memoryResults)
    {
        Console.WriteLine($"Result {++i}:");
        Console.WriteLine("  URL:     : " + memoryResult.Metadata.Id);
        Console.WriteLine("  Title    : " + memoryResult.Metadata.Description);
        Console.WriteLine("  Relevance: " + memoryResult.Relevance);
        Console.WriteLine();
    }

    Console.WriteLine("----------------------");
}

async Task StoreMemoryAsync(ISemanticTextMemory memory)
{
    Console.WriteLine("\n添加.Net各类框架的官方学习文档...");
    var githubFiles = SampleData();
    var i = 0;
    foreach (var entry in githubFiles)
    {
        await memory.SaveReferenceAsync(
            collection: MemoryCollectionName,
            externalSourceName: "GitHub",
            externalId: entry.Key,
            description: entry.Value,
            text: entry.Value);

        Console.Write($" #{++i} saved.");
    }

    Console.WriteLine("\n----------------------");
}

static Dictionary<string, string> SampleData()
{
    return new Dictionary<string, string>
    {
        ["https://learn.microsoft.com/zh-cn/aspnet/core/?view=aspnetcore-8.0"]
            = "了解如何使用 ASP.NET Core 创建快速、安全、跨平台和基于云的 Web 应用和服务。 浏览教程、示例代码、基础知识、API 参考和更多内容。",
        ["https://learn.microsoft.com/zh-cn/dotnet/maui/?view=net-maui-8.0"]
            = "通过 .NET 多平台应用 UI (.NET MAUI)，可使用 .NET 跨平台 UI 工具包生成各种原生应用，这一工具包可适应 Android、iOS、macOS、Windows 和 Tizen 系统上的移动端和桌面端应用的形态。",
        ["https://learn.microsoft.com/zh-cn/dotnet/desktop/wpf/?view=netdesktop-8.0"]
            = "了解如何在 .NET 8 上使用 Windows Presentation Foundation (WPF)，这是一种适用于 Windows 的开放源代码图形用户界面。",
        ["https://learn.microsoft.com/zh-cn/dotnet/aspire/"]
            = "了解 .NET Aspire，这是一个自以为是的云就绪堆栈，用于构建可观察、生产就绪的分布式应用程序。浏览 API 参考、示例代码、教程、快速入门、概念文章等。",
        ["https://learn.microsoft.com/zh-cn/dotnet/ai/"]
            = "了解如何将 AI 与 .NET 配合使用。 浏览示例代码、教程、快速入门、概念文章等。",
    };
}