using Microsoft.AspNetCore.Mvc;

using TomNam.Models.DTO;
using TomNam.Interfaces;


[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try{
            var userByEmail = await _userService.GetUserByEmail(request.Email);
            if (userByEmail != null)
            {
                return StatusCode(StatusCodes.Status409Conflict,
                    new ErrorResponseDTO { 
                        Message = "Registration failed",
                        Error = $"User with email {request.Email} already exists." 
                    }
                );
            }

            var user = await _userService.Create(request);
            var role = await _userService.GetRole(user);

        
            return StatusCode(StatusCodes.Status201Created,
                new SuccessResponseDTO
                {
                    Message = "User created successfully",
                    Data = new JWTDTO {
                        Token = _userService.GenerateToken(user, role)
                    }
                }
            );
        }catch (Exception e) {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new ErrorResponseDTO
                {
                    Message = "Register failed",
                    Error = "Internal server error."
                }
            );
        }
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try{
            var token = await _userService.Login(request);
            if (token == null){
                return StatusCode(StatusCodes.Status401Unauthorized,
                    new ErrorResponseDTO
                    {
                        Message = "Login failed",
                        Error = "Invalid credentials."
                    }
                );
            }

            return Ok(
                new SuccessResponseDTO
                {
                    Message = "Login successful",
                    Data = new JWTDTO { Token = token }
                }
            );
        }catch (Exception e) {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new ErrorResponseDTO
                {
                    Message = "Login failed",
                    Error = "Internal server error."
                }
            );
        }
    }
}