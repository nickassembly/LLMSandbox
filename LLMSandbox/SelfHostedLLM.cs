using OpenAI;
using OpenAI.Chat;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Text;

namespace LLMSandbox
{
    public class SelfHostedLLM
    {
        // handle custom calls for self-hosted LLMs that are compatible with OpenAI API 
        private readonly ChatClient _client;
        public SelfHostedLLM(string apiKey, string modelName)
        {
            _client = new ChatClient(
                model: modelName,
                credential: new ApiKeyCredential(apiKey),
                options: new OpenAIClientOptions()
                {
                    Endpoint = new Uri("BASE_URL")
                }
            );
        }

        private readonly ChatMessage[] localMessagePrompts =
        {
            "what is my name",
            "what are some of my hobbies",
            "who are my siblings and parents"
        };

        public async Task SelfHostedLLMCompletion()
        {
            ChatCompletion completion = await _client.CompleteChatAsync(localMessagePrompts[0]);

            Console.WriteLine($"[Self-Hosted Assistant]: {completion.Content[0].Text}");
        }



    }
}
