using System.Text.Json;
using WebApiTestesSoftware.Models;

namespace WebApiTestesSoftware.Services
{
    public interface IExternalValidationService
    {
        Task<bool> ValidateTransactionAsync(Transaction transaction);
    }

    public class ExternalValidationService : IExternalValidationService
    {
        private readonly HttpClient _httpClient;

        public ExternalValidationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> ValidateTransactionAsync(Transaction transaction)
        {
            var response = await _httpClient.PostAsJsonAsync("http://localhost:5000/validateTransaction", transaction);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ExternalServiceResult>();
                Console.WriteLine(JsonSerializer.Serialize(result));
                return result.Status == "success";
            }

            return false;
        }
    }

}
