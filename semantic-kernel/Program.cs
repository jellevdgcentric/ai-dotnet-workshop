// See https://aka.ms/new-console-template for more information
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Spectre.Console;
using DotNetEnv;

// Load environment variables from .env file
Env.Load();

// Get configuration from environment variables
var model = Environment.GetEnvironmentVariable("AI_MODEL") ?? throw new ArgumentNullException("AI_MODEL", "AI_MODEL is not set");
var azureEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new ArgumentNullException("AZURE_OPENAI_ENDPOINT", "AZURE_OPENAI_ENDPOINT is not set");
var apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY") ?? throw new ArgumentNullException("AZURE_OPENAI_API_KEY", "AZURE_OPENAI_API_KEY is not set");

// Create first kernel for generating poem ideas
var builder1 = Kernel.CreateBuilder();
builder1.AddAzureOpenAIChatCompletion(model, azureEndpoint, apiKey);
var ideaKernel = builder1.Build();
var ideaService = ideaKernel.GetRequiredService<IChatCompletionService>();
var ideaHistory = new ChatHistory();

// Create second kernel for writing the poem
var builder2 = Kernel.CreateBuilder();
builder2.AddAzureOpenAIChatCompletion(model, azureEndpoint, apiKey);
var poemKernel = builder2.Build();
var poemService = poemKernel.GetRequiredService<IChatCompletionService>();
var poemHistory = new ChatHistory();

// Main loop
while (true)
{
    // Get user question
    AnsiConsole.MarkupLine("[green]Ask a question for the poem:[/]");
    var userQuestion = Console.ReadLine() ?? string.Empty;
    
    if (string.IsNullOrEmpty(userQuestion))
        continue;
    
    if (userQuestion.ToLower() == "exit")
        break;
    
    // Show first kernel is working
    AnsiConsole.MarkupLine("[red]Agent 1 is generating poem ideas...[/]");
    
    // Generate poem ideas using the first kernel
    var poemIdeas = await GeneratePoemIdeas(ideaService, ideaHistory, userQuestion);
    
    // Display the ideas from the first kernel
    AnsiConsole.MarkupLine("[red]Agent 1 generated these ideas:[/]");
    AnsiConsole.MarkupLine($"[red]{poemIdeas}[/]");
    AnsiConsole.WriteLine();
    
    // Show second kernel is working
    AnsiConsole.MarkupLine("[red]Agent 2 is composing the final poem based on these ideas...[/]");
    
    // Generate the final poem using the second kernel
    var poem = await GeneratePoem(poemService, poemHistory, userQuestion, poemIdeas);
    
    // Display the poem
    AnsiConsole.MarkupLine("[blue]Here's your poem:[/]");
    AnsiConsole.WriteLine(poem);
    AnsiConsole.WriteLine();
    
    // Show the collaboration summary
    AnsiConsole.MarkupLine("[red]The two agents worked together: Agent 1 brainstormed ideas, and Agent 2 crafted them into a poem.[/]");
    AnsiConsole.WriteLine();
}

// Method to generate poem ideas
async Task<string> GeneratePoemIdeas(IChatCompletionService service, ChatHistory history, string question)
{
    history.AddUserMessage($"Generate 4 distinct themes or ideas for a poem about: {question}. Be creative and thoughtful.");
    
    var response = await service.GetChatMessageContentAsync(history);
    var ideas = response.Content ?? "No ideas generated";
    
    history.AddAssistantMessage(ideas);
    return ideas;
}

// Method to generate the poem
async Task<string> GeneratePoem(IChatCompletionService service, ChatHistory history, string question, string ideas)
{
    history.AddUserMessage($"Based on these ideas: {ideas}\n\nWrite a poem about '{question}'. The poem should have exactly 4 sections, and must include the phrase 'AI is the future' somewhere in the poem. Make it creative and emotional.");
    
    var response = await service.GetChatMessageContentAsync(history);
    var poem = response.Content ?? "Could not generate a poem";
    
    history.AddAssistantMessage(poem);
    return poem;
}
