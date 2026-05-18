using System.ComponentModel.DataAnnotations;
using ShelfMaster.Domain.Entities;

namespace ShelfMaster.Application.DTOs;
public record UserRegisterDTO
(
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
    string Username,

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    string Email,

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
    string Password
);

public record UserLoginDTO
(
    [Required(ErrorMessage = "Username is required.")]
    string Username,

    [Required(ErrorMessage = "Password is required.")]
    string Password
);

public record UserAuthwnticationDTO
(
    string Token,
    UserResponseDTO User
);
public record UserResponseDTO
(
    string Id,
    string Username,
    string Email,
    UserRole Role
);

public record UserNameUpdateDTO
(
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
    string Username
);

public record UserEmailUpdateDTO
(
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    string Email
);

public record UserPasswordUpdateDTO
(
    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
    string Password
);

public record UserRoleUpdateDTO
(
    [Required(ErrorMessage = "Role is required.")]
    UserRole Role
);