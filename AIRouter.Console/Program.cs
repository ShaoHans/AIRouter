using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddUserSecrets<Program>()
    .Build();

var kernel = Kernel.CreateBuilder().AddChatCompletion(configuration, "ollama").Build();
var response = await kernel.InvokePromptAsync("你是谁？");
Console.WriteLine(response.ToString());
