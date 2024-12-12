
using System.Security.Claims;
using TomNam.Models;
namespace TomNam.Interfaces
{
    public interface ITransactionRepository
    {
        Task CreateTransaction(Transaction Transaction);
        Task<List<Transaction>> GetAllCustomerTransactions(CustomerProfile customerProfile);
    }
}