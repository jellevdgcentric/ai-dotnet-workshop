using DotNetEnv;

// Load environment variables from .env file
Env.Load();

Console.WriteLine("Hello, World!");

// Get configuration from environment variables
var model = Environment.GetEnvironmentVariable("AI_MODEL") ?? "gpt-4o";
var azureEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? "";
var apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY") ?? "";

Console.WriteLine($"Model: {model}");
Console.WriteLine($"Azure Endpoint: {azureEndpoint}");
Console.WriteLine($"API Key: {apiKey[..6]}...");


