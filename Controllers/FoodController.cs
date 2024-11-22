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
    public async Task<IActionResult> Create([FromBody] FoodDTO.CreateFood request,[FromRoute] Guid karenderyaId){
        var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (UserId == null) {
            return NotFound(new { message = "User not found" });
        }

        var Karen = await _context.Karenderya.FirstOrDefaultAsync(k => k.Id == karenderyaId && k.UserId == UserId);

        if (Karen == null) {
            return NotFound(new { message = "Karenderya not found" });
        }
        
        // unique food names. 
        var foodCount = await _context.Food.CountAsync(f => f.FoodName == request.FoodName);
        if (foodCount >= 1)
        {
            return Conflict(new { message = "Food name already exists" });
        }

        var Food = new Food {
            KarenderyaId = Karen.Id,
            Karenderya = Karen,
            FoodName = request.FoodName,
            FoodDescription = (request.FoodDescription == null) ? "" : request.FoodDescription,
            UnitPrice = request.UnitPrice,
            FoodPhoto = request.FoodPhoto
        
        };

        await _context.Food.AddAsync(Food);
        await _context.SaveChangesAsync();

        return CreatedAtRoute("GetFoodById", new { foodId = Food.Id },Food);
    }

    // get by id ra
    [HttpGet("{foodId}", Name = "GetFoodById")]
    public async Task<IActionResult> Get([FromRoute] Guid FoodId){
        var food = await _context.Food.SingleOrDefaultAsync(f => f.Id == FoodId);
    
        if (food == null) {
            return NotFound(new { message = "Food not found" });
        }

        return Ok(new {message = "Search success", data = food });
    }

    // search food at karenderya
    [HttpGet()]
    public async Task<IActionResult> GetFromSearch([FromQuery] FoodDTO.ReadUpdateFood search){
        var query = _context.Food.AsQueryable();
    
        if(search.FoodName != null) {
            query = query.Where(f => f.FoodName.ToLower().Contains(search.FoodName.ToLower()));
        }

        var foodlist = await query.ToListAsync();
        if (!foodlist.Any()) {
            return NotFound(new { message = "Food not found" });
        }

        return Ok(new {message = "Search success", data = foodlist });
    }

    //TODO: di mo gana. 415
    [HttpPut("{FoodId}/update")]
    [Authorize(Policy="OwnerPolicy")]
    public async Task<IActionResult> Update([FromRoute] Guid FoodId, [FromForm] FoodDTO.ReadUpdateFood request){
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var food = await _context.Food.FirstOrDefaultAsync(f => f.Id == FoodId);

        if (food == null) {
            return NotFound(new { message = "Food not found" });
        }

        var karenderya = await _context.Karenderya.FirstOrDefaultAsync(k => k.Id == food.KarenderyaId);

        if(karenderya == null) {
            return NotFound(new { message = "Karenderya not found" });
        }

        // TODO: mo null sya diri dapita??
        if (userId != karenderya.UserId) {
            return Unauthorized(new { message = "You are not the owner of this karenderya" });
        }

        // Update food but nanuon nlng kos karenderya update pud
        if(request.FoodName != null) {
            food.FoodName = request.FoodName;
        } if(request.FoodDescription != null) {
            food.FoodDescription = request.FoodDescription;
        } if(request.UnitPrice != food.UnitPrice) {
            food.UnitPrice = request.UnitPrice;
        } 
        
        // //TODO: 
        // if(request.FoodPhoto != null) {
        //     food.FoodPhoto = request.FoodPhoto;
        // }

        await _context.SaveChangesAsync();

        return Ok(food);
    }

    [HttpDelete("{FoodId}/delete")]
    [Authorize(Policy="OwnerPolicy")]
    public async Task<IActionResult> Delete([FromRoute] Guid FoodId){
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var food = await _context.Food.FirstOrDefaultAsync(f => f.Id == FoodId);

        if (food == null) {
            return NotFound(new { message = "Food not found" });
        }
        
        var karenderya = await _context.Karenderya.FirstOrDefaultAsync(k => k.Id == food.KarenderyaId);

        if(karenderya == null) {
            return NotFound(new { message = "Karenderya not found" });
        }

        // TODO: mo null sya diri dapita??
        if (userId != karenderya.UserId) {
            return Unauthorized(new { message = "You are not the owner of this karenderya" });
        }

        _context.Food.Remove(food);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Food deleted successfully" });
    }
}