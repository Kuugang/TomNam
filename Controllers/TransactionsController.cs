using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TomNam.Models.DTO;
using TomNam.Interfaces;

[ApiController]
[Route("api/transaction")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionController(
        ITransactionService transactionService
    )
    {
        _transactionService = transactionService;
    }

    [HttpPost]
    [Authorize(Policy = "OwnerPolicy")]
    public async Task<IActionResult> CreateTransaction([FromBody] TransactionRequestDTO.Create request)
    {
        var Transaction = await _transactionService.CreateTransaction(request, User);

        return StatusCode(StatusCodes.Status201Created, new SuccessResponseDTO
        {
            Message = "Transaction created successfully",
            Data = Transaction
        });
    }

    [HttpGet]
    [Authorize(Policy = "CustomerPolicy")]
    public async Task<IActionResult> GetAllCustomerTransactions()
    {
        var userTransactions = await _transactionService.GetAllCustomerTransactions(User);

        return Ok(new SuccessResponseDTO
        {
            Message = "Transaction retrieved successfully",
            Data = userTransactions
        });
    }
}