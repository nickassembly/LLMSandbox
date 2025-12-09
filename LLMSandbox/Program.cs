using LLMSandbox;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

// Sandbox for calling LLM Functions & experimenting with tokens and embeddings
// using open ai 2.7.0 sdk
// https://github.com/openai/openai-dotnet?tab=readme-ov-file

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var vaultUri = configuration["AzureKeyVault:VaultUri"] ?? "";

// uses service to call factory method to create creds with a device code fallback
// typically for web apps DefaultAzureCredential is sufficient but the aim here is to not rely on web infrastrctural patterns
var keyVault = new KeyVaultService(vaultUri);

// get open ai key and set as environment variable for consistency;
var openAiKey = await keyVault.GetSecretAsync("ApiKeys--OpenAI") ?? string.Empty;
Environment.SetEnvironmentVariable("OPENAI_API_KEY", openAiKey, EnvironmentVariableTarget.Process);

// chat response service
var chatService = new RequestCompletions(openAiKey);

// self hosted services
// var selfHostedLLMService = new SelfHostedLLM(openAiKey, "model placeholder");

// embeddings service (SDK has separate model for embeddings use)
var embeddingService = new Embeddings(openAiKey);

// simple request
// await chatService.SingleRequestCompletion();

// streaming request -- use in most cases unless the request is very simple and not real time chat
// await chatService.StreamingRequestCompletion();

// function calling
   await chatService.FunctionCallingRequest();

// embeddings example (Output is large vectors of floats)
// await embeddingService.CreateMultipleEmbeddings();
