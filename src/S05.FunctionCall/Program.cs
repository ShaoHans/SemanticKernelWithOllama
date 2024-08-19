
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using S05.FunctionCall;

using System.Text;

var kernel = Kernel.CreateBuilder()
                .AddOllamaChatCompletion("llama3.1")
                .Build();

kernel.Plugins.AddFromType<TimePlugin>();

var executionSettings = new OpenAIPromptExecutionSettings
{
    Temperature = 0.1,
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

Console.WriteLine(await kernel.InvokePromptAsync("现在几点钟，请用中文回答?", new(executionSettings)));


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
    await foreach (var message in chatService.GetStreamingChatMessageContentsAsync(chatHistory, executionSettings, kernel))
    {
        Console.Write(message);
        content.Append(message.Content);
    }

    Console.WriteLine();
    chatHistory.AddAssistantMessage(content.ToString());
    Console.WriteLine();
}