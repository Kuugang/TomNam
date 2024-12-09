using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TomNam.Data;
using TomNam.Interfaces;
using TomNam.Middlewares;
using TomNam.Models;
using TomNam.Models.DTO;

[ApiController]
[Route("api/karenderyas/foods")]
public class FoodController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IUserService _userService;
    private readonly IFileUploadService _uploadService;

    public FoodController(
        DataContext context,
        IUserService userService,
        IFileUploadService uploadService
    )
    {
        _context = context;
        _userService = userService;
        _uploadService = uploadService;
    }

    [HttpPost("{karenderyaId}")]
    [Authorize(Policy = "OwnerPolicy")]
    public async Task<IActionResult> Create(
        [FromForm] FoodDTO.CreateFood request,
        [FromRoute] Guid karenderyaId
    )
    {
        var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (UserId == null)
        {
            return StatusCode(
                StatusCodes.Status404NotFound,
                new ErrorResponseDTO
                {
                    Message = "Food create failed",
                    Error = $"User with ID {UserId} not found",
                }
            );
        }

        var Karen = await _context.Karenderya.FirstOrDefaultAsync(k =>
            k.Id == karenderyaId && k.UserId == UserId
        );

        if (Karen == null)
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

        // unique food names.
        var foodCount = await _context.Food.CountAsync(f => f.FoodName == request.FoodName);
        if (foodCount >= 1)
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

        string FoodPhotoPath = _uploadService.Upload(request.FoodPhoto, "Food\\Photo");
        var Food = new Food
        {
            KarenderyaId = Karen.Id,
            Karenderya = Karen,
            FoodName = request.FoodName,
            FoodDescription = (request.FoodDescription == null) ? "" : request.FoodDescription,
            UnitPrice = request.UnitPrice,
            FoodPhoto = FoodPhotoPath,
        };

        await _context.Food.AddAsync(Food);
        await _context.SaveChangesAsync();

        return CreatedAtRoute("GetFoodById", new { foodId = Food.Id }, Food);
    }

    // get by id ra
    [HttpGet("{foodId}", Name = "GetFoodById")]
    public async Task<IActionResult> Get([FromRoute] Guid FoodId)
    {
        var food = await _context.Food.SingleOrDefaultAsync(f => f.Id == FoodId);

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

    // search food at karenderya
    [HttpGet()]
    public async Task<IActionResult> GetFromSearch([FromQuery] FoodDTO.ReadFood search)
    {
        var query = _context.Food.AsQueryable();

        if (search.FoodName != null)
        {
            query = query.Where(f => f.FoodName.ToLower().Contains(search.FoodName.ToLower()));
        }

        if (search.KarenderyaId != null)
        {
            query = query.Where(f => f.KarenderyaId.ToString() == search.KarenderyaId);
        }

        var foodlist = await query.ToListAsync();

        if (!foodlist.Any())
        {
            return StatusCode(
                StatusCodes.Status404NotFound,
                new ErrorResponseDTO { Message = "Food get failed", Error = "Food not found" }
            );
        }

        List<FoodDTO.ReadFood> responseFoods = new List<FoodDTO.ReadFood>();

        foreach (var food in foodlist)
        {
            responseFoods.Add(new FoodDTO.ReadFood
            {
                Id = food.Id,
                KarenderyaId = food.KarenderyaId.ToString(),
                FoodName = food.FoodName,
                FoodDescription = food.FoodDescription,
                UnitPrice = food.UnitPrice,
                FoodPhoto = food.FoodPhoto,
            });
        }

        return StatusCode(
            StatusCodes.Status200OK,
            new SuccessResponseDTO { Message = "Search success", Data =  responseFoods}
        );
    }

    [HttpPut("{FoodId}/update")]
    [Authorize(Policy = "OwnerPolicy")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid FoodId,
        [FromForm] FoodDTO.UpdateFood request
    )
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var food = await _context.Food.FirstOrDefaultAsync(f => f.Id == FoodId);

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

        var karenderya = await _context.Karenderya.FirstOrDefaultAsync(k =>
            k.Id == food.KarenderyaId
        );

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

        // Update food but nanuon nlng kos karenderya update pud
        if (request.FoodName != null)
        {
            food.FoodName = request.FoodName;
        }
        if (request.FoodDescription != null)
        {
            food.FoodDescription = request.FoodDescription;
        }
        if (request.UnitPrice != null)
        {
            food.UnitPrice = (double)request.UnitPrice;
        }

        if(request.FoodPhoto != null) {
            string FoodPhotoPath = _uploadService.Upload(request.FoodPhoto, "Food\\Photo");
            food.FoodPhoto = FoodPhotoPath;
        }

        await _context.SaveChangesAsync();

        return StatusCode(
            StatusCodes.Status200OK,
            new SuccessResponseDTO { Message = "Food update success", Data = food }
        );
    }

    [HttpDelete("{FoodId}/delete")]
    [Authorize(Policy = "OwnerPolicy")]
    public async Task<IActionResult> Delete([FromRoute] Guid FoodId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var food = await _context.Food.FirstOrDefaultAsync(f => f.Id == FoodId);

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

        var karenderya = await _context.Karenderya.FirstOrDefaultAsync(k =>
            k.Id == food.KarenderyaId
        );

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

        // TODO: mo null sya diri dapita??
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

        _context.Food.Remove(food);
        await _context.SaveChangesAsync();
        return StatusCode(
            StatusCodes.Status200OK,
            new SuccessResponseDTO { Message = "Food deleted successfully" }
        );
    }
}
