
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using S05.FunctionCall;

using System.Runtime.Serialization;
using System.Text;

#pragma warning disable SKEXP0001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。

var kernel = Kernel.CreateBuilder()
                .AddOllamaChatCompletion("llama3.1")
                .Build();

kernel.Plugins.AddFromType<TimePlugin>();

var executionSettings = new OpenAIPromptExecutionSettings
{
    Temperature = 0.1,
    ToolCallBehavior = ToolCallBehavior.EnableKernelFunctions
};


var chatService = kernel.GetRequiredService<IChatCompletionService>();
var chatHistory = new ChatHistory("你是一位资深.Net开发人员，精通各种.Net框架");
var content = new StringBuilder();

while (true)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write("You:");
    chatHistory.AddUserMessage(Console.ReadLine()!);

    content.Clear();
    Console.ResetColor();
    Console.Write("Assistant:");

    AuthorRole? authorRole = null;
    var fccBuilder = new FunctionCallContentBuilder();

    var result = await chatService.GetChatMessageContentAsync(chatHistory, executionSettings, kernel);
    if(result != null)
    {
        Console.Write(result);
        content.Append(result.Content);
        authorRole ??= result.Role;
    }

    var functionCalls = fccBuilder.Build();
    if (functionCalls.Any())
    {
        var fcContent = new ChatMessageContent(role: authorRole ?? default, content: null);
        chatHistory.Add(fcContent);

        // Iterating over the requested function calls and invoking them
        foreach (var functionCall in functionCalls)
        {
            fcContent.Items.Add(functionCall);

            var functionResult = await functionCall.InvokeAsync(kernel);

            chatHistory.Add(functionResult.ToChatMessage());
            Console.Write(functionResult.ToChatMessage());
        }
    }

    Console.WriteLine();
    chatHistory.AddAssistantMessage(content.ToString());
    Console.WriteLine();
}