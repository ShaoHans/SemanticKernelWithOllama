using Microsoft.SemanticKernel;

using System.ComponentModel;

namespace S05.FunctionCall;

public class TimePlugin
{
    [KernelFunction]
    [Description("获取当前系统时间")]
    public string Now()
    {
        return $"现在是北京时间：{DateTime.Now}";
    }
}
