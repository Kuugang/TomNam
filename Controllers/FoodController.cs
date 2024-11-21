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
public class FoodController : ControllerBase {
    
    private readonly DataContext _context;
    private readonly IUserService _userService;

    public FoodController(DataContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    [HttpPost("{karenderyaId}")]
    [Authorize(Policy="OwnerPolicy")]
    public async Task<IActionResult> Create([FromBody] FoodDTO request,[FromRoute] Guid karenderyaId,[FromQuery] FoodDTO search){
        var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (UserId == null) {
            return NotFound(new { message = "User not found" });
        }

        var Karenderya = await _context.Karenderya.FirstOrDefaultAsync(k => k.Id == karenderyaId && k.UserId == UserId);

        if (Karenderya == null) {
            return NotFound(new { message = "Karenderya not found" });
        }
        
        // TODO: unique food names. 
        // var foodName = await _context.Food.SingleOrDefaultAsync(f => f.FoodName == request.FoodName);
        // if(foodName)

        var Food = new Food {
            KarenderyaId = Karenderya.Id,
            Karenderya = Karenderya,
            FoodName = request.FoodName,
            FoodDescription = (request.FoodDescription == null) ? "" : request.FoodDescription,
            UnitPrice = request.UnitPrice,
            FoodPhoto = request.FoodPhoto
        };

        await _context.Food.AddAsync(Food);
        await _context.SaveChangesAsync();

        return Ok(Food);

    }

    // get by id ra
    [HttpGet("{FoodId}")]
    public async Task<IActionResult> Get([FromRoute] Guid FoodId){
        var food = await _context.Food.SingleOrDefaultAsync(f => f.Id == FoodId);
    
        if (food == null) {
            return NotFound(new { message = "Food not found" });
        }

        return Ok(food);
    }

    // search food
    [HttpGet()]
    public async Task<IActionResult> GetFromSearch([FromQuery] FoodDTO search){
        var query = _context.Food.AsQueryable();
    
        if(search.FoodName != null) {
            query = query.Where(f => f.FoodName.ToLower().Contains(search.FoodName.ToLower()));
        }

        var foodlist = await query.ToListAsync();
        if (!foodlist.Any()) {
            return NotFound(new { message = "Food not found" });
        }

        return Ok(foodlist);
    }
}