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
var model = Environment.GetEnvironmentVariable("AI_MODEL") ?? "gpt-4o";
var azureEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? "";
var apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY") ?? "";

Console.WriteLine($"Model: {model}");
Console.WriteLine($"Azure Endpoint: {azureEndpoint}");
Console.WriteLine($"API Key: {apiKey[..6]}...");

// Get search configuration from environment variables
var searchEndpoint = Environment.GetEnvironmentVariable("AZURE_SEMANTIC_SEARCH_ENDPOINT") ?? "";
var searchApiKey = Environment.GetEnvironmentVariable("AZURE_SEMANTIC_SEARCH_API_KEY") ?? "";
var iRacingVectorName = Environment.GetEnvironmentVariable("AZURE_SEMANTIC_SEARCH_IRACING_VECTOR_NAME") ?? "";
var bladesVectorName = Environment.GetEnvironmentVariable("AZURE_SEMANTIC_SEARCH_BLADES_VECTOR_NAME") ?? "";

Console.WriteLine($"Search Endpoint: {searchEndpoint}");
Console.WriteLine($"Search API Key: {searchApiKey[..6]}...");
Console.WriteLine($"iRacing Vector Name: {iRacingVectorName}");
Console.WriteLine($"Blades in the Dark Vector Name: {bladesVectorName}");