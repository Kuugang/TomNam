using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using TomNam.Models;
using TomNam.Models.DTO;
using TomNam.Data;
using TomNam.Interfaces;
using TomNam.Middlewares;


namespace TomNam.Services
{
    public class KarenderyaService : IKarenderyaService
    {
        private readonly IKarenderyaRepository _karenderyaRepository;
        
        public KarenderyaService(IUserRepository userRepository, JwtAuthenticationService jwtAuthenticationService, UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _jwtAuthenticationService = jwtAuthenticationService;
            _userManager = userManager;
        }
        public async Task<Karenderya> Create(KarenderyaRequestDTO.Create request, User user)
        {
            var karenderya = new Karenderya
		    {
			UserId = user.Id,
			User = user,
			Name = request.Name,
			LocationStreet = request.LocationStreet,
			LocationBarangay = request.LocationBarangay,
			LocationCity = request.LocationCity,
			LocationProvince = request.LocationProvince,
			Description = request.Description,
			DateFounded = request.DateFounded
		    };
            
            return await _karenderyaRepository.Create(karenderya);
        }
    }
}
