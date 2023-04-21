using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;

namespace ConsoleGPT
{
    public static class Text2Speech
    {
        public static async Task SynthesisToSpeakerAsync(string subscriptionKey, string region, string voice, string text)
        {
            // To support Chinese Characters on Windows platform
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Console.InputEncoding = System.Text.Encoding.Unicode;
                Console.OutputEncoding = System.Text.Encoding.Unicode;
            }


            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key and service region (e.g., "westus").
            var config = SpeechConfig.FromSubscription(subscriptionKey, region);

            // Set the voice name, refer to https://aka.ms/speech/voices/neural for full list.
            config.SpeechSynthesisVoiceName = voice;


            // Creates a speech synthesizer using the default speaker as audio output.
            using (var synthesizer = new SpeechSynthesizer(config))
            {
                var result = await synthesizer.SpeakTextAsync(text);
            }
        }
    }
}
