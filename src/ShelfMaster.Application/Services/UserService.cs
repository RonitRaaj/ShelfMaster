using ShelfMaster.Application.DTOs;
using ShelfMaster.Application.Interfaces;
using ShelfMaster.Domain.Exceptions;
using ShelfMaster.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;

namespace ShelfMaster.Application.Services;

public class UserService
{
    private readonly IUserRepository _repository;
    private readonly IConfiguration _configuration;

    public UserService(IUserRepository repository, IConfiguration configuration)
    {
        _repository = repository;
        _configuration = configuration;
    }

    public async Task<UserAuthwnticationDTO> RegisterUserAsync(UserRegisterDTO dto)
    {
        var userWithSameUsername = await _repository.GetUserByUsernameAsync(dto.Username);
        if (userWithSameUsername != null) throw new ValidationException($"A user with username {dto.Username} already exists.");

        var userWithSameEmail = await _repository.GetUserByEmailAsync(dto.Email);
        if (userWithSameEmail != null) throw new ValidationException($"A user with email {dto.Email} already exists.");

        string workFactorHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var user = new User(dto.Username, dto.Email, workFactorHash);
        await _repository.AddUserAsync(user);

        var token = await GenerateJwtTokenAsync(MapToResponseDto(user));
        return new UserAuthwnticationDTO(token, MapToResponseDto(user));
    }

    public async Task<UserResponseDTO?> GetUserByIdAsync(string id)
    {
        var user = await _repository.GetUserByIdAsync(id);
        if (user == null) throw new NotFoundException($"User with ID {id} not found");
        return MapToResponseDto(user);
    }

    public async Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync()
    {
        var users = await _repository.GetAllUsersAsync();
        return users.Select(MapToResponseDto);
    }

    public async Task<UserAuthwnticationDTO> UserLoginAsync(UserLoginDTO dto)
    {
        var user = await _repository.GetUserByUsernameAsync(dto.Username);
        if (user == null) throw new ValidationException("Invalid username or password");

        bool passwordMatches = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!passwordMatches) throw new ValidationException("Invalid username or password");

        var token = await GenerateJwtTokenAsync(MapToResponseDto(user));

        return new UserAuthwnticationDTO(token, MapToResponseDto(user));
    }

    public async Task<string> GenerateJwtTokenAsync(UserResponseDTO user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"] ?? "SuperSecretDefaultKeyThatIsAtLeast32BytesLong!");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString()) // Critical for Role-Based Access Control!
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7), // Token valid for 1 week
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<string> UpdateUserNameAsync(string userId, UserNameUpdateDTO dto)
    {
        var user = await _repository.GetUserByIdAsync(userId);
        if (user == null) throw new NotFoundException($"User with ID {userId} not found");

        var userWithSameUsername = await _repository.GetUserByUsernameAsync(dto.Username);
        if (userWithSameUsername != null && userWithSameUsername.Id != userId) throw new ValidationException($"A user with username {dto.Username} already exists.");

        user.UpdateUsername(dto.Username);
        await _repository.UpdateUserAsync(user);
        return user.Username;
    }

    public async Task<string> UpdateUserEmailAsync(string userId, UserEmailUpdateDTO dto)
    {
        var user = await _repository.GetUserByIdAsync(userId);
        if (user == null) throw new NotFoundException($"User with ID {userId} not found");

        var userWithSameEmail = await _repository.GetUserByEmailAsync(dto.Email);
        if (userWithSameEmail != null && userWithSameEmail.Id != userId) throw new ValidationException($"A user with email {dto.Email} already exists.");

        user.UpdateEmail(dto.Email);
        await _repository.UpdateUserAsync(user);
        return user.Email;
    }

    public async Task UpdateUserPasswordAsync(string userId, UserPasswordUpdateDTO dto)
    {
        var user = await _repository.GetUserByIdAsync(userId);
        if (user == null) throw new NotFoundException($"User with ID {userId} not found");

        string workFactorHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        user.UpdatePasswordHash(workFactorHash);
        await _repository.UpdateUserAsync(user);
    }

    public async Task<UserRole> UpdateUserRoleAsync(string userId, UserRoleUpdateDTO dto)
    {
        var user = await _repository.GetUserByIdAsync(userId);
        if (user == null) throw new NotFoundException($"User with ID {userId} not found");

        user.UpdateRole(dto.Role);
        await _repository.UpdateUserAsync(user);
        return user.Role;
    }

    public async Task DeleteUserAsync(string userId)
    {
        var user = await _repository.GetUserByIdAsync(userId);
        if (user == null) throw new NotFoundException($"User with ID {userId} not found");

        await _repository.DeleteUserAsync(userId);
    }

    public async Task<UserAuthwnticationDTO> RegisterAdminUserAsync(UserRegisterDTO dto)
    {
        var userWithSameUsername = await _repository.GetUserByUsernameAsync(dto.Username);
        if (userWithSameUsername != null) throw new ValidationException($"A user with username {dto.Username} already exists.");

        var userWithSameEmail = await _repository.GetUserByEmailAsync(dto.Email);
        if (userWithSameEmail != null) throw new ValidationException($"A user with email {dto.Email} already exists.");

        string workFactorHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var user = new User(dto.Username, dto.Email, workFactorHash , UserRole.Admin);
        await _repository.AddUserAsync(user);

        var token = await GenerateJwtTokenAsync(MapToResponseDto(user));
        return new UserAuthwnticationDTO(token, MapToResponseDto(user));
    }

    private static UserResponseDTO MapToResponseDto(User user)
    {
        return new UserResponseDTO(
            user.Id,
            user.Username,
            user.Email,
            user.Role
        );
    }
    
}