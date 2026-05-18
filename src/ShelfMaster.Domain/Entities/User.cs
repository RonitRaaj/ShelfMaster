namespace ShelfMaster.Domain.Entities;

public enum UserRole
{
    Admin,
    Manager,
    Staff
}
public class User
{
    public string Id { get; private set; }
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public UserRole Role { get; private set; }

    private User() 
    {
      Id = null!;
      Username = null!;
      Email = null!;
      PasswordHash = null!;
    }// For EF Core

    public User(string username, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty.", nameof(username));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty.", nameof(passwordHash));

            
        Id = Guid.NewGuid().ToString();
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        Role = UserRole.Staff;
    }

    public void UpdateUsername(string newUsername)
    {
        if (string.IsNullOrWhiteSpace(newUsername))
            throw new ArgumentException("Username cannot be empty.", nameof(newUsername));
        Username = newUsername;
    }

    public void UpdateEmail(string newEmail)
    {
        if (string.IsNullOrWhiteSpace(newEmail))
            throw new ArgumentException("Email cannot be empty.", nameof(newEmail));
        Email = newEmail;
    }

    public void UpdatePasswordHash(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash cannot be empty.", nameof(newPasswordHash));
        PasswordHash = newPasswordHash;
    }

    public void UpdateRole(UserRole newRole)
    {
        Role = newRole;
    }


}