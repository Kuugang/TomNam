using TomNam.Interfaces;
using TomNam.Models;
using TomNam.Models.DTO;

namespace TomNam.Services{
    public class FoodService : IFoodService
    {
        private readonly IFoodRepository _foodRepository; 
        private readonly IFileUploadService _uploadService;
        public FoodService(IFoodRepository foodRepository, IFileUploadService uploadService){
            _foodRepository = foodRepository;
            _uploadService = uploadService;
        }

        public async Task<Food?> GetById(Guid Id){
            return await _foodRepository.GetById(Id);
        }

        public async Task<Food> Create(Karenderya Karenderya, FoodDTO.CreateFood request){
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

        public async Task<List<FoodDTO.ReadFood>> FilterFood(FoodDTO.ReadFood filter){
            var foods = await _foodRepository.FilterFood(filter);
            var response = foods.Select(food => new FoodDTO.ReadFood
            {
                Id = food.Id,
                KarenderyaId = food.KarenderyaId.ToString(),
                FoodName = food.FoodName,
                FoodDescription = food.FoodDescription,
                UnitPrice = food.UnitPrice,
                FoodPhoto = food.FoodPhoto,
            }).ToList();
            return response;
        }

        public async Task<Food> Update(Food food, FoodDTO.UpdateFood request){
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
            await _foodRepository.Update(food);
            return food;
        }
        public async Task Delete(Food food){
            await _foodRepository.Delete(food);
        }
    }
}