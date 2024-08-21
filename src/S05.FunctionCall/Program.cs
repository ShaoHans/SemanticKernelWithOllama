
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using S05.FunctionCall;

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

while (true)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write("You:");
    chatHistory.AddUserMessage(Console.ReadLine()!);

    Console.ResetColor();
    Console.Write("Assistant:");

    var result = await chatService.GetChatMessageContentAsync(chatHistory, executionSettings, kernel);
    if (result.Content is not null)
    {
        Console.Write(result.Content);
        chatHistory.Add(result);
    }
    else
    {
        var functionCalls = FunctionCallContent.GetFunctionCalls(result);
        if (functionCalls.Any())
        {
            foreach (var functionCall in functionCalls)
            {
                var functionContent = await functionCall.InvokeAsync(kernel);
                Console.Write(functionContent.InnerContent);
                chatHistory.Add(functionContent.ToChatMessage());
            }
        }
    }
    
    Console.WriteLine();
}