using OpenAI;
using OpenAI.Chat;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace LLMSandbox
{
    public class SelfHostedLLM
    {
        private readonly HttpClient _httpClient;
        private string _modelName;
        public SelfHostedLLM(string modelName)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:11434") };
            _modelName = modelName;
        }

        public async Task SelfHostedLLMCompletion()
        {
            var request = new
            {
                model = _modelName,
                messages = new[]
                {
                    new { role = "user", content = "say hello this is a test" }
                }
            };

            var response = await _httpClient.PostAsJsonAsync("/api/chat", request);

            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);

            Console.Write("[offline assistant]:");

            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                var obj = JsonDocument.Parse(line);
                if (obj.RootElement.TryGetProperty("message", out var message))
                {
                    Console.Write(message.GetProperty("content").GetString());
                }
            }
        }

    }
}
