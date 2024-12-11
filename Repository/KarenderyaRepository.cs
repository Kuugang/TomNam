using Microsoft.EntityFrameworkCore;

using TomNam.Models;
using TomNam.Data;
using TomNam.Interfaces;
using TomNam.Models.DTO;

namespace TomNam.Repository
{
    public class KarenderyaRepository : IKarenderyaRepository
    {
        private readonly DataContext _context;

        public KarenderyaRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<Karenderya> Create(Karenderya karenderya)
        {
            await _context.Karenderya.AddAsync(karenderya);
            await _context.SaveChangesAsync();
            return karenderya;
        }

        public async Task<Karenderya?> GetByIdAsync(Guid id)
        {
            var karenderya = await _context.Karenderya.FirstOrDefaultAsync(k => k.Id == id);
            return karenderya;
        }
        public async Task<Karenderya?> GetByOwnerId(string id)
        {
            var karenderya = await _context.Karenderya.FirstOrDefaultAsync(k => k.UserId == id);
            return karenderya;
        }

        public async Task Update(Karenderya karenderya)
        {
            _context.Karenderya.Update(karenderya);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Karenderya>> FilterKarenderya(KarenderyaRequestDTO.Read filters)
        {

            var query = _context.Karenderya.AsQueryable();

            if (filters.KarenderyaId != null)
            {
                query = query.Where(k => k.Id == filters.KarenderyaId);
            }

            if (!string.IsNullOrEmpty(filters.Name))
            {
                query = query.Where(k => k.Name.ToLower().Contains(filters.Name.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.LocationStreet))
            {
                query = query.Where(k => k.LocationStreet.ToLower().Contains(filters.LocationStreet.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.LocationBarangay))
            {
                query = query.Where(k => k.LocationBarangay.ToLower().Contains(filters.LocationBarangay.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.LocationCity))
            {
                query = query.Where(k => k.LocationCity.ToLower().Contains(filters.LocationCity.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.LocationProvince))
            {
                query = query.Where(k => k.LocationProvince.ToLower().Contains(filters.LocationProvince.ToLower()));
            }

            return await query.ToListAsync();
        }

        public async Task CreateProofOfBusiness(ProofOfBusiness proofOfBusiness){
		    await _context.ProofOfBusiness.AddAsync(proofOfBusiness);
		    await _context.SaveChangesAsync();
        }

        public async Task <ProofOfBusiness?> GetProofOfBusiness(Guid karenderyaId){
            return await _context.ProofOfBusiness.FirstOrDefaultAsync(p => p.KarenderyaId == karenderyaId);
        }

        public async Task UpdateProofOfBusiness(ProofOfBusiness proofOfBusiness){
            _context.ProofOfBusiness.Update(proofOfBusiness);
            await _context.SaveChangesAsync();
        }
    }
}
