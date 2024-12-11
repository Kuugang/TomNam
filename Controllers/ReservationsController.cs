using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using TomNam.Data;
using TomNam.Interfaces;
using TomNam.Models;
using TomNam.Models.DTO;

[ApiController]
[Route("api/reservation")]
public class ReservationController : ControllerBase
{
    private readonly  IReservationService _reservationService;

    public ReservationController(IReservationService reservationService){
        _reservationService = reservationService;
    }

    [HttpPost]
    [Authorize(Policy = "CustomerPolicy")]
    public async Task<IActionResult> CreateReservation([FromBody] ReservationRequestDTO.Create request)
    {
        var response = await _reservationService.CreateReservationAsync(request, User);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpGet("")]
    [Authorize(Policy = "CustomerPolicy")]
    public async Task<IActionResult> GetReservations()
    {
        var response = await _reservationService.GetAllReservationsAsync(User);
        return StatusCode(StatusCodes.Status200OK, response);
    }
}