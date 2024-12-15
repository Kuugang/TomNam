using System.Security.Claims;
using TomNam.Exceptions;
using TomNam.Interfaces;
using TomNam.Models;
using TomNam.Models.DTO;

namespace TomNam.Services
{
	public class FoodService : IFoodService
	{
		private readonly IUserService _userService;
		private readonly IKarenderyaService _karenderyaService;
		private readonly IFoodRepository _foodRepository;
		private readonly IFileUploadService _uploadService;
		public FoodService(IUserService userService, IKarenderyaService karenderyaService, IFoodRepository foodRepository, IFileUploadService uploadService)
		{
			_userService = userService;
			_karenderyaService = karenderyaService;
			_foodRepository = foodRepository;
			_uploadService = uploadService;
		}

		public async Task<Food> GetById(Guid Id)
		{
			var Food = await _foodRepository.GetById(Id);
			if (Food == null)
			{
				throw new ApplicationExceptionBase(
					$"Food with ID {Id} not found",
					"Food creation failed",
					StatusCodes.Status404NotFound
				);
			}
			return Food;
		}
		
		public async Task<Food> GetByIdWithKarenderya(Guid Id)
        {
            var Food = await _foodRepository.GetByIdWithKarenderya(Id);
			if (Food == null)
			{
				throw new ApplicationExceptionBase(
					$"Food with ID {Id} not found",
					"Food creation failed",
					StatusCodes.Status404NotFound
				);
			}
			return Food;
        }

		public async Task<Food> Create(Guid KarenderyaId, FoodDTO.CreateFood request, ClaimsPrincipal User)
		{
			var UserId = _userService.GetUserIdFromToken(User);
			var Karenderya = await _karenderyaService.GetById(KarenderyaId);

			if (Karenderya == null)
			{
				throw new ApplicationExceptionBase(
					$"Karenderya with ID {KarenderyaId} not found",
					"Food creation failed",
					StatusCodes.Status404NotFound
				);
			}

			if (UserId != Karenderya.UserId)
			{
				throw new ApplicationExceptionBase(
					$"You are not the owner of this karenderya with ID {KarenderyaId}",
					"Food creation failed",
					StatusCodes.Status403Forbidden
				);
			}

			if (!await UniqueFoodName(KarenderyaId, request.FoodName))
			{
				throw new ApplicationExceptionBase(
					"Food name already exists",
					"Food creation failed",
					StatusCodes.Status409Conflict
				);
			}

			string FoodPhotoPath = _uploadService.Upload(request.FoodPhoto, "Food\\Photo");
			var Food = new Food
			{
				KarenderyaId = Karenderya.Id,
				Karenderya = Karenderya,
				FoodName = request.FoodName,
				FoodDescription = request.FoodDescription ?? "",
				UnitPrice = request.UnitPrice,
				FoodPhoto = FoodPhotoPath,
			};

			await _foodRepository.Create(Food);
			return Food;
		}

		public Task<bool> UniqueFoodName(Guid KarenderyaId, string FoodName)
		{
			return _foodRepository.UniqueFoodName(KarenderyaId, FoodName);
		}

		public async Task<List<Food>> FilterFood(FoodDTO.ReadFood filter)
		{
			var Foods = await _foodRepository.FilterFood(filter);
			return Foods;
		}

		public async Task<Food> Update(Guid FoodId, FoodDTO.UpdateFood request, ClaimsPrincipal User)
		{
			var Food = await GetById(FoodId);

			if (Food == null)
			{
				throw new ApplicationExceptionBase(
					$"Food with ID {FoodId} not found",
					"Food update failed",
					StatusCodes.Status404NotFound
				);
			}

			var karenderya = await _karenderyaService.GetById(Food.KarenderyaId);

			var userId = _userService.GetUserIdFromToken(User);
			if (userId != karenderya!.UserId)
			{
				throw new ApplicationExceptionBase(
					$"You are not the owner of this karenderya with ID {Food.KarenderyaId}",
					"Food update failed",
					StatusCodes.Status403Forbidden
				);
			}

			if (request.FoodName != null)
			{
				Food.FoodName = request.FoodName;
			}
			if (request.FoodDescription != null)
			{
				Food.FoodDescription = request.FoodDescription;
			}
			if (request.UnitPrice != null)
			{
				Food.UnitPrice = (double)request.UnitPrice;
			}

			if (request.FoodPhoto != null)
			{
				string FoodPhotoPath = _uploadService.Upload(request.FoodPhoto, "Food\\Photo");
				Food.FoodPhoto = FoodPhotoPath;
			}
			await _foodRepository.Update(Food);
			return Food;
		}
		public async Task Delete(Guid FoodId, ClaimsPrincipal User)
		{
			var userId = _userService.GetUserIdFromToken(User);
			var food = await GetById(FoodId);

			if (food == null)
			{
				throw new ApplicationExceptionBase(
					$"Food with ID {FoodId} not found",
					"Food delete failed",
					StatusCodes.Status404NotFound
				);
			}

			var karenderya = await _karenderyaService.GetById(food.KarenderyaId);

			if (userId != karenderya!.UserId)
			{
				throw new ApplicationExceptionBase(
					$"You are not the owner of this karenderya with ID {food.KarenderyaId}",
					"Food delete failed",
					StatusCodes.Status403Forbidden
				);
			}

			await _foodRepository.Delete(food);
		}
	}
}