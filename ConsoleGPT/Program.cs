
using ConsoleGPT;
using Microsoft.Extensions.Configuration;
using System;

ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
IConfiguration configuration = configurationBuilder.AddUserSecrets<Program>().Build();

Console.WriteLine("Hello, Welcome to consoleGPT!");
var model = "gpt-3.5-turbo";
var temperature = 0.2;
var maxTokens = 2000;

var openAIConfig = configuration.GetSection("OpenAI");
IOpenAIProxy chatOpenAI = new OpenAIProxy(
    apiKey: openAIConfig["ApiKey"],
    organizationId: openAIConfig["OrganizationId"],
    model,
    temperature,
    maxTokens);
var text2SpeechConfig = configuration.GetSection("Text2Speech");

Console.ForegroundColor = ConsoleColor.Blue;
Console.WriteLine($"Model: {model}");
Console.WriteLine($"Temperature: {temperature}");
Console.WriteLine($"Max tokens: {maxTokens}");
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("If you want to use text to speech write the neural voice. Refers to https://learn.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support?tabs=tts#prebuilt-neural-voices");
var voice = Console.ReadLine();

Console.ForegroundColor = ConsoleColor.Gray;
Console.WriteLine("Start a conversation (To leave write 'bye')");
Console.WriteLine(" █>User:");

var msg = Console.ReadLine();
while (msg != "bye")
{
    if (msg != null || !string.IsNullOrEmpty(msg))
    {
        var lines = 0;
        using (var spinner = new Spinner(0, Console.CursorTop))
        {
            spinner.Start();
            Standard.AI.OpenAI.Models.Services.Foundations.ChatCompletions.ChatCompletionMessage[] results = new Standard.AI.OpenAI.Models.Services.Foundations.ChatCompletions.ChatCompletionMessage[lines];
            try
            {
                results = await chatOpenAI.SendChatMessage(msg);
            }
            catch (Exception ex)
            {
                results = new Standard.AI.OpenAI.Models.Services.Foundations.ChatCompletions.ChatCompletionMessage[1];
                results[0] = new Standard.AI.OpenAI.Models.Services.Foundations.ChatCompletions.ChatCompletionMessage { Content = ex.Message, Role = "SYSTEM" };
            }
            spinner.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (var item in results)
            {
                Task? task= null;
                if (!string.IsNullOrEmpty(voice))
                {
                    task = Text2Speech.SynthesisToSpeakerAsync(text2SpeechConfig["SubscriptionKey"], text2SpeechConfig["Region"], voice, item.Content);
                }
                Console.WriteLine($"█>{item.Role}:");
                lines += WriteLineWordWrap.WriteLine(item.Content);
                
                if(task!= null)
                    await Task.WhenAll(task);

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



