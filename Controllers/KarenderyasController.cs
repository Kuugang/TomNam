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
using System.Reflection.Metadata.Ecma335;
using Microsoft.VisualBasic;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Runtime;


[ApiController]
[Route("api/karenderyas")]

public class KarenderyasController : ControllerBase
{
	private readonly DataContext _context;
	private readonly IUserService _userService;
	private readonly IFileUploadService _uploadService;

	public KarenderyasController(DataContext context, IUserService userService, IFileUploadService uploadService)
	{
		_context = context;
		_userService = userService;
		_uploadService = uploadService;
	}

	[HttpPost("create")]
	[Authorize(Policy = "OwnerPolicy")]
	public async Task<IActionResult> Create([FromBody] KarenderyaRequestDTO.Create request)
	{
		var user = await JwtAuthenticationService.GetUserFromTokenAsync(User, _userService);
		if (user == null)
		{
			return StatusCode(StatusCodes.Status401Unauthorized,
				new ErrorResponseDTO
				{
					Message = "User is not authenticated.",
					Error = $"User {User} is not found."
				}
			);
		}
		
		var userHasKarenderya = await _context.Karenderya.FirstOrDefaultAsync(k => k.UserId == user.Id);
		
		if (userHasKarenderya != null) 
		{
			return StatusCode(StatusCodes.Status409Conflict,
			new ErrorResponseDTO {
				Message = "User already has a karenderya.",
				Error = $"User {user.UserName} already has a karenderya."
			});
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
		
		var ownerProfile = await _context.OwnerProfile.FirstOrDefaultAsync(p => p.UserId == user.Id);
		if (ownerProfile != null)
		{
			ownerProfile.Karenderya = karenderya;
			await _context.SaveChangesAsync();
		}
		
		// Construct the URI for the newly created resource
		var locationUri = Url.Action(nameof(Get), new { KarenderyaId = karenderya.Id });

		// Return 201 Created with the resource details
		return Created(locationUri, 
			new SuccessResponseDTO
			{
				Message = $"Karenderya {karenderya.Name} is created successfully.",
				Data = new KarenderyaResponseDTO 
				{
					Id = karenderya.Id,
					UserId = karenderya.UserId,
					Name = karenderya.Name,
					LocationStreet = karenderya.LocationStreet,
					LocationBarangay = karenderya.LocationBarangay,
					LocationCity = karenderya.LocationCity,
					LocationProvince = karenderya.LocationProvince,
					DateFounded = karenderya.DateFounded,
					Description = karenderya.Description,
					LogoPhoto = karenderya.LogoPhoto,
					CoverPhoto = karenderya.CoverPhoto,
										Rating = karenderya.Rating,
					IsVerified = karenderya.IsVerified
				}
			}
		);
	}
	
	[HttpGet("")]
	public async Task<IActionResult> Get([FromQuery] KarenderyaRequestDTO.Read filters)
	{
		var query = _context.Karenderya.AsQueryable();  // Start with the base query
		// query = query.Where(k => k.IsVerified == true); // Get the karenderyas that are verified

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
			return StatusCode(StatusCodes.Status200OK,
				new SuccessResponseDTO 
				{
					Message = "No karenderyas were found."				}
			);
		}
		
		List<KarenderyaResponseDTO> responseKarenderyas = new List<KarenderyaResponseDTO>();
		
		// Return the karenderyas as a successful response
		foreach(var karenderya in karenderyas)
		{
			responseKarenderyas.Add(new KarenderyaResponseDTO {
				Id = karenderya.Id,
				UserId = karenderya.UserId,
				Name = karenderya.Name,
				LocationStreet = karenderya.LocationStreet,
				LocationBarangay = karenderya.LocationBarangay,
				LocationCity = karenderya.LocationCity,
				LocationProvince = karenderya.LocationProvince,
				DateFounded = karenderya.DateFounded,
				Description = karenderya.Description,
				LogoPhoto = karenderya.LogoPhoto,
				CoverPhoto = karenderya.CoverPhoto,
								Rating = karenderya.Rating,
				IsVerified = karenderya.IsVerified
			});
		}

		return StatusCode(StatusCodes.Status200OK,
			new SuccessResponseDTO 
			{
				Message = "Karenderyas found.",
				Data = responseKarenderyas
			}
		);
	}
	
	
	[HttpPut("{karenderyaId}/update")]
	[Authorize(Policy = "OwnerPolicy")]
	public async Task<IActionResult> Update([FromRoute] Guid karenderyaId, [FromForm] KarenderyaRequestDTO.Update request)
	{
		var karenderya = await _context.Karenderya.FirstOrDefaultAsync(k => k.Id == karenderyaId);
		
		if (karenderya == null)
		{
			return StatusCode(StatusCodes.Status404NotFound,
				new ErrorResponseDTO 
				{
					Message = "Karenderya update failed.",
					Error = $"Karenderya with ID = {karenderyaId} does not exist."
				}
			);  // Return 404 if not found
		}
		
		var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user id
		
		if (karenderya.UserId != UserId)
		{
			return StatusCode(StatusCodes.Status401Unauthorized,
				new ErrorResponseDTO 
				{
					Message = "Karenderya update failed",
					Error = $"User {User} is not authorized to update karenderya with ID = {karenderyaId}. User is not the owner of the karenderya."
				}
			); 
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

		if (request.LogoPhoto != null){
			string LogoPath = _uploadService.Upload(request.LogoPhoto, "Karenderya\\Logo");
			karenderya.LogoPhoto = LogoPath;
		}

		if (request.CoverPhoto != null){
			string CoverPhotoPath = _uploadService.Upload(request.CoverPhoto, "Karenderya\\Cover");
			karenderya.CoverPhoto = CoverPhotoPath;
		}
		
		
		if (request.IsVerified != null)
		{
			karenderya.IsVerified = request.IsVerified;
		}
		
		// Save changes to the database
		await _context.SaveChangesAsync();
		
		return StatusCode(StatusCodes.Status200OK,
			new SuccessResponseDTO 
			{
				Message = $"Karenderya {karenderya.Name} updated successfully",
				Data = new KarenderyaResponseDTO 
				{
					Id = karenderya.Id,
					UserId = karenderya.UserId,
					Name = karenderya.Name,
					LocationStreet = karenderya.LocationStreet,
					LocationBarangay = karenderya.LocationBarangay,
					LocationCity = karenderya.LocationCity,
					LocationProvince = karenderya.LocationProvince,
					DateFounded = karenderya.DateFounded,
					Description = karenderya.Description,
					LogoPhoto = karenderya.LogoPhoto,
					CoverPhoto = karenderya.CoverPhoto,
										Rating = karenderya.Rating,
					IsVerified = karenderya.IsVerified
				}
			}
		);  // Return the updated Karenderya
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
	public async Task<IActionResult> CreateProof([FromRoute] Guid karenderyaId, [FromForm] KarenderyaRequestDTO.ProofOfBusinessCreateDTO request)
	{	
		var karenderya = await _context.Karenderya.FindAsync(karenderyaId);
		
		if (karenderya == null) 
		{
			return StatusCode(StatusCodes.Status404NotFound,
			new ErrorResponseDTO
			{
				Message = "Karenderya's proof of business creation failed.",
				Error = $"Karenderya with id = {karenderyaId} does not exist."
			});
		}
		
		var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user id
		
		if (karenderya.UserId != UserId)
		{
			return StatusCode(StatusCodes.Status401Unauthorized,
				new ErrorResponseDTO 
				{
					Message = "Karenderya's proof of business creation failed.",
					Error = $"User {User} is not authorized to update karenderya with ID = {karenderyaId}. User is not the owner of the karenderya."
				}
			); 
		}
		
		string ownerValidID1Path = _uploadService.Upload(request.OwnerValidID1, "Karenderya\\Proof\\ValidID");
		string ownerValidID2Path = _uploadService.Upload(request.OwnerValidID2, "Karenderya\\Proof\\ValidID");
		string businessPermitPath = _uploadService.Upload(request.BusinessPermit, "Karenderya\\Proof\\BusinessPermit");
		string BIRPermitPath = _uploadService.Upload(request.BIRPermit, "Karenderya\\Proof\\BIRPermit");
		
		var proof = new ProofOfBusiness
		{
			KarenderyaId = karenderya.Id,
			Karenderya = karenderya,
			OwnerValidID1 = ownerValidID1Path,
			OwnerValidID2 = ownerValidID2Path,
			BusinessPermit = businessPermitPath,
			BIRPermit = BIRPermitPath
		};
		
		await _context.ProofOfBusiness.AddAsync(proof);
		await _context.SaveChangesAsync();
		
		// Construct the URI for the newly created resource
		var locationUri = Url.Action(nameof(GetProof), new { KarenderyaId = karenderya.Id });

		// Return 201 Created with the resource details
		return Created(locationUri, 
			new SuccessResponseDTO
			{
				Message = $"Karenderya {karenderya.Name}'s proof of business is created successfully.",
				Data = new KarenderyaResponseDTO.ProofOfBusiness
				{
					Id = proof.Id,
					OwnerValidID1 = proof.OwnerValidID1,
					OwnerValidID2 = proof.OwnerValidID2,
					BusinessPermit = proof.BusinessPermit,
					BIRPermit = proof.BIRPermit
				}
			}
		);
	}
	
	[HttpGet("{karenderyaId}/proof/")]
	[Authorize(Policy = "OwnerPolicy")]
	public async Task<IActionResult> GetProof([FromRoute] Guid karenderyaId)
	{
		var karenderya = await _context.Karenderya.FirstOrDefaultAsync(k => k.Id == karenderyaId);
		
		if (karenderya == null)
		{
			return StatusCode(StatusCodes.Status404NotFound,
				new ErrorResponseDTO 
				{
					Message = "Proof is not found.",
					Error = $"Karenderya with ID = {karenderyaId} does not exist."
				}
			);
		}
		
		var proof = await _context.ProofOfBusiness.FirstOrDefaultAsync(p => p.KarenderyaId == karenderyaId);
		
		if (proof == null)
		{
			return StatusCode(StatusCodes.Status404NotFound,
				new ErrorResponseDTO {
					Message = "Proof is not found.",
					Error = $"No proof of business found for karenderya with ID = {karenderyaId}."
				}
			);
		}
		
		return StatusCode(StatusCodes.Status200OK,
			new SuccessResponseDTO 
			{
				Message = "Proof of business found.",
				Data = new KarenderyaResponseDTO.ProofOfBusiness
				{
					Id = proof.Id,
					OwnerValidID1 = proof.OwnerValidID1,
					OwnerValidID2 = proof.OwnerValidID2,
					BusinessPermit = proof.BusinessPermit,
					BIRPermit = proof.BIRPermit
				}
			}
		);
	}
	
	[HttpPut("{karenderyaId}/proof/update")]
	[Authorize(Policy = "OwnerPolicy")]
	public async Task<IActionResult> UpdateProof([FromRoute] Guid karenderyaId, [FromForm] KarenderyaRequestDTO.ProofOfBusinessUpdateDTO request)
	{	
		var karenderya = await _context.Karenderya.FindAsync(karenderyaId);
		
		if (karenderya == null) 
		{
			return StatusCode(StatusCodes.Status404NotFound,
			new ErrorResponseDTO
			{
				Message = "Karenderya's proof of business update failed.",
				Error = $"Karenderya with id = {karenderyaId} does not exist."
			});
		}
		
		var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user id
		
		if (karenderya.UserId != UserId)
		{
			return StatusCode(StatusCodes.Status401Unauthorized,
				new ErrorResponseDTO 
				{
					Message = "Karenderya's proof of business update failed.",
					Error = $"User {User} is not authorized to update karenderya with ID = {karenderyaId}. User is not the owner of the karenderya."
				}
			); 
		}
		
		var proof = await _context.ProofOfBusiness.FirstOrDefaultAsync(p => p.KarenderyaId == karenderyaId);
		
		if (proof == null)
		{
			return StatusCode(StatusCodes.Status404NotFound,
				new ErrorResponseDTO {
					Message = "Proof is not found.",
					Error = $"No proof of business found for karenderya with ID = {karenderyaId}."
				}
			);
		}
		
		if (request.OwnerValidID1 != null)
		{
			string ownerValidID1Path = _uploadService.Upload(request.OwnerValidID1, "Karenderya\\Proof\\ValidID");
			proof.OwnerValidID1 = ownerValidID1Path;
		}
		
		if (request.OwnerValidID2 != null)
		{
			string ownerValidID2Path = _uploadService.Upload(request.OwnerValidID2, "Karenderya\\Proof\\ValidID");
			proof.OwnerValidID2 = ownerValidID2Path;
		}
		
		if (request.BusinessPermit!= null)
		{
			string businessPermitPath = _uploadService.Upload(request.BusinessPermit, "Karenderya\\Proof\\BusinessPermit");
			proof.BusinessPermit = businessPermitPath;
		}
		
		if (request.BIRPermit!= null)
		{
			string BIRPermitPath = _uploadService.Upload(request.BIRPermit, "Karenderya\\Proof\\BIRPermit");
			proof.BIRPermit = BIRPermitPath;
		}
		
		await _context.SaveChangesAsync();
		
		return StatusCode(StatusCodes.Status200OK,
			new SuccessResponseDTO 
			{
				Message = "Proof of business updated sucessfully.",
				Data = new KarenderyaResponseDTO.ProofOfBusiness
				{
					Id = proof.Id,
					OwnerValidID1 = proof.OwnerValidID1,
					OwnerValidID2 = proof.OwnerValidID2,
					BusinessPermit = proof.BusinessPermit,
					BIRPermit = proof.BIRPermit
				}
			}
		);
	}
	
}
