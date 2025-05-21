public class User
{
    public int Id { get; set; } // Unique identifier for the user
    public string FirstName { get; set; } // User's first name
    public string LastName { get; set; } // User's last name
    public string Email { get; set; } // User's email address
    public DateTime DateOfBirth { get; set; } // User's date of birth
    public string PhoneNumber { get; set; } // User's phone number
    public string Address { get; set; } // User's address
    public DateTime CreatedAt { get; set; } // Account creation timestamp
    public DateTime UpdatedAt { get; set; } // Last update timestamp
    public bool IsActive { get; set; } // Indicates if the user account is active
}
