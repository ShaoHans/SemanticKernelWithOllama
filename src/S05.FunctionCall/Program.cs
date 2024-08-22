using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using S05.FunctionCall;

#pragma warning disable SKEXP0001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。

var kernel = Kernel.CreateBuilder()
                .AddOllamaChatCompletion("llama3.1")
                //.AddOllamaChatCompletion("mistral")
                .Build();

kernel.Plugins.AddFromType<TimePlugin>();

var executionSettings = new OpenAIPromptExecutionSettings
{
    Temperature = 0.1,
    ToolCallBehavior = ToolCallBehavior.EnableKernelFunctions
};

var chatService = kernel.GetRequiredService<IChatCompletionService>();
var chatHistory = new ChatHistory("你是一位优秀的生活助理，会回答我提出的问题，同时可能会使用一些插件来协助回答问题");
chatHistory.AddUserMessage("现在时间是多少");

// ollmam暂不支持streaming tools call, https://ollama.com/blog/tool-support
var messageContent = await chatService.GetChatMessageContentAsync(chatHistory, executionSettings, kernel);

var functionCalls = FunctionCallContent.GetFunctionCalls(messageContent).ToArray();

while (functionCalls.Length != 0)
{
    chatHistory.Add(messageContent);

    foreach (var functionCall in functionCalls)
    {
        var r = await functionCall.InvokeAsync(kernel);

        chatHistory.Add(r.ToChatMessage());
    }

    messageContent = await chatService.GetChatMessageContentAsync(chatHistory, executionSettings, kernel);
    functionCalls = FunctionCallContent.GetFunctionCalls(messageContent).ToArray();
    Console.WriteLine(messageContent);
}
