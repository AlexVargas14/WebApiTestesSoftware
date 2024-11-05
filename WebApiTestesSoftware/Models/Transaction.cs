namespace WebApiTestesSoftware.Models
{
    public class Transaction
    {
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } // "deposit" ou "withdrawal"
        public DateTime Date { get; set; }
    }

}
