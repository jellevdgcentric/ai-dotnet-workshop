using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.SemanticKernel;
public class ListUsersPlugin
{
    private readonly HttpClient _httpClient;
    
    public ListUsersPlugin()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5472")
        };
    }
    
    [KernelFunction("get_users")]
    [Description("Gets a array of Users")]
    public async Task<User[]> GetUsersAsync()
    {
        var response = await _httpClient.GetAsync("/api/user");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<User[]>();
    }
}
