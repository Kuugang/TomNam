using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

using TomNam.Models;
using TomNam.Models.DTO;
using TomNam.Data;
using TomNam.Middlewares;
using TomNam.Interfaces;
using System.Diagnostics;


[ApiController]
[Route("api/karenderya")]

public class KarenderyaController : ControllerBase
{
	private readonly DataContext _context;
	private readonly IUserService _userService;

	public KarenderyaController(DataContext context, IUserService userService)
	{
		_context = context;
		_userService = userService;
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> GetKarenderya([FromRoute] Guid id)
	{
		var karenderya = await _context.Karenderya.FirstOrDefaultAsync(k => k.Id == id);

		if (karenderya == null)
		{
			return NotFound();  // Return 404 if not found
		}

		return Ok(karenderya);  // Return the found Karenderya
	}

	// QUERY PARAMETERS
	// api/karendera/?open=true&locationCity=Manila&locationProvince=Metro%20Manila

	// From Route
	// api/karenderya/create/name/locationStreet/locationBarangay/locationCity/locationProvince/description/logoPhoto/coverPhoto

	// From Body
	// api/karenderya/create 
	// Response Body : {
	//     "Name": "Karenderya Name",
	//     "LocationStreet": "Location Street",
	//     "LocationBarangay": "Location
	// }

	[HttpPost("create")]
	[Authorize(Policy = "OwnerPolicy")]
	public async Task<IActionResult> Create([FromBody] KarenderyaDTO.Create request)
	{
		var user = await JwtAuthenticationService.GetUserFromTokenAsync(User, _userService);
		if (user == null)
		{
			return Unauthorized();
		}

		if (user == null)
		{
			return Unauthorized(new { message = "Invalid or malformed token" });
		}

		var karenderya = new Karenderya
		{
			UserId = user.Id,
			User = user,
			Name = request.Name,
			LocationStreet = request.LocationStreet,
			LocationBarangay = request.LocationBarangay,
			LocationCity = request.LocationCity,
			LocationProvince = request.LocationProvince,
			Description = request.Description,
			DateFounded = DateOnly.FromDateTime(DateTime.Now)
		};

		await _context.Karenderya.AddAsync(karenderya);
		await _context.SaveChangesAsync();

		[Required]
		public required string KarenderyaId { get; set; } // uuid of karenderya
		[Required]
		public required string Name { get; set; }
		[Required]
		public required string LocationStreet { get; set; }
		[Required]
		public required string LocationBarangay { get; set; }
		[Required]
		public required string LocationCity { get; set; }
		[Required]
		public required string LocationProvince { get; set; }
		public string? Description { get; set; }
		public string? LogoPhoto { get; set; }
		public string? CoverPhoto { get; set; }
		
		return Ok();
	}
}