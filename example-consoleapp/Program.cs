using Azure;
using Azure.AI.OpenAI;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using DotNetEnv;
using OpenAI.Chat;

// Load environment variables from .env file
Env.Load();

// Get configuration from environment variables
var model = Environment.GetEnvironmentVariable("AI_MODEL") ?? throw new Exception("AI_MODEL environment variable not set.");
var azureEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new Exception("AZURE_OPENAI_ENDPOINT environment variable not set.");
var apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY") ?? throw new Exception("AZURE_OPENAI_API_KEY environment variable not set.");

// Get search configuration from environment variables
var searchEndpoint = Environment.GetEnvironmentVariable("AZURE_SEMANTIC_SEARCH_ENDPOINT") ?? throw new Exception("AZURE_SEMANTIC_SEARCH_ENDPOINT environment variable not set.");
var searchApiKey = Environment.GetEnvironmentVariable("AZURE_SEMANTIC_SEARCH_API_KEY") ?? throw new Exception("AZURE_SEMANTIC_SEARCH_API_KEY environment variable not set.");
var iRacingVectorName = Environment.GetEnvironmentVariable("AZURE_SEMANTIC_SEARCH_IRACING_VECTOR_NAME") ?? throw new Exception("AZURE_SEMANTIC_SEARCH_IRACING_VECTOR_NAME environment variable not set.");
var bladesVectorName = Environment.GetEnvironmentVariable("AZURE_SEMANTIC_SEARCH_BLADES_VECTOR_NAME") ?? throw new Exception("AZURE_SEMANTIC_SEARCH_BLADES_VECTOR_NAME environment variable not set.");

Console.WriteLine("=== Blades in the Dark Assistant ===");
Console.WriteLine("Ask questions about the Blades in the Dark game. Type 'exit' to quit.");
Console.WriteLine();

AzureKeyCredential azureKeyCredential = new AzureKeyCredential(apiKey);
AzureOpenAIClient azureOpenAIClient = new AzureOpenAIClient(new Uri(azureEndpoint), azureKeyCredential);

ChatClient chatClient = azureOpenAIClient.GetChatClient(model);

// Semantic search setup
AzureKeyCredential semanticKeyCredential = new AzureKeyCredential(searchApiKey);
Uri searchUriEndpoint = new Uri(searchEndpoint);
SearchClient searchClient = new SearchClient(searchUriEndpoint, bladesVectorName, semanticKeyCredential);

bool isRunning = true;

while (isRunning)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write("You: ");
    Console.ForegroundColor = ConsoleColor.White;
    string input = Console.ReadLine() ?? "";
    
    if (input.ToLower() == "exit")
    {
        isRunning = false;
        continue;
    }

    // Semantic search
    SearchResults<SearchDocument> results = await searchClient.SearchAsync<SearchDocument>(input, new SearchOptions()
    {
        Size = 10
    });

    var resultTextList = results.GetResults().Select(result => result.Document["content"]).ToList();
    string resultText = string.Join("\n\n", resultTextList);

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("Search Results: ");
    Console.WriteLine(resultText);
    Console.WriteLine();

    // Response
    string formattedInput = "### User Query: " + input + "\n\n### Search Results: " + resultText;

    SystemChatMessage systemPrompt = new SystemChatMessage("You are a helpful Blades in the Dark assistant, " +
        "you are only allowed to respond with information that is provided in the Search Results.");
    UserChatMessage userPrompt = new UserChatMessage(formattedInput);

    ChatMessage[] chatMessages = new ChatMessage[] { systemPrompt, userPrompt };
    
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write("AI: ");
    Console.ForegroundColor = ConsoleColor.White;
    
    // Stream the response
    await foreach (StreamingChatCompletionUpdate update in chatClient.CompleteChatStreamingAsync(chatMessages))
    {
        foreach (ChatMessageContentPart part in update.ContentUpdate)
        {
            Console.Write(part.Text);
        }
    }
    
    Console.WriteLine();
    Console.WriteLine();
}

Console.WriteLine("Goodbye!");