using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TomNam.Models.DTO;
using TomNam.Interfaces;

[ApiController]
[Route("api/users")]

public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var userDTO = await _userService.GetUserProfile(User);
        
        return Ok(new SuccessResponseDTO
        {
            Message = "User profile fetched successfully",
            Data = userDTO
        });
    }
}
