using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

using TomNam.Models;
using TomNam.Models.DTO;
using TomNam.Data;
using TomNam.Middlewares;
using TomNam.Interfaces;

[ApiController]
[Route("api/users")]

public class UserController : ControllerBase
{
	private readonly DataContext _context;
    private readonly IUserService _userService;
	private readonly UserManager<User> _userManager;

	public UserController(DataContext context, IUserService userService, UserManager<User> userManager)
	{
		_context = context;
        _userService = userService;
		_userManager = userManager;
	}

	[HttpGet("profile")]
	[Authorize]
	public async Task<IActionResult> Profile()
	{
		var user = await JwtAuthenticationService.GetUserFromTokenAsync(User, _userService);
        var role = await _userManager.GetRolesAsync(user);

        UserDTO userDTO = null;

        if(role[0] ==  "Customer"){
            var CustomerProfile = await _context.CustomerProfile.FirstOrDefaultAsync(p => p.UserId == user.Id); 
            userDTO = new UserDTO.CustomerProfileDTO
            {
                Id = user.Id,
                Email = user.Email,
                Role = role[0],
                FirstName = user.FirstName,
                LastName = user.LastName,
                BehaviorScore = CustomerProfile?.BehaviorScore ?? 0,
            };
        }else{
            userDTO = new UserDTO
            {
                Id = user.Id,
                Email = user.Email,
                Role = role[0],
                FirstName = user.FirstName,
                LastName = user.LastName,
            };
        }

        return Ok(
            new SuccessResponseDTO{ 
                Message = "User profile fetched successfully",
                Data = userDTO
            }
        );
	}
}