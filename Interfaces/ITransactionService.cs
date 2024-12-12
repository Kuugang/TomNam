using System.Security.Claims;
using TomNam.Models;
using TomNam.Models.DTO;
namespace TomNam.Interfaces
{
    public interface ITransactionService
    {
        Task<Transaction> CreateTransaction(TransactionRequestDTO.Create request, ClaimsPrincipal user);
        Task<List<Transaction>> GetAllCustomerTransactions(ClaimsPrincipal User);
    }
}