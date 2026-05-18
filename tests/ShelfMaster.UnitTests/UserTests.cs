using FluentAssertions;
using ShelfMaster.Domain.Entities;
using System;
using Xunit;

namespace ShelfMaster.UnitTests;

public class UserDomainTests
{
    [Fact]
    public void Constructor_ShouldCreateUserSuccessfully_WhenDataIsValid()
    {
        // Arrange
        string username = "ronit";
        string email = "ronit@test.com";
        string passwordHash = "SomeCryptographicHash123";

        // Act
        var user = new User(username, email, passwordHash);

        // Assert
        user.Id.Should().NotBeNullOrEmpty();
        user.Username.Should().Be(username);
        user.Email.Should().Be(email);
        user.PasswordHash.Should().Be(passwordHash);
        user.Role.Should().Be(UserRole.Staff);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_ShouldThrowArgumentException_WhenUsernameIsEmpty(string? invalidUsername)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            new User(invalidUsername!, "test@email.com", "hash123"));
            
        exception.Message.Should().Contain("Username cannot be empty.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_ShouldThrowArgumentException_WhenEmailIsEmpty(string? invalidEmail)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            new User("username", invalidEmail!, "hash123"));


        exception.Message.Should().Contain("Email cannot be empty.");
    }

    [Fact]
    public void UpdateEmail_ShouldChangeEmail_WhenNewEmailIsValid()
    {
        // Arrange
        var user = new User("username", "old@email.com", "hash123");

        // Act
        user.UpdateEmail("NEW@email.com");

        // Assert
        user.Email.Should().Be("NEW@email.com"); 
    }

    [Fact]
    public void UpdateRole_ShouldChangeRoleSuccessfully()
    {
        // Arrange
        var user = new User("username", "test@email.com", "hash123");

        // Act
        user.UpdateRole(UserRole.Admin);

        // Assert
        user.Role.Should().Be(UserRole.Admin);
    }

    
}