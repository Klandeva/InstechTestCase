using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using Xunit;
using System.Net;
using Newtonsoft.Json;
using Claims.BLL.Services.IServices;
using Claims.Tests.ServicesTest;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Claims.Tests
{
    public class ClaimsControllerTests
    {
        private readonly WebApplicationFactory<Program> _application;
        private readonly HttpClient _httpClient;

        private readonly ICosmosDbService<Claim> _claimCosmosDbService;
        private readonly ICosmosDbService<Cover> _coverCosmosDbService;

        public ClaimsControllerTests() 
        {
            _claimCosmosDbService = new ClaimsCosmosDbServiceTest();
            _coverCosmosDbService = new CoverCosmosDbServiceTest();

            _application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services.AddSingleton(_claimCosmosDbService);
                        services.AddSingleton(_coverCosmosDbService);
                        services.AddScoped<IAuditerService, AuditerServiceTest>();
                    });
                });
            _httpClient = _application.CreateClient();
        }

        [Fact]
        public async Task Get_Claims_ReturnsSuccessStatusCode()
        {
            var response = await _httpClient.GetAsync("/Claims");
            
            response.EnsureSuccessStatusCode();   
        }

        [InlineData(3)]
        [Theory]
        public async Task Get_Claims_ReturnsAllItems(int itemsCount)
        {
            var response = await _httpClient.GetAsync("/Claims");
            var responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            IEnumerable<Claim> res = JsonConvert.DeserializeObject<IEnumerable<Claim>>(responseString);

            res.Count().Should().Be(itemsCount);
        }

        [Fact]
        public async Task Get_Claims_ById_ReturnsSuccessStatusCode()
        {
            var response = await _httpClient.GetAsync("/Claims/1234");
            response.EnsureSuccessStatusCode();
        }

        [InlineData("1234")]
        [Theory]
        public async Task Get_Claims_ById_ReturnsItemById(string id)
        {
            var response = await _httpClient.GetAsync($"/Claims/{id}");
            var responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            Claim res = JsonConvert.DeserializeObject<Claim>(responseString);

            res.Id.Should().Be(id);
        }

        [Fact]
        public async Task Add_Claim_ReturnsSuccessStatusCode()
        {
            Claim claim = new Claim() 
            {
                Id = "1357",
                CoverId = "7531",
                Name = "ClaimTest",
                Created = DateTime.Now,
                DamageCost = 0, 
                Type = ClaimType.Fire
            };
            var response = await _httpClient.PostAsJsonAsync("/Claims", claim);
            response.EnsureSuccessStatusCode();
        }

        // DamageCost exceed 100.000
        [InlineData("The field DamageCost must be between 0 and 100000.")]
        [Theory]
        public async Task Add_Claim_ValidateDamageCostProperty(string error)
        {
            Claim claim = new Claim()
            {
                Id = "1357",
                CoverId = "7531",
                Name = "ClaimTest",
                Created = DateTime.Now,
                DamageCost = 100001,
                Type = ClaimType.Fire
            };

            var response = await _httpClient.PostAsJsonAsync("/Claims", claim);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var responseJObject = JObject.Parse(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
            var actualError = (string)((JObject)responseJObject["errors"])["DamageCost"].First;
            actualError.Should().Be(error);
        }

        // Created date is not within the period of the related Cover
        [InlineData("Created date must be within the period of the related Cover")]
        [Theory]
        public async Task Add_Claim_ValidateCreatedDateProperty(string error)
        {
            Claim claim = new Claim()
            {
                Id = "1357",
                CoverId = "7531",
                Name = "ClaimTest",
                Created = new DateTime(2024,7,31,12,21,0),
                DamageCost = 100000,
                Type = ClaimType.Fire
            };

            var response = await _httpClient.PostAsJsonAsync("/Claims", claim);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var actualError = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            actualError.Should().Be(error);
        }

        [Fact]
        public async Task Delete_Claim_ReturnsSuccessStatusCode()
        {
            var response = await _httpClient.DeleteAsync($"/Claims/1234");
            response.EnsureSuccessStatusCode();
        }

    }
}
