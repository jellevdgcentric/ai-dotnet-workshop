using Azure;
using Azure.AI.OpenAI;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using DotNetEnv;
using OpenAI.Chat;

// Load environment variables from .env file
Env.Load();

Console.WriteLine("Hello, World!");

// Get configuration from environment variables
var model = Environment.GetEnvironmentVariable("AI_MODEL") ?? throw new Exception("AI_MODEL environment variable not set.");
var azureEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new Exception("AZURE_OPENAI_ENDPOINT environment variable not set.");
var apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY") ?? throw new Exception("AZURE_OPENAI_API_KEY environment variable not set.");

Console.WriteLine($"Model: {model}");
Console.WriteLine($"Azure Endpoint: {azureEndpoint}");
Console.WriteLine($"API Key: {apiKey[..6]}...");

// Get search configuration from environment variables
var searchEndpoint = Environment.GetEnvironmentVariable("AZURE_SEMANTIC_SEARCH_ENDPOINT") ?? throw new Exception("AZURE_SEMANTIC_SEARCH_ENDPOINT environment variable not set.");
var searchApiKey = Environment.GetEnvironmentVariable("AZURE_SEMANTIC_SEARCH_API_KEY") ?? throw new Exception("AZURE_SEMANTIC_SEARCH_API_KEY environment variable not set.");
var iRacingVectorName = Environment.GetEnvironmentVariable("AZURE_SEMANTIC_SEARCH_IRACING_VECTOR_NAME") ?? throw new Exception("AZURE_SEMANTIC_SEARCH_IRACING_VECTOR_NAME environment variable not set.");
var bladesVectorName = Environment.GetEnvironmentVariable("AZURE_SEMANTIC_SEARCH_BLADES_VECTOR_NAME") ?? throw new Exception("AZURE_SEMANTIC_SEARCH_BLADES_VECTOR_NAME environment variable not set.");

Console.WriteLine($"Search Endpoint: {searchEndpoint}");
Console.WriteLine($"Search API Key: {searchApiKey[..6]}...");
Console.WriteLine($"iRacing Vector Name: {iRacingVectorName}");
Console.WriteLine($"Blades in the Dark Vector Name: {bladesVectorName}");

AzureKeyCredential azureKeyCredential = new AzureKeyCredential(apiKey);
AzureOpenAIClient azureOpenAIClient = new AzureOpenAIClient(new Uri(azureEndpoint), azureKeyCredential);

ChatClient chatClient = azureOpenAIClient.GetChatClient(model);

string input = Console.ReadLine() ?? "";


// Semantic search
AzureKeyCredential semanticKeyCredential = new AzureKeyCredential(searchApiKey);
Uri searchUriEndpoint = new Uri(searchEndpoint);
SearchClient searchClient = new SearchClient(searchUriEndpoint, bladesVectorName, semanticKeyCredential);
SearchResults<SearchDocument> results = await searchClient.SearchAsync<SearchDocument>(input, new SearchOptions()
{
    Size = 10
});

var resultTextList = results.GetResults().Select(result => result.Document["content"]).ToList();
string resultText = string.Join("\n\n", resultTextList);

Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("Search Results:" + resultText);

// Response
string formattedInput = "### User Query: " + input + "\n\n### Search Results: " + resultText;

SystemChatMessage systemPrompt = new SystemChatMessage("You are a helpful Blades in the Dark assistant, " +
    "you are only allowed to respond with information that is provided in the Search Results.");
UserChatMessage userPrompt = new UserChatMessage(formattedInput);

ChatMessage[] chatMessages = new ChatMessage[] { systemPrompt, userPrompt };
ChatCompletion chatCompletion = await chatClient.CompleteChatAsync(chatMessages);
string[] responseArray = chatCompletion.Content.Select(completion => completion.Text).ToArray();

Console.ForegroundColor = ConsoleColor.White;
Console.WriteLine($"Response: {string.Join(", ", responseArray)}");
