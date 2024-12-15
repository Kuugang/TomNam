using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TomNam.Exceptions;
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
    [Authorize(Policy = "OwnerCustomerPolicy")]
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

    [HttpGet("{ReservationId}")]
    //TODO Add authorization policy
    public async Task<IActionResult> GetReservation([FromRoute] Guid ReservationId)
    {
        var Reservation = await _reservationService.GetById(ReservationId);
        if(Reservation == null)
        {
            throw new ApplicationExceptionBase(
                "Reservation not found.",
                "Reservation retrieval failed.",
                StatusCodes.Status404NotFound
            );
        }
        return StatusCode(StatusCodes.Status200OK,
            new SuccessResponseDTO
            {
                Message = "Reservations retrieved successfully",
                Data = new ReservationResponseDTO(Reservation)
            } 
        );
    }
}