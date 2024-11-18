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
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


[ApiController]
[Route("api/karenderya")]

public class KarenderyaController : ControllerBase 
{
	private readonly DataContext _context;

	public KarenderyaController(DataContext context)
	{
		_context = context;
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> GetKarenderya([FromRoute] Guid id)
	{
		var karenderya = await _context.Karenderyas.FirstOrDefaultAsync(k => k.Id == id);

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
	
	// temp sol
	private readonly UserManager<User> _userManager;

	[HttpPost("create")]
	[Authorize(Roles = "Owner")] 
	public async Task<IActionResult> CreateKarenderya([FromRoute] KarenderyaDTO.Create request){
		User user = await _userManager.FindByEmailAsync("user@example.com");
		
		var karenderya = new Karenderya
		{ 
			// TODO: kuhaon si token para makuha si user and userid
			
			// temporary measure
			UserId = user.Id,
			User = user,
			
			Name = request.Name,
			LocationStreet = request.LocationStreet,
			LocationBarangay = request.LocationBarangay,
			LocationCity = request.LocationCity,
			LocationProvince = request.LocationProvince,
			Description = request.Description,
			DateFounded = DateOnly.FromDateTime(DateTime.Now),
			LogoPhoto = request.LogoPhoto,
			CoverPhoto = request.CoverPhoto
		};

		await _context.Karenderyas.AddAsync(karenderya);
		await _context.SaveChangesAsync();

		return Ok(karenderya);
	}
}