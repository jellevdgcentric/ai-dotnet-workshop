using Azure.AI.OpenAI;
using OpenAI.Chat;

const string endpoint = "https://azureaiservicesworkshop.openai.azure.com/";
const string apiKey = "4TkMyWgSeqKZkSgONCZAxLhndWkSqKxXkubZkcf8BRUbT0vq41aoJQQJ99BAACfhMk5XJ3w3AAAAACOGyExw";
Uri endpointUri = new Uri(endpoint);

AzureOpenAIClient client = new AzureOpenAIClient(endpointUri, new Azure.AzureKeyCredential(apiKey), new AzureOpenAIClientOptions());
ChatClient chatClient = client.GetChatClient("gpt-4.1");

ChatCompletion response = await chatClient.CompleteChatAsync(new UserChatMessage("Hello, how are you?"));
string[] responseTextArray = response.Content.Select(m => m.Text).ToArray();
string responseText = string.Join(", ", responseTextArray);
Console.WriteLine(responseText);

Console.ReadLine();
