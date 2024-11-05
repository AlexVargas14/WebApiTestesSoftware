using Moq;
using WebApiTestesSoftware.Models;
using WebApiTestesSoftware.Services;

namespace WebApiTestesSoftware.UnitTest
{
    public class TransactionServiceTests
    {
        private Mock<ITransactionService> _transactionServiceMock;
        private TransactionService _transactionService;
        private Mock<IExternalValidationService> _externalServiceMock;

        [SetUp]
        public void SetUp()
        {
            _transactionServiceMock = new Mock<ITransactionService>();
            _externalServiceMock = new Mock<IExternalValidationService>();
            _transactionService = new TransactionService(_externalServiceMock.Object);
            _externalServiceMock.Setup(p => p.ValidateTransactionAsync(It.IsAny<Transaction>())).ReturnsAsync(true);    
        }

        [Test]
        public async Task ProcessTransaction_ShouldDepositMoneySuccessfully()
        {
            var transaction = new Transaction
            {
                AccountId = 1,
                Amount = 100,
                Type = "deposit",
                Date = DateTime.Now
            };

            var result = await _transactionService.ProcessTransaction(transaction);
            Assert.IsTrue(result);
        }

        [Test]
        public async Task ProcessTransaction_ShouldWithdrawMoneySuccessfully()
        {
            var transaction = new Transaction
            {
                AccountId = 1,
                Amount = 50,
                Type = "withdrawal",
                Date = DateTime.Now
            };

            // Configura a conta para ter 100 antes de retirar
           await  _transactionService.ProcessTransaction(new Transaction { AccountId = 1, Amount = 100, Type = "deposit", Date = DateTime.Now });

            var result = await _transactionService.ProcessTransaction(transaction);
            Assert.IsTrue(result);
        }

        [Test]
        public async Task ProcessTransaction_ShouldFailWhenInsufficientFunds()
        {
            var transaction = new Transaction
            {
                AccountId = 1,
                Amount = 150,
                Type = "withdrawal",
                Date = DateTime.Now
            };

            var result = await _transactionService.ProcessTransaction(transaction);
            Assert.IsFalse(result); // Saldo insuficiente
        }

        [Test]
        public async Task GetBalance_ShouldReturnCorrectBalance()
        {
            await _transactionService.ProcessTransaction(new Transaction { AccountId = 1, Amount = 100, Type = "deposit", Date = DateTime.Now });
            await _transactionService.ProcessTransaction(new Transaction { AccountId = 1, Amount = 50, Type = "withdrawal", Date = DateTime.Now });

            var balance = _transactionService.GetBalance(1);
            Assert.AreEqual(50, balance);
        }
    }
}