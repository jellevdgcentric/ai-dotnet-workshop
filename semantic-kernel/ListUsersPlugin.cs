using System.ComponentModel;
using Microsoft.SemanticKernel;
public class ListUsersPlugin
{
    [KernelFunction("get_users")]
    [Description("Gets a array of Users")]
    public Task<User[]> GetUsersAsync()
    {
        var api = Refit.RestService.For<UserService>("https://localhost:7311");
        return api.GetUsersAsync();
    }
}
