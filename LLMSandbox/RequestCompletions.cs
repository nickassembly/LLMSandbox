using OpenAI.Assistants;
using OpenAI.Chat;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LLMSandbox
{
    public class RequestCompletions
    {
        private readonly ChatClient _client;
        public RequestCompletions(string apiKey)
        {
            _client = new ChatClient(apiKey: apiKey, model: "gpt-4o");
        }

        private readonly ChatMessage[] chatMessagePrompts =
        {
            "say this is a test",
            "Explain pythagorean theorem"
        };

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

        public async Task FunctionCallingRequest()
        {
            // Define a function(tool)
            var getWeatherTool = ChatTool.CreateFunctionTool(
                 functionName: "get_weather",
                 functionDescription: "Get the current weather for a city",
                 functionParameters: BinaryData.FromString("""
                        {
                          "type": "object",
                          "properties": {
                            "location": { "type": "string", "description": "City name, e.g. Dallas, TX" }
                          },
                          "required": ["location"]
                        }
                    """)
            );

            // chat message sent to tool
            var messages = new List<ChatMessage>
            {
                new UserChatMessage("What is the weather in Baton Rouge, LA right now?")
            };

            // define tools as a potential asset for LLM
            var options = new ChatCompletionOptions
            {
                Tools = { getWeatherTool }
            };

            // 1st Chat Completion call --> ask model (calling tool is not guarenteed, model is non-deterministic)
            ChatCompletion resp = await _client.CompleteChatAsync(messages, options);

            if (resp.ToolCalls.Count > 0)
            {
                // ToolCall was used for response and can be noted (and added to message history)
                var call = resp.ToolCalls[0];
                Console.WriteLine($"Model requested function: {call.FunctionName}");
                Console.WriteLine($"Arguments: {call.FunctionArguments}");

                // parse JSON args for function definition
                using var doc = JsonDocument.Parse(call.FunctionArguments);
                string city = doc.RootElement.GetProperty("location").GetString() ?? "";

                /* implement the behavior of get_weather here
                 * it could be a local method, web API call, database call
                 * here we define it as a placeholder string for demo purposes
                 * in production it would likely reference get_weather from a separate part of the code
                 * i.e. string weatherResult = WeatherApi.GetCurrentWeather(city);
                */
                string weatherResult = $"It is 72°F and sunny in {city}."; 

                // Add assistant's function‑call request to history
                var history = new List<ChatMessage>(messages)
                {
                    new AssistantChatMessage(resp)
                };

                // Add tool response message
                history.Add(new ToolChatMessage(call.Id, weatherResult));

                // 2nd ChatCompletion Call --> send function result back to model
                ChatCompletion final = await _client.CompleteChatAsync(history);

                Console.WriteLine("Final reply from assistant:");
                Console.WriteLine(final.Content[0].Text);
            }
            else
            {
                // ToolCall was not used in response, so we just output assistant content
                // **Note** the nature of LLMs are non-deterministic, so the tool call is not guarenteed
                Console.WriteLine(resp.Content[0].Text);
            }
        }

    }
}
