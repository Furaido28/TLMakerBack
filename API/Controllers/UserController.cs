using API.Services.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/")]
public class UserController : BaseController {
    private readonly IUsersService _usersService;
    private readonly IJwtService _jwtService;

    public UserController(IUsersService usersService, IJwtService jwtService) {
        _usersService = usersService;
        _jwtService = jwtService;
    }
    
    [HttpGet("users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ICollection<User>>> GetAllAsync() {
        return Ok(await _usersService.GetAllAsync());
    }
    
    [HttpPost("login")]
    public async Task<ActionResult> LoginAsync([FromQuery] int userId) {
        var existingUser = await _usersService.GetById(userId);

        if (existingUser == null) {
            return NotFound();
        }
        
        var token = _jwtService.GenerateToken(existingUser);
        
        Response.Cookies.Append("jwt", token, new CookieOptions {
            HttpOnly = true,
            Secure = false, 
            SameSite = SameSiteMode.Lax,
            Path = "/",
            Expires = DateTime.UtcNow.AddMinutes(60)
        });
        
        return Ok(new {
            Token = token,
            User = existingUser
        });
    }
}