using WebApiTestesSoftware.Models;

namespace WebApiTestesSoftware.Services
{
    public interface ITransactionService
    {
        Task<bool> ProcessTransaction(Transaction transaction);
        decimal GetBalance(int accountId);
    }

    public class TransactionService : ITransactionService
    {
        private readonly IExternalValidationService _validationService;
        private readonly Dictionary<int, decimal> _accounts = new Dictionary<int, decimal>();

        public TransactionService(IExternalValidationService validationService)
        {
            _validationService = validationService;
        }

        public async Task<bool> ProcessTransaction(Transaction transaction)
        {
            // Valida a transação com o serviço externo
            bool isValid = await _validationService.ValidateTransactionAsync(transaction);

            if (!isValid)
            {
                return false; // Se a validação falhar, retorna false
            }

            // Processa a transação internamente
            if (!_accounts.ContainsKey(transaction.AccountId))
            {
                _accounts[transaction.AccountId] = 0;
            }

            if (transaction.Type == "deposit")
            {
                _accounts[transaction.AccountId] += transaction.Amount;
                return true;
            }
            else if (transaction.Type == "withdrawal" && _accounts[transaction.AccountId] >= transaction.Amount)
            {
                _accounts[transaction.AccountId] -= transaction.Amount;
                return true;
            }

            return false;
        }

        public decimal GetBalance(int accountId)
        {
            return _accounts.ContainsKey(accountId) ? _accounts[accountId] : 0;
        }
    }

}
