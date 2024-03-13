
using ConsoleGPT;
using Microsoft.Extensions.Configuration;
using System;

ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
IConfiguration configuration = configurationBuilder.AddUserSecrets<Program>().Build();
IConfiguration appConfig = configurationBuilder.AddJsonFile("appsettings.json").Build();

Console.WriteLine("Hello, Welcome to consoleGPT!");
var model = "gpt-4-turbo-preview"; // (gpt-3.5-turbo,gpt-4,gpt-4-turbo-preview) 
var temperature = new Random().NextDouble();
var maxTokens = 2000;

var openAIConfig = configuration.GetSection("OpenAI");
var apiKey = openAIConfig["ApiKey"];
var organizationId = openAIConfig["OrganizationId"];
if (apiKey != null && organizationId != null)
{
    IOpenAIProxy chatOpenAI = new OpenAIProxy(
        apiKey: apiKey,
        organizationId: organizationId,
        model,
        temperature,
        maxTokens);
    var text2SpeechConfig = configuration.GetSection("Text2Speech");
    var voiceConfig = appConfig.GetSection("Text2Speech");
    var audioConfig = appConfig.GetSection("Speech2Text");
    var t2sVoice = voiceConfig["Voice"];
    Console.ForegroundColor = ConsoleColor.Blue;
    Console.WriteLine($"Model: {model}");
    Console.WriteLine($"Temperature: {temperature}");
    Console.WriteLine($"Max tokens: {maxTokens}");
    Console.WriteLine($"Voice: {t2sVoice}");
    Console.ForegroundColor = ConsoleColor.Yellow;


    Console.ForegroundColor = ConsoleColor.Gray;
    Console.WriteLine("To leave write 'bye'");
    Console.WriteLine("To use speech recognition write 'listen'");
    Console.WriteLine("Start a conversation");
    Console.WriteLine(" █>User:");

    var subscriptionKey = text2SpeechConfig["SubscriptionKey"];
    var region = text2SpeechConfig["Region"];
    var s2tVoice = audioConfig["Voice"];

    var msg = Console.ReadLine();
    while (msg != "bye")
    {
        if (msg == "listen")
        {
            if(subscriptionKey != null && region != null && s2tVoice != null)
            {

                msg = await Speech2Text.RecognizeSpeechAsync(subscriptionKey, region, s2tVoice);
                WriteLineWordWrap.WriteLine(msg);
            }
            else
            {
                Console.WriteLine("Text2Speech configuration missing!");
            }
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
                    results = new Standard.AI.OpenAI.Models.Services.Foundations.ChatCompletions.ChatCompletionMessage[1];
                    results[0] = new Standard.AI.OpenAI.Models.Services.Foundations.ChatCompletions.ChatCompletionMessage { Content = ex.Message, Role = "SYSTEM" };
                }
                spinner.Stop();

                Console.ForegroundColor = ConsoleColor.Yellow;
                foreach (var item in results)
                {
                    Task? task = null;
                    if (subscriptionKey != null && region != null && t2sVoice != null)
                    {
                        task = Text2Speech.SynthesisToSpeakerAsync(subscriptionKey, region, t2sVoice, item.Content);
                    }
                    Console.WriteLine($"█>{item.Role}:");
                    lines += WriteLineWordWrap.WriteLine(item.Content);

                    if (task != null)
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
}
else
{
    Console.WriteLine("ApiKey or OrganizationId missing!");
}


