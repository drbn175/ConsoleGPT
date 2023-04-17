﻿
using ConsoleGPT;
using Microsoft.Extensions.Configuration;

ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
IConfiguration configuration = configurationBuilder.AddUserSecrets<Program>().Build();

Console.WriteLine("Hello, Welcome to consoleGPT! Start a conversation:");

var openAIConfig = configuration.GetSection("OpenAI");
IOpenAIProxy chatOpenAI = new OpenAIProxy(
    apiKey: openAIConfig["ApiKey"],
    organizationId: openAIConfig["OrganizationId"]);

var msg = Console.ReadLine();

do
{
    var results = await chatOpenAI.SendChatMessage(msg);

    foreach (var item in results)
    {
        Console.WriteLine($"{item.Role}: {item.Content}");
    }

    Console.WriteLine("Next Prompt:");
    msg = Console.ReadLine();

} while (msg != "bye");