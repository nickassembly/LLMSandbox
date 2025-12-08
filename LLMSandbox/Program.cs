using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

// Sandbox for calling LLM Functions & experimenting with tokens and embeddings
// using open ai 2.7.0 sdk
// https://github.com/openai/openai-dotnet?tab=readme-ov-file

// Chat completion calls
// Function calls
// Embedding requests
// Streaming responses

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var vaultUri = configuration["AzureKeyVault:VaultUri"] ?? "";

// uses service to call factory method to create creds with a device code fallback
// typically for web apps DefaultAzureCredential is sufficient but the aim here is to not rely on web infrastrctural patterns
// since this is a console app
var keyVault = new KeyVaultService(vaultUri);

// get open ai key and set as environment variable for consistency;
var openAiKey = await keyVault.GetSecretAsync("ApiKeys--OpenAI");

// set environment variable for call consistency
// TODO*** - env should get populated in SelfHostedLLM in order to set up client 
Environment.SetEnvironmentVariable("OPENAI_API_KEY", openAiKey);

// Test AI Prompts for Open AI SDK
ChatClient client = new(model: "gpt-4o", apiKey: openAiKey);
ChatCompletion completion = client.CompleteChat("Say 'this is a test.'");
Console.WriteLine($"[ASSISTANT]: {completion.Content[0].Text}");

// ChatMessage[] -- param for CompleteChat



