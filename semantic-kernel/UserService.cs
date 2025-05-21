using Refit;

public interface UserService
{
    [Get("/api/user")]
    Task<User[]> GetUsersAsync();

    [Post("/api/user")]
    Task<User> CreateUser(User user);
}
