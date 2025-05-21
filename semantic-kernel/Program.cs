// See https://aka.ms/new-console-template for more information
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Spectre.Console;
using DotNetEnv;

Console.WriteLine("Hello, World!");

// Load environment variables from .env file
Env.Load();

// Get configuration from environment variables
var model = Environment.GetEnvironmentVariable("AI_MODEL") ?? throw new ArgumentNullException("AI_MODEL", "AI_MODEL is not set");
var azureEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new ArgumentNullException("AZURE_OPENAI_ENDPOINT", "AZURE_OPENAI_ENDPOINT is not set");
var apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY") ?? throw new ArgumentNullException("AZURE_OPENAI_API_KEY", "AZURE_OPENAI_API_KEY is not set");

// Create kernel
var builder = Kernel.CreateBuilder();

builder.AddAzureOpenAIChatCompletion(model, azureEndpoint, apiKey);
var kernel = builder.Build();


var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// Add a plugin (the LightsPlugin class is defined below)
kernel.Plugins.AddFromType<LightsPlugin>("Lights");

kernel.Plugins.AddFromType<ListUsersPlugin>("ListUsers");
kernel.Plugins.AddFromType<CreateUserPlugin>("CreateUser");

// kernel.Plugins.AddFromType<GetDateTimePlugin>("GetDateTime");

// Enable planning
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

// Create a history store the conversation
var history = new ChatHistory();

// Initiate a back-and-forth chat
string? userInput;
do
{
    // Collect user input
    AnsiConsole.Markup("[green]User > [/]");
    userInput = Console.ReadLine();

    if (string.IsNullOrEmpty(userInput) || userInput.Trim().ToLower() == "exit")
    {
        break; // Exit the loop if the user input is empty
    }

    // Add user input
    history.AddUserMessage(userInput);

    // Get the response from the AI
    var result = await chatCompletionService.GetChatMessageContentAsync(
        history,
        executionSettings: openAIPromptExecutionSettings,
        kernel: kernel);

    // Print the results
    Console.WriteLine("Assistant > " + result);
    Console.WriteLine(); // Add an empty line below the assistant's message

    // Add the message from the agent to the chat history
    history.AddMessage(result.Role, result.Content ?? string.Empty);
} while (string.IsNullOrEmpty(userInput) == false);
