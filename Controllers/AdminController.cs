using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TomNam.Data;
using TomNam.Models.DTO;

[ApiController]
[Route("api/admin")]
[Authorize(Policy = "AdminPolicy")]
public class AdminController : ControllerBase
{

    private readonly DataContext _context;

    public AdminController(DataContext context)
    {
        _context = context;
    }
    // Your actions here
    [HttpGet("dashboard")]
    public IActionResult GetDashboard()
    {
        return Ok(new { message = "Admin dashboard data" });
    }

    [HttpPut("karenderyas/{karenderyaId}/verify")]
    public async Task<IActionResult> VerifyKarenderya([FromRoute] Guid karenderyaId)
    {
		    var karenderya = await _context.Karenderya.FindAsync(karenderyaId);
        
        if(karenderya == null){
			      return StatusCode(StatusCodes.Status404NotFound,
			      new ErrorResponseDTO
			      {
			      	Message = "Karenderya verification failed.",
			      	Error = $"Karenderya with id = {karenderyaId} does not exist."
			      });
        }
        karenderya.IsVerified = true;
		    await _context.SaveChangesAsync();
        return Ok(
			      new SuccessResponseDTO 
			      {
			      	Message = $"Karenderya {karenderya.Name} verified successfully",
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
			      		IsVerified = karenderya.IsVerified
			      	}
			      }
        );
    }
}
