using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TomNam.Interfaces;
using TomNam.Models.DTO;

[ApiController]
[Route("api/reservation")]
public class ReservationController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpPost]
    [Authorize(Policy = "CustomerPolicy")]
    public async Task<IActionResult> CreateReservation([FromBody] ReservationRequestDTO.Create request)
    {
        var Reservation = await _reservationService.CreateReservation(request, User);

        return StatusCode(StatusCodes.Status201Created,
            new SuccessResponseDTO
            {
                Message = "Reservation created successfully",
                Data = new ReservationResponseDTO(Reservation)
            }
        );
    }

    [HttpGet("")]
    [Authorize(Policy = "CustomerPolicy")]
    public async Task<IActionResult> GetReservations()
    {
        var Reservations = await _reservationService.GetAllReservations(User);
        return StatusCode(StatusCodes.Status200OK,
            new SuccessResponseDTO
            {
                Message = "Reservations retrieved successfully",
                Data = Reservations.Select(r => new ReservationResponseDTO(r)).ToList()
            } 
        );
    }
}