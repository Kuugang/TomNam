using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

using TomNam.Models;
using TomNam.Models.DTO;
using TomNam.Data;
using TomNam.Middlewares;
using TomNam.Interfaces;


[ApiController]
[Route("api/karenderyas")]

public class KarenderyasController : ControllerBase
{
	private readonly DataContext _context;
	private readonly IUserService _userService;
	private readonly UserManager<User> _userManager;

	public KarenderyasController(DataContext context, IUserService userService, UserManager<User> userManager)
	{
		_context = context;
		_userService = userService;
		_userManager = userManager;
	}

	[HttpPost("create")]
	[Authorize(Policy = "OwnerPolicy")]
	public async Task<IActionResult> Create([FromBody] KarenderyaRequestDTO.Create request)
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
			DateFounded = request.DateFounded
		};

		await _context.Karenderya.AddAsync(karenderya);
		await _context.SaveChangesAsync();

		return Ok(
			new
			{
				karenderya.Id
			}
		);
	}
	
	[HttpGet("")]
	public async Task<IActionResult> Get([FromQuery] KarenderyaRequestDTO.Read filters)
	{
		var query = _context.Karenderya.AsQueryable();  // Start with the base query

		// Apply KarenderyaId filter if provided
		if (filters.KarenderyaId != null)
		{
			query = query.Where(k => k.Id == filters.KarenderyaId);
		}

		// Apply Name filter if provided
		if (!string.IsNullOrEmpty(filters.Name))
		{
			query = query.Where(k => k.Name.ToLower().Contains(filters.Name.ToLower()));  // Case-insensitive by default
		}
		
		// Apply LocationStreet filter if provided
		if (!string.IsNullOrEmpty(filters.LocationStreet))
		{
			query = query.Where(k => k.LocationStreet.ToLower().Contains(filters.LocationStreet.ToLower()));
		}

		// Apply LocationBarangay filter if provided (case-insensitive)
		if (!string.IsNullOrEmpty(filters.LocationBarangay))
		{
			query = query.Where(k => k.LocationBarangay.ToLower().Contains(filters.LocationBarangay.ToLower()));
		}

		// Apply LocationCity filter if provided (case-insensitive)
		if (!string.IsNullOrEmpty(filters.LocationCity))
		{
			query = query.Where(k => k.LocationCity.ToLower().Contains(filters.LocationCity.ToLower()));
		}

		// Apply LocationProvince filter if provided (case-insensitive)
		if (!string.IsNullOrEmpty(filters.LocationProvince))
		{
			query = query.Where(k => k.LocationProvince.ToLower().Contains(filters.LocationProvince.ToLower()));
		}

		// Execute the query and get the results
		var karenderyas = await query.ToListAsync();

		if (!karenderyas.Any())
		{
			return NotFound(new { message = "No Karenderyas found with the given filters" });
		}

		return Ok(karenderyas);
	}
	
	
	[HttpPut("{karenderyaId}/update")]
	[Authorize(Policy = "OwnerPolicy")]
	public async Task<IActionResult> Update([FromRoute] Guid karenderyaId, [FromBody] KarenderyaRequestDTO.Update request)
	{
		var karenderya = await _context.Karenderya.FirstOrDefaultAsync(k => k.Id == karenderyaId);
		
		if (karenderya == null)
		{
			return NotFound(new { message = "Karenderya not found" });  // Return 404 if not found
		}
		
		var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user id
		
		if (karenderya.UserId != UserId)
		{
			return Unauthorized(new { message = "You are not the owner of this karenderya." }); 
		}
		
		// Update karenderya
		if (request.Name != null)
	   		karenderya.Name = request.Name;

		if (request.LocationStreet != null)
			karenderya.LocationStreet = request.LocationStreet;

		if (request.LocationBarangay != null)
			karenderya.LocationBarangay = request.LocationBarangay;

		if (request.LocationCity != null)
			karenderya.LocationCity = request.LocationCity;

		if (request.LocationProvince != null)
			karenderya.LocationProvince = request.LocationProvince;

		if (request.Description != null)
			karenderya.Description = request.Description;

		if (request.LogoPhoto != null)
			karenderya.LogoPhoto = request.LogoPhoto;

		if (request.CoverPhoto != null)
			karenderya.CoverPhoto = request.CoverPhoto;
		
		// Save changes to the database
		await _context.SaveChangesAsync();
		
		return Ok(karenderya);  // Return the updated Karenderya
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

	
	[HttpPost("{karenderyaId}/proof/create")]
	[Authorize(Policy = "OwnerPolicy")]
	public async Task<IActionResult> CreateProof([FromRoute] Guid karenderyaId, [FromBody] KarenderyaRequestDTO.ProofOfBusinessCreateDTO request)
	{	
		var karenderya = await _context.Karenderya.FindAsync(karenderyaId);
		
		if (karenderya == null) 
		{
			return NotFound(new { message = "Karenderya not found" });
		}
		
		var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user id
		
		if (karenderya.UserId != UserId)
		{
			return Unauthorized(new { message = "You are not the owner of this karenderya." }); 
		}
		
		var proof = new ProofOfBusiness
		{
			KarenderyaId = karenderya.Id,
			Karenderya = karenderya,
			OwnerValidID1 = request.OwnerValidID1,
			OwnerValidID2 = request.OwnerValidID2,
			BusinessPermit = request.BusinessPermit,
			BIRPermit = request.BIRPermit
		};
		
		await _context.ProofOfBusiness.AddAsync(proof);
		await _context.SaveChangesAsync();
		
		return CreatedAtAction("Created Karenderya proof", new { proof.Id });
	}
	
	[HttpGet("{karenderyaId}/proof/")]
	[Authorize(Policy = "OwnerPolicy")]
	public async Task<IActionResult> GetProof([FromRoute] Guid karenderyaId)
	{
		var karenderya = await _context.Karenderya.FirstOrDefaultAsync(k => k.Id == karenderyaId);
		
		if (karenderya == null)
		{
			return NotFound(new {message = "Karenderya not found"});
		}
		
		var proof = await _context.ProofOfBusiness.FirstAsync();
		
		if (proof == null)
		{
			return NotFound(new {message = "Karenderya does not have proof of business"});
		}
		
		return Ok(proof);
	}
	
	[HttpPut("{karenderyaId}/proof/update")]
	[Authorize(Policy = "OwnerPolicy")]
	public async Task<IActionResult> UpdateProof([FromRoute] Guid karenderyaId, [FromBody] KarenderyaRequestDTO.ProofOfBusinessUpdateDTO request)
	{	
		var karenderya = await _context.Karenderya.FirstOrDefaultAsync(k => k.Id == karenderyaId);
		
		if (karenderya == null)
		{
			return NotFound(new { message = "Karenderya not found" });
		}
		
		var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user id
		
		if (karenderya.UserId != UserId)
		{
			return Unauthorized(new { message = "You are not the owner of this karenderya." }); 
		}
		
		var proof = await _context.ProofOfBusiness.FirstOrDefaultAsync(p => p.KarenderyaId == karenderyaId);
		
		if (proof == null)
		{
			return NotFound(new { message = "Proof of Business not found" });
		}
		
		if (request.OwnerValidID1 != null)
		{
			proof.OwnerValidID1 = request.OwnerValidID1;
		}
		
		if (request.OwnerValidID2 != null)
		{
			proof.OwnerValidID2 = request.OwnerValidID2;
		}
		
		if (request.BusinessPermit!= null)
		{
			proof.BusinessPermit = request.BusinessPermit;
		}
		
		if (request.BIRPermit!= null)
		{
			proof.BIRPermit = request.BIRPermit;
		}
		
		await _context.SaveChangesAsync();
		
		return Ok(proof);
	}
	
}