using System.ComponentModel;
using Microsoft.SemanticKernel;

public class CreateUserPlugin
{
    [KernelFunction("create_user")]
    [Description("Creates a single user")]
    public async Task<User> GetUsersAsync(
     [Description("The physical address of the user")] string address,
     [Description("The user's date of birth in MM/DD/YYYY format")] DateTime dateOfBirth,
     [Description("The email address of the user")] string email,
     [Description("The user's first name")] string firstName,
     [Description("The user's last name")] string lastName,
     [Description("The user's phone number including country code")] string phoneNumber
     )
    {
        var api = Refit.RestService.For<UserService>("https://localhost:7311");
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

        var createdUser = await api.CreateUser(user);
        return createdUser;
    }
}
