using Newtonsoft.Json;
using RestSharp;
using System.Net;
using System.Text;
using WebApiTestesSoftware.Models;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace WebApiTestesSoftware.IntegrationTests
{
    [TestFixture]
    public class TransactionControllerIntegrationTests
    {
        private WireMockServer _server;
        private RestClient _client;

        [SetUp]
        public void SetUp()
        {
            // Inicia o WireMock server em uma porta livre
            _server = WireMockServer.Start(5000);

            // Define o client RestSharp
            _client = new RestClient(_server.Url);
        }

        [Test]
        public void Test_Transaction_Should_Succeed_With_External_Service_Validation()
        {
            var modelResult = new ExternalServiceResult()
            {
                Status = "success",
                Message = "Ok"
            };
            // Simula a resposta do serviço externo
            _server.Given(Request.Create().WithPath("/validateTransaction").UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithBody(JsonConvert.SerializeObject(modelResult)));

            // Faz uma requisição simulada
            var request = new RestRequest("http://localhost:5186/api/Transaction", Method.Post);
            request.AddJsonBody(new
            {
                accountId = 1,
                amount = 100,
                type = "deposit",
                date = DateTime.Now
            });

            var response = _client.Execute(request);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void Test_Transaction_Should_Fail_With_External_Service_Validation_Failure()
        {
            // Simula a falha no serviço externo
            _server.Given(Request.Create().WithPath("/validateTransaction").UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithBody("{\"status\":\"error\",\"message\":\"Transaction declined\"}"));

            // Faz uma requisição simulada
            var request = new RestRequest("http://localhost:5186/api/Transaction", Method.Post);
            request.AddJsonBody(new
            {
                AccountId = 1,
                Amount = 2000, // Supondo que esse valor seja recusado
                Type = "withdrawal",
                Date = DateTime.Now
            });

            var response = _client.Execute(request);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TearDown]
        public void TearDown()
        {
            // Para o servidor WireMock quando os testes terminarem
            _server.Stop();
        }
    }
}