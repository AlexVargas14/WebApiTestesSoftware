using Microsoft.AspNetCore.Mvc;
using WebApiTestesSoftware.Models;
using WebApiTestesSoftware.Services;

namespace WebApiTestesSoftware.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessTransaction([FromBody] Transaction transaction)
        {
            if (await _transactionService.ProcessTransaction(transaction))
            {
                return Ok();
            }
            return BadRequest("Transação falhou");
        }

        [HttpGet("{accountId}")]
        public IActionResult GetBalance(int accountId)
        {
            var balance = _transactionService.GetBalance(accountId);
            return Ok(balance);
        }
    }

}