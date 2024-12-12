using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TomNam.Models.DTO;
using TomNam.Interfaces;
[ApiController]
[Route("api/karenderyas")]

public class KarenderyasController : ControllerBase
{
    private readonly IKarenderyaService _karenderyaService;

    public KarenderyasController(IKarenderyaService karenderyaService)
    {
        _karenderyaService = karenderyaService;
    }

    [HttpPost("create")]
    [Authorize(Policy = "OwnerPolicy")]
    public async Task<IActionResult> Create([FromBody] KarenderyaRequestDTO.Create request)
    {
        var Karenderya = await _karenderyaService.Create(User, request);
        return StatusCode(StatusCodes.Status201Created,
            new SuccessResponseDTO
            {
                Message = $"Karenderya {Karenderya.Name} is created successfully.",
                Data = Karenderya
            }
        );
    }

    [HttpGet("")]
    public async Task<IActionResult> Get([FromQuery] KarenderyaRequestDTO.Read filters)
    {
        var Karenderyas = await _karenderyaService.FilterKarenderyas(filters);

        return StatusCode(StatusCodes.Status200OK,
            new SuccessResponseDTO
            {
                Message = "Karenderyas found.",
                Data = Karenderyas
            }
        );
    }


    [HttpPut("{karenderyaId}/update")]
    [Authorize(Policy = "OwnerPolicy")]
    public async Task<IActionResult> Update([FromRoute] Guid KarenderyaId, [FromForm] KarenderyaRequestDTO.Update request)
    {

        var Karenderya = await _karenderyaService.Update(KarenderyaId, request, User);

        return StatusCode(StatusCodes.Status200OK,
            new SuccessResponseDTO
            {
                Message = $"Karenderya {Karenderya.Name} updated successfully",
                Data = Karenderya,
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
    public async Task<IActionResult> CreateProof([FromRoute] Guid KarenderyaId, [FromForm] KarenderyaRequestDTO.ProofOfBusinessCreateDTO request)
    {
        // Return 201 Created with the resource details
        var proof = await _karenderyaService.CreateProofOfBusiness(KarenderyaId, request, User);
        return StatusCode(StatusCodes.Status201Created,
            new SuccessResponseDTO
            {
                Message = $"Karenderya {proof.Karenderya.Name}'s proof of business is created successfully.",
                Data = proof
            }
        );
    }

    [HttpGet("{karenderyaId}/proof/")]
    [Authorize(Policy = "OwnerPolicy")]
    public async Task<IActionResult> GetProof([FromRoute] Guid KarenderyaId)
    {
        var ProofOfBusiness = await _karenderyaService.GetProofOfBusiness(KarenderyaId, User);
        return StatusCode(StatusCodes.Status200OK,
            new SuccessResponseDTO
            {
                Message = "Karenderya Proof of Business found.",
                Data = ProofOfBusiness
            }
        );
    }

    [HttpPut("{karenderyaId}/proof/update")]
    [Authorize(Policy = "OwnerPolicy")]
    public async Task<IActionResult> UpdateProof([FromRoute] Guid KarenderyaId, [FromForm] KarenderyaRequestDTO.ProofOfBusinessUpdateDTO request)
    {
        var proof = await _karenderyaService.UpdateProofOfBusiness(KarenderyaId, request, User);
        return StatusCode(StatusCodes.Status200OK,
            new SuccessResponseDTO
            {
                Message = "Proof of business updated sucessfully.",
                Data = proof
            }
        );
    }
}