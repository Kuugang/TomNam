using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using TomNam.Models;
using TomNam.Data;
using TomNam.Interfaces;
using System.Security.Claims;

namespace TomNam.Repository
{
	public class TransactionRepository : ITransactionRepository
	{
		private readonly DataContext _context;
		private readonly UserManager<User> _userManager;
		
		public TransactionRepository(DataContext context, UserManager<User> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

        public async Task CreateTransaction(Transaction Transaction)
        {
            await _context.Transaction.AddAsync(Transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Transaction>> GetAllCustomerTransactions(CustomerProfile customerProfile)
        {
            var userTransactions = await _context.Transaction
                .Include(t => t.Reservation)
                .ThenInclude(r => r.Customer)
                .Where(t => t.Reservation.CustomerProfileId == customerProfile!.Id)
                .ToListAsync();

            return userTransactions;
        }
    }
}