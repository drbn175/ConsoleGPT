
using ConsoleGPT;
using Microsoft.Extensions.Configuration;
using System;

ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
IConfiguration configuration = configurationBuilder.AddUserSecrets<Program>().Build();

Console.WriteLine("Hello, Welcome to consoleGPT!");
var model = "gpt-3.5-turbo";
var temperature = 0.2;
var maxTokens = 800;

var openAIConfig = configuration.GetSection("OpenAI");
IOpenAIProxy chatOpenAI = new OpenAIProxy(
    apiKey: openAIConfig["ApiKey"],
    organizationId: openAIConfig["OrganizationId"],
    model,
    temperature,
    maxTokens);
Console.ForegroundColor = ConsoleColor.Blue;
Console.WriteLine($"Model: {model}");
Console.WriteLine($"Temperature: {temperature}");
Console.WriteLine($"Max tokens: {maxTokens}");

                    

Console.ForegroundColor = ConsoleColor.Gray;
Console.WriteLine("Start a conversation (To leave write 'bye')");
Console.WriteLine(" █>User:");

var msg = Console.ReadLine();
while (msg != "bye")
{
    if (msg != null || !string.IsNullOrEmpty(msg))
    {
        var lines = 1;
        using (var spinner = new Spinner(0, Console.CursorTop))
        {
            spinner.Start();
            var results = await chatOpenAI.SendChatMessage(msg);
            spinner.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (var item in results)
            {
                Console.WriteLine($"█>{item.Role}:");
                Console.WriteLine(item.Content);
                lines += item.Content.Split('\n').Length + item.Content.Split('\n').Where(t => t.Length > Console.WindowWidth).Count();
            }
        }

        Console.ForegroundColor = ConsoleColor.Gray;
        Console.SetCursorPosition(0, Console.CursorTop + lines + 2);
        Console.WriteLine(" █>User:");

        msg = Console.ReadLine();
    }
    else
    {
        Console.WriteLine("Invalid prompt! Try again");
        Console.WriteLine(" █>User:");
    }

};

