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
	private readonly DataContext _context;
	private readonly IUserService _userService;
	private readonly IFileUploadService _uploadService;
    private readonly IFoodService _foodService;
    private readonly IKarenderyaService _karenderyaService;

	public FoodsController(
		DataContext context,
		IUserService userService,
		IFileUploadService uploadService,
        IFoodService foodService,
        IKarenderyaService karenderyaService
	)
	{
		_context = context;
		_userService = userService;
		_uploadService = uploadService;
        _foodService = foodService;
        _karenderyaService = karenderyaService;
	}

	[HttpPost("{karenderyaId}")]
	[Authorize(Policy = "OwnerPolicy")]
	public async Task<IActionResult> Create([FromForm] FoodDTO.CreateFood request, [FromRoute] Guid karenderyaId)
	{
        var UserId = _userService.GetUserIdFromToken(User);
        var Karenderya = await _karenderyaService.GetById(karenderyaId);

		if (Karenderya == null)
		{
			return StatusCode(
				StatusCodes.Status404NotFound,
				new ErrorResponseDTO
				{
					Message = "Food creation failed",
					Error = $"Karenderya with ID {karenderyaId} not found",
				}
			);
		}

        if (UserId != Karenderya.UserId)
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new ErrorResponseDTO
                {
                    Message = "Food creation failed",
                    Error = $"You are not the owner of this karenderya with ID {karenderyaId}",
                }
            );
        }

        if(!await _foodService.UniqueFoodName(karenderyaId, request.FoodName))
        {
            return StatusCode(
                StatusCodes.Status409Conflict,
                new ErrorResponseDTO
                {
                    Message = "Food creation failed",
                    Error = "Food name already exists",
                }
            );
        }

        var Food = await _foodService.Create(Karenderya, request);

		return StatusCode(StatusCodes.Status201Created,
			new SuccessResponseDTO
			{
				Message = "Food created successfully",
				Data = new FoodDTO.ReadFood
				{
					Id = Food.Id,
					KarenderyaId = Food.KarenderyaId.ToString(),
					FoodName = Food.FoodName,
					FoodDescription = Food.FoodDescription,
					UnitPrice = Food.UnitPrice,
					FoodPhoto = Food.FoodPhoto,
				},
			}
		);
	}

	// get by id ra
	[HttpGet("{foodId}", Name = "GetFoodById")]
	public async Task<IActionResult> Get([FromRoute] Guid FoodId)
	{
        var food = await _foodService.GetById(FoodId);

		if (food == null)
		{
			return StatusCode(
				StatusCodes.Status404NotFound,
				new ErrorResponseDTO
				{
					Message = "Food creation failed",
					Error = $"Food with ID {FoodId} not found",
				}
			);
		}

		return StatusCode(
			StatusCodes.Status200OK,
			new SuccessResponseDTO { Message = "Search success", Data = food }
		);
	}

	[HttpGet()]
	public async Task<IActionResult> GetFromSearch([FromQuery] FoodDTO.ReadFood filters)
	{
		List<FoodDTO.ReadFood> responseFoods = await _foodService.FilterFood(filters);
        // if (responseFoods.Count == 0)
        // {
        //     return StatusCode(
        //         StatusCodes.Status404NotFound,
        //         new ErrorResponseDTO
        //         {
        //             Message = "Search failed",
        //             Error = "No food found",
        //         }
        //     );
        // }

		return StatusCode(
			StatusCodes.Status200OK,
			new SuccessResponseDTO { Message = "Search success", Data =  responseFoods}
		);
	}

	[HttpPut("{FoodId}/update")]
	[Authorize(Policy = "OwnerPolicy")]
	public async Task<IActionResult> Update([FromRoute] Guid FoodId, [FromForm] FoodDTO.UpdateFood request)
	{
        var food = await _foodService.GetById(FoodId);

		if (food == null)
		{
			return StatusCode(
				StatusCodes.Status404NotFound,
				new ErrorResponseDTO
				{
					Message = "Food update failed",
					Error = $"Food with ID {FoodId} not found",
				}
			);
		}

        var karenderya = await _karenderyaService.GetById(food.KarenderyaId);

		if (karenderya == null)
		{
			return StatusCode(
				StatusCodes.Status404NotFound,
				new ErrorResponseDTO
				{
					Message = "Food update failed",
					Error = $"karenderya with ID {food.KarenderyaId} not found",
				}
			);
		}

        var userId = _userService.GetUserIdFromToken(User);
		if (userId != karenderya.UserId)
		{
			return StatusCode(
				StatusCodes.Status403Forbidden,
				new ErrorResponseDTO
				{
					Message = "Food delete failed",
					Error = $"You are not the owner of this karenderya with ID {food.KarenderyaId}",
				}
			);
		}

        food = await _foodService.Update(food, request);

		return StatusCode(
			StatusCodes.Status200OK,
			new SuccessResponseDTO { Message = "Food update success", Data = food }
		);
	}

	[HttpDelete("{FoodId}/delete")]
	[Authorize(Policy = "OwnerPolicy")]
	public async Task<IActionResult> Delete([FromRoute] Guid FoodId)
	{
        var userId = _userService.GetUserIdFromToken(User);
        var food = await _foodService.GetById(FoodId);

		if (food == null)
		{
			return StatusCode(
				StatusCodes.Status404NotFound,
				new ErrorResponseDTO
				{
					Message = "Food delete failed",
					Error = $"Food with ID {FoodId} not found",
				}
			);
		}
        var karenderya = await _karenderyaService.GetById(food.KarenderyaId);

		if (karenderya == null)
		{
			return StatusCode(
				StatusCodes.Status404NotFound,
				new ErrorResponseDTO
				{
					Message = "Food delete failed",
					Error = $"karenderya with ID {food.KarenderyaId} not found",
				}
			);
		}

		if (userId != karenderya.UserId)
		{
			return StatusCode(
				StatusCodes.Status403Forbidden,
				new ErrorResponseDTO
				{
					Message = "Food delete failed",
					Error = "You are not the owner of this karenderya",
				}
			);
		}

        await _foodService.Delete(food);

		return StatusCode(
			StatusCodes.Status200OK,
			new SuccessResponseDTO { Message = "Food deleted successfully" }
		);
	}
}
