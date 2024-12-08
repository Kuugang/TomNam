using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using TomNam.Models;
using TomNam.Data;
using TomNam.Interfaces;

namespace TomNam.Repository
{
    public class KarenderyaRepository : IKarenderyaRepository
    {
        private readonly DataContext _context;
       
        public UserRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<Karenderya> Create(Karenderya karenderya){
            await _context.Karenderya.AddAsync(karenderya);
		    await _context.SaveChangesAsync();
            return karenderya;
        }
    }
}
