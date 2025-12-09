using OpenAI.Chat;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;
using System.Text;

namespace LLMSandbox
{
    public class RequestCompletions
    {
        private readonly ChatClient _client;

        private readonly ChatMessage[] chatMessagePrompts =
        {
            "say this is a test",
            "Explain pythagorean theorem"
        };

        public RequestCompletions(string apiKey) 
        {
            _client = new ChatClient(apiKey: apiKey, model: "gpt-4o");
        }
        
        public async Task SingleRequestCompletion()
        {
            ChatCompletion completion = await _client.CompleteChatAsync(chatMessagePrompts[0]);

            Console.WriteLine($"[ASSISTANT]: {completion.Content[0].Text}");
        }

        public async Task StreamingRequestCompletion()
        {
            CollectionResult<StreamingChatCompletionUpdate> completionUpdates = _client.CompleteChatStreaming("Say 'this is a streaming test.'");

            Console.Write($"[ASSISTANT]: ");
            foreach (StreamingChatCompletionUpdate completionUpdate in completionUpdates)
            {
                if (completionUpdate.ContentUpdate.Count > 0)
                {
                    Console.Write(completionUpdate.ContentUpdate[0].Text);
                }
            }
        }
    }
}
