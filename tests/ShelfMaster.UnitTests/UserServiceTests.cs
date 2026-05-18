using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using ShelfMaster.Application.DTOs;
using ShelfMaster.Application.Interfaces;
using ShelfMaster.Application.Services;
using ShelfMaster.Domain.Entities;
using ShelfMaster.Domain.Exceptions;
using Xunit;

namespace ShelfMaster.UnitTests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _configurationMock = new Mock<IConfiguration>();

        _configurationMock
            .Setup(c => c["JwtSettings:Secret"])
            .Returns("SuperSecretDefaultKeyThatIsAtLeast32BytesLong!");

        _userService = new UserService(_userRepositoryMock.Object, _configurationMock.Object);
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldHashPasswordAndReturnToken_WhenUserIsValid()
    {
        // Arrange
        var dto = new UserRegisterDTO("ronit", "ronit@test.com", "SecurePassword123");
        
        _userRepositoryMock
            .Setup(r => r.GetUserByUsernameAsync(dto.Username))
            .ReturnsAsync((User?)null);
            
        _userRepositoryMock
            .Setup(r => r.GetUserByEmailAsync(dto.Email))
            .ReturnsAsync((User?)null);

        var result = await _userService.RegisterUserAsync(dto);

        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.User.Username.Should().Be("ronit");
        result.User.Email.Should().Be("ronit@test.com");
    
        _userRepositoryMock.Verify(r => r.AddUserAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task UserLoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
    
        var plaintextPassword = "MySecretPassword";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(plaintextPassword);
        var existingUser = new User("alex", "alex@test.com", hashedPassword);

        var loginDto = new UserLoginDTO("alex", plaintextPassword);

        _userRepositoryMock
            .Setup(r => r.GetUserByUsernameAsync(loginDto.Username))
            .ReturnsAsync(existingUser);

        
        var result = await _userService.UserLoginAsync(loginDto);

        
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.User.Username.Should().Be("alex");
    }

    [Fact]
    public async Task UserLoginAsync_ShouldThrowValidationException_WhenPasswordIsIncorrect()
    {
        
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("RealPassword123");
        var existingUser = new User("alex", "alex@test.com", hashedPassword);

        var loginDto = new UserLoginDTO("alex", "WrongPassword123");

        _userRepositoryMock
            .Setup(r => r.GetUserByUsernameAsync(loginDto.Username))
            .ReturnsAsync(existingUser);

        await Assert.ThrowsAsync<ValidationException>(() => 
            _userService.UserLoginAsync(loginDto));
    }
}