using Microsoft.AspNetCore.Mvc;
using ShelfMaster.Application.Services;
using ShelfMaster.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using ShelfMaster.WebAPI.Controllers;

namespace ShelfMaster.WebAPI.Controllers;


public class UserController : BaseApiController
{
    private readonly UserService _service;

    public UserController(UserService service)
    {
        _service = service;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegisterDTO dto)
    {
        var result = await _service.RegisterUserAsync(dto);
        return Ok(result);
    }

    [HttpPost("Admin/register")]
    public async Task<IActionResult> RegisterAdminUser([FromBody] UserRegisterDTO dto)
    {
        var result = await _service.RegisterAdminUserAsync(dto);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] UserLoginDTO dto)
    {
        var result = await _service.UserLoginAsync(dto);
        return Ok(result);
    }


    [Authorize]
    [HttpGet("Profile", Name = "GetUserById")]
    public async Task<IActionResult> GetUserById()
    {

        var user = await _service.GetUserByIdAsync(CurrentUserId);
        return Ok(user);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _service.GetAllUsersAsync();
        return Ok(users);
    }

    [Authorize]
    [HttpPut("update/name")]
    public async Task<IActionResult> UpdateUserName([FromBody] UserNameUpdateDTO dto)
    {
        var updatedUser = await _service.UpdateUserNameAsync(CurrentUserId, dto);
        return Ok(updatedUser);
    }

    [Authorize]
    [HttpPut("update/email")]
    public async Task<IActionResult> UpdateUserEmail([FromBody] UserEmailUpdateDTO dto)
    {
        var updatedUser = await _service.UpdateUserEmailAsync(CurrentUserId, dto);
        return Ok(updatedUser);
    }
    
    [Authorize]
    [HttpPut("update/password")]
    public async Task<IActionResult> UpdateUserPassword([FromBody] UserPasswordUpdateDTO dto)
    {
        await _service.UpdateUserPasswordAsync(CurrentUserId, dto);
        return Ok(new { message = "Password updated successfully" });
    }


    [Authorize(Roles = "Admin")]
    [HttpPut("update/role")]
    public async Task<IActionResult> UpdateUserRole([FromBody] UserRoleUpdateDTO dto)
    {
        var updatedUser = await _service.UpdateUserRoleAsync(CurrentUserId, dto);
        return Ok(updatedUser);
    }


    [Authorize(Roles = "Admin")]
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteUser([FromRoute] string id)
    {
        await _service.DeleteUserAsync(id);
        return Ok(new { message = "User deleted successfully" });
    }
}