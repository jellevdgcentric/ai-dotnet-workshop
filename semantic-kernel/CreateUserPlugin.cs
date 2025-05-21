using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.SemanticKernel;

public class CreateUserPlugin
{
    private readonly HttpClient _httpClient;
    
    public CreateUserPlugin()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5472")
        };
    }
    
    [KernelFunction("create_user")]
    [Description("Creates a single user")]
    public async Task<User> GetUsersAsync(
     [Description("The physical address of the user")] string address,
     [Description("The user's date of birth in MM/DD/YYYY format")] DateTime dateOfBirth,
     [Description("The email address of the user")] string email,
     [Description("The user's first name")] string firstName,
     [Description("The user's last name")] string lastName,
     [Description("The user's phone number including country code")] string phoneNumber
     )    {
        var user = new User()
        {
            Address = address,
            CreatedAt = DateTime.Now,
            DateOfBirth = dateOfBirth,
            Email = email,
            FirstName = firstName,
            IsActive = true,
            LastName = lastName,
            PhoneNumber = phoneNumber,
            UpdatedAt = DateTime.Now
        };

        var response = await _httpClient.PostAsJsonAsync("/api/user", user);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<User>();
    }
}
