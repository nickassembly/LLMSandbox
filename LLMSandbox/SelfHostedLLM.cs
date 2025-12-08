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
        public ChatClient CreateSelfHostedClient(string modelName, string selfHostedEndpoint)
        {
            var openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? string.Empty;

            ChatClient offlineClient = new(
                model: modelName,
                credential: new ApiKeyCredential(openAiApiKey),
                options: new OpenAIClientOptions()
                {
                    Endpoint = new Uri("BASE_URL")
                }
            );

            return offlineClient;
        }

    }
}
