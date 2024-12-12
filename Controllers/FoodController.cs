using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TomNam.Data;
using TomNam.Interfaces;
using TomNam.Models;
using TomNam.Models.DTO;

[ApiController]
[Route("api/karenderyas/foods")]
public class FoodsController : ControllerBase
{
    private readonly IFoodService _foodService;

	public FoodsController(
        IFoodService foodService
	)
	{
        _foodService = foodService;
	}

	[HttpPost("{karenderyaId}")]
	[Authorize(Policy = "OwnerPolicy")]
	public async Task<IActionResult> Create([FromForm] FoodDTO.CreateFood request, [FromRoute] Guid KarenderyaId)
	{
        var Food = await _foodService.Create(KarenderyaId, request, User);

		return StatusCode(StatusCodes.Status201Created,
			new SuccessResponseDTO
			{
				Message = "Food created successfully",
				Data = Food,
			}
		);
	}

	// get by id ra
	[HttpGet("{foodId}", Name = "GetFoodById")]
	public async Task<IActionResult> Get([FromRoute] Guid FoodId)
	{
        var Food = await _foodService.GetById(FoodId);

		return StatusCode(
			StatusCodes.Status200OK,
			new SuccessResponseDTO { Message = "Search success", Data = Food }
		);
	}

	[HttpGet()]
	public async Task<IActionResult> GetFromSearch([FromQuery] FoodDTO.ReadFood filters)
	{
		var Foods = await _foodService.FilterFood(filters);

		return StatusCode(
			StatusCodes.Status200OK,
			new SuccessResponseDTO { Message = "Search success", Data =  Foods }
		);
	}

	[HttpPut("{FoodId}/update")]
	[Authorize(Policy = "OwnerPolicy")]
	public async Task<IActionResult> Update([FromRoute] Guid FoodId, [FromForm] FoodDTO.UpdateFood request)
	{
        var Food = await _foodService.Update(FoodId, request, User);

		return StatusCode(
			StatusCodes.Status200OK,
			new SuccessResponseDTO { Message = "Food update success", Data = Food }
		);
	}

	[HttpDelete("{FoodId}/delete")]
	[Authorize(Policy = "OwnerPolicy")]
	public async Task<IActionResult> Delete([FromRoute] Guid FoodId)
	{
        await _foodService.Delete(FoodId, User);

		return StatusCode(
			StatusCodes.Status200OK,
			new SuccessResponseDTO { Message = "Food deleted successfully" }
		);
	}
}
