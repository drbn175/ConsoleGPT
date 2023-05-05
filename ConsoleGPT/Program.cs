
using ConsoleGPT;
using Microsoft.Extensions.Configuration;
using System;

ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
IConfiguration configuration = configurationBuilder.AddUserSecrets<Program>().Build();
IConfiguration appConfig = configurationBuilder.AddJsonFile("appsettings.json").Build();

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
var voiceConfig = appConfig.GetSection("Text2Speech");
var audioConfig = appConfig.GetSection("Speech2Text");
var voice = voiceConfig["Voice"];
Console.ForegroundColor = ConsoleColor.Blue;
Console.WriteLine($"Model: {model}");
Console.WriteLine($"Temperature: {temperature}");
Console.WriteLine($"Max tokens: {maxTokens}");
Console.WriteLine($"Voice: {voice}");
Console.ForegroundColor = ConsoleColor.Yellow;


Console.ForegroundColor = ConsoleColor.Gray;
Console.WriteLine("To leave write 'bye'");
Console.WriteLine("To use speech recognition write 'listen'");
Console.WriteLine("Start a conversation");
Console.WriteLine(" █>User:");

var msg = Console.ReadLine();
while (msg != "bye")
{
    if(msg == "listen")
    {
        msg = await Speech2Text.RecognizeSpeechAsync(text2SpeechConfig["SubscriptionKey"], text2SpeechConfig["Region"], audioConfig["Voice"]);
        WriteLineWordWrap.WriteLine(msg);
    }
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
                results[0].Content = ex.Message;
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



