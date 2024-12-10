using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TomNam.Models;
using TomNam.Models.DTO;
using TomNam.Interfaces;
[ApiController]
[Route("api/karenderyas")]

public class KarenderyasController : ControllerBase
{
	private readonly IUserService _userService;
	private readonly IKarenderyaService _karenderyaService;

	public KarenderyasController(IUserService userService, IKarenderyaService karenderyaService)
	{
		_userService = userService;
		_karenderyaService = karenderyaService;
	}

	[HttpPost("create")]
	[Authorize(Policy = "OwnerPolicy")]
	public async Task<IActionResult> Create([FromBody] KarenderyaRequestDTO.Create request)
	{
		var user = await _userService.GetUserFromToken(User);
		if (user == null)
		{
			return StatusCode(StatusCodes.Status403Forbidden,
				new ErrorResponseDTO
				{
					Message = "User is not authenticated.",
					Error = $"User {User} is not found."
				}
			);
		}
		
        var karenderya = await _karenderyaService.GetByOwnerId(user.Id);

		if (karenderya != null) 
		{
			return StatusCode(StatusCodes.Status409Conflict,
			new ErrorResponseDTO {
				Message = "User already has a karenderya.",
				Error = $"User {user.UserName} already has a karenderya."
			});
		}

		karenderya = await _karenderyaService.Create(request, user);

        var ownerProfile = await _userService.GetOwnerProfile(user.Id);
        if (ownerProfile == null)
        {
            return StatusCode(StatusCodes.Status404NotFound,
                new ErrorResponseDTO
                {
                    Message = "Owner profile not found.",
                    Error = $"Owner profile for user {user.UserName} not found."
                }
            );
        }

        ownerProfile.Karenderya = karenderya;

		// Return 201 Created with the resource details
		return StatusCode(StatusCodes.Status201Created, 
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
        List<KarenderyaResponseDTO> karenderyas = await _karenderyaService.FilterKarenderya(filters);

		if (karenderyas.Count == 0)
		{
			return StatusCode(StatusCodes.Status404NotFound,
				new ErrorResponseDTO 
				{
					Message = "Karenderya search failed.",
					Error = "No karenderyas were found."
				}
			);
		}
		
		return StatusCode(StatusCodes.Status200OK,
			new SuccessResponseDTO 
			{
				Message = "Karenderyas found.",
				Data = karenderyas 
			}
		);
	}
	
	
	[HttpPut("{karenderyaId}/update")]
	[Authorize(Policy = "OwnerPolicy")]
	public async Task<IActionResult> Update([FromRoute] Guid karenderyaId, [FromForm] KarenderyaRequestDTO.Update request)
	{
        var karenderya = await _karenderyaService.GetById(karenderyaId);
		
		if (karenderya == null)
		{
			return StatusCode(StatusCodes.Status404NotFound,
				new ErrorResponseDTO 
				{
					Message = "Karenderya update failed.",
					Error = $"Karenderya with ID = {karenderyaId} does not exist."
				}
			);
		}
		
		var UserId = _userService.GetUserIdFromToken(User);
		
		if (karenderya.UserId != UserId)
		{
			return StatusCode(StatusCodes.Status403Forbidden,
				new ErrorResponseDTO 
				{
					Message = "Karenderya update failed",
					Error = $"User {User} is not authorized to update karenderya with ID = {karenderyaId}. User is not the owner of the karenderya."
				}
			); 
		}

        karenderya = await _karenderyaService.Update(karenderya, request);
		
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
		);
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
        var karenderya = await _karenderyaService.GetById(karenderyaId);
		
		if (karenderya == null) 
		{
			return StatusCode(StatusCodes.Status404NotFound,
			new ErrorResponseDTO
			{
				Message = "Karenderya's proof of business creation failed.",
				Error = $"Karenderya with id = {karenderyaId} does not exist."
			});
		}
		
        var UserId = _userService.GetUserIdFromToken(User);
		
		if (karenderya.UserId != UserId)
		{
			return StatusCode(StatusCodes.Status403Forbidden,
				new ErrorResponseDTO 
				{
					Message = "Karenderya's proof of business creation failed.",
					Error = $"User {User} is not authorized to update karenderya with ID = {karenderyaId}. User is not the owner of the karenderya."
				}
			); 
		}
        var proof = await _karenderyaService.CreateProofOfBusiness(karenderya, request);
		
		// Return 201 Created with the resource details
		return StatusCode(StatusCodes.Status201Created, 
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
        var karenderya = await _karenderyaService.GetById(karenderyaId);
		
		if (karenderya == null)
		{
			return StatusCode(StatusCodes.Status404NotFound,
				new ErrorResponseDTO 
				{
					Message = "Karenderya Proof of Business is not found.",
					Error = $"Karenderya with ID = {karenderyaId} does not exist."
				}
			);
		}

        var UserId = _userService.GetUserIdFromToken(User);

        if (karenderya.UserId != UserId){
            return StatusCode(StatusCodes.Status403Forbidden,
                new ErrorResponseDTO
                {
                    Message = "Unauthorized",
                    Error = $"User {User} is not authorized to view karenderya proof of business. User is not the owner of the karenderya."
                }
            );
        }
		
        var ProofOfBusiness = await _karenderyaService.GetProofOfBusiness(karenderyaId);
		
		if (ProofOfBusiness == null)
		{
			return StatusCode(StatusCodes.Status404NotFound,
				new ErrorResponseDTO {
					Message = "Karenderya Proof of Business is not found.",
					Error = $"No proof of business found for karenderya with ID = {karenderyaId}."
				}
			);
		}
		
		return StatusCode(StatusCodes.Status200OK,
			new SuccessResponseDTO 
			{
				Message = "Karenderya Proof of Business found.",
				Data = new KarenderyaResponseDTO.ProofOfBusiness
				{
					Id = ProofOfBusiness.Id,
					OwnerValidID1 = ProofOfBusiness.OwnerValidID1,
					OwnerValidID2 = ProofOfBusiness.OwnerValidID2,
					BusinessPermit = ProofOfBusiness.BusinessPermit,
					BIRPermit = ProofOfBusiness.BIRPermit
				}
			}
		);
	}
	
	[HttpPut("{karenderyaId}/proof/update")]
	[Authorize(Policy = "OwnerPolicy")]
	public async Task<IActionResult> UpdateProof([FromRoute] Guid karenderyaId, [FromForm] KarenderyaRequestDTO.ProofOfBusinessUpdateDTO request)
	{	
        var karenderya = await _karenderyaService.GetById(karenderyaId);
		
		if (karenderya == null) 
		{
			return StatusCode(StatusCodes.Status404NotFound,
			new ErrorResponseDTO
			{
				Message = "Karenderya's proof of business update failed.",
				Error = $"Karenderya with id = {karenderyaId} does not exist."
			});
		}
		
        var UserId = _userService.GetUserIdFromToken(User);
		
		if (karenderya.UserId != UserId)
		{
			return StatusCode(StatusCodes.Status403Forbidden,
				new ErrorResponseDTO 
				{
					Message = "Karenderya's proof of business update failed.",
					Error = $"User {User} is not authorized to update karenderya with ID = {karenderyaId}. User is not the owner of the karenderya."
				}
			); 
		}
		
        var proof = await _karenderyaService.GetProofOfBusiness(karenderyaId);
		
		if (proof == null)
		{
			return StatusCode(StatusCodes.Status404NotFound,
				new ErrorResponseDTO {
					Message = "Proof is not found.",
					Error = $"No proof of business found for karenderya with ID = {karenderyaId}."
				}
			);
		}

        proof = await _karenderyaService.UpdateProofOfBusiness(proof, request);

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