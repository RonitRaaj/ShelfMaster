using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace ShelfMaster.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected string CurrentUserId
    {
        get
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User context is missing or invalid.");
            }
            
            return userId;
        }
    }
}