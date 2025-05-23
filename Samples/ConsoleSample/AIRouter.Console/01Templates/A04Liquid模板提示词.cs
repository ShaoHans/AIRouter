﻿using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.PromptTemplates.Liquid;

namespace AIRouter.Console.Templates;

internal class A04Liquid模板提示词
{
    public static async Task TestAsync(Kernel kernel)
    {
        var promptTemplateConfig = new PromptTemplateConfig()
        {
            Template = """
                    <message role="system">根据用户的提问，说出用户想去哪里，从下面给出的选项列表中选择一个最合适的选项，不需要给出理由，
                    如果不知道答案，就选择第一个选项。
                    选项列表: {{choices}}
                    </message>
                    {% for item in fewShotExamples %}
                        <message role="{{item.role}}">{{item.content}}</message>
                    {% endfor %}

                    {% for item in history %}
                        <message role="{{item.role}}">{{item.content}}</message>
                    {% endfor %}
                    <message role="user">{{request}}</message>
                """,
            TemplateFormat = "liquid"
        };

        List<string> choices = ["不知道", "KTV", "电影院", "餐厅", "棋牌室"];
        var fewShotExamples = new[]
        {
            new { role = "user", content = "最近有什么好看的电影推荐吗?" },
            new { role = "assistant", content = "电影院" },
            new { role = "user", content = "肚子饿了，想去吃点好吃的?" },
            new { role = "assistant", content = "餐厅" },
        };

        ChatHistory history = [];

        KernelArguments kernelArguments =
            new()
            {
                { "choices", choices },
                { "chatHistory", history },
                { "fewShotExamples", fewShotExamples }
            };

        var factory = new LiquidPromptTemplateFactory();
        var promptTemplate = factory.Create(promptTemplateConfig);
        var renderedPrompt = await promptTemplate.RenderAsync(kernel, kernelArguments);
        System.Console.WriteLine(renderedPrompt);

        System.Console.WriteLine("请输入你想说的：");
        var request = System.Console.ReadLine();
        kernelArguments["request"] = request;

        var function = kernel.CreateFunctionFromPrompt(promptTemplateConfig, factory);
        var result = await kernel.InvokeAsync(function, kernelArguments);
        history.AddUserMessage(request!);
        history.AddAssistantMessage(result.ToString());
        System.Console.WriteLine(result.ToString());
    }
}
