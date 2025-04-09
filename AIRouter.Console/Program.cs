using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddUserSecrets<Program>()
    .Build();

//var kernel = Kernel.CreateBuilder().AddChatCompletion(configuration, "ollama").Build();

var services = new ServiceCollection();
services.RegisterKernels(configuration);
var sp = services.BuildServiceProvider();
var kernel = sp.GetRequiredKeyedService<Kernel>("zhipu");

var response = await kernel!.InvokePromptAsync("你是谁？");
Console.WriteLine(response.ToString());
