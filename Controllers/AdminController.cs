using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TomNam.Data;
using TomNam.Interfaces;
using TomNam.Models.DTO;

[ApiController]
[Route("api/admin")]
[Authorize(Policy = "AdminPolicy")]
public class AdminController : ControllerBase
{

    private readonly DataContext _context;
    private readonly IKarenderyaService _karenderyaService;

    public AdminController(DataContext context, IKarenderyaService karenderyaService)
    {
        _context = context;
        _karenderyaService = karenderyaService;
    }

    [HttpPut("karenderyas/{karenderyaId}/verify")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> VerifyKarenderya([FromRoute] Guid karenderyaId)
    {
        var karenderya = await _karenderyaService.VerifyKarenderya(karenderyaId);

        return Ok(
            new SuccessResponseDTO
            {
                Message = $"Karenderya {karenderya.Name} verified successfully",
                Data = karenderya
            }
        );
    }
}