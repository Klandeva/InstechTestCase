using Claims.BLL.Services.IServices;
using Claims.Tests.ServicesTest;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Claims.Tests
{
    public class CoversControllerTests
    {
        private readonly WebApplicationFactory<Program> _application;
        private readonly HttpClient _httpClient;

        private readonly ICosmosDbService<Claim> _claimCosmosDbService;
        private readonly ICosmosDbService<Cover> _coverCosmosDbService;

        public CoversControllerTests() 
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
        public async Task Get_Covers_ReturnsSuccessStatusCode()
        {
            var response = await _httpClient.GetAsync("/Covers");

            response.EnsureSuccessStatusCode();
        }

        [InlineData(4)]
        [Theory]
        public async Task Get_Covers_ReturnsAllItems(int itemsCount)
        {
            var response = await _httpClient.GetAsync("/Covers");
            var responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            IEnumerable<Cover> res = JsonConvert.DeserializeObject<IEnumerable<Cover>>(responseString);

            res.Count().Should().Be(itemsCount);
        }

        [Fact]
        public async Task Get_Covers_ById_ReturnsSuccessStatusCode()
        {
            var response = await _httpClient.GetAsync("/Covers/4321");
            response.EnsureSuccessStatusCode();
        }

        [InlineData("4321")]
        [Theory]
        public async Task Get_Covers_ById_ReturnsItemById(string id)
        {
            var response = await _httpClient.GetAsync($"/Covers/{id}");
            var responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            Cover res = JsonConvert.DeserializeObject<Cover>(responseString);

            res.Id.Should().Be(id);
        }

        [Fact]
        public async Task Add_Cover_ReturnsSuccessStatusCode()
        {
            Cover cover = new Cover()
            {
                Id = "8642",
                StartDate = new DateOnly(2099,1,1),
                EndDate = new DateOnly(2099,12,31),
                Premium = 100,
                Type = CoverType.BulkCarrier
            };
            var response = await _httpClient.PostAsJsonAsync("/Covers", cover);
            response.EnsureSuccessStatusCode();
        }

        // StartDate cannot be in the Past
        [Fact]
        public async Task Add_Cover_ValidateStartDate()
        {
            Cover cover = new Cover()
            {
                Id = "8642",
                StartDate = new DateOnly(2000, 1, 1),
                EndDate = new DateOnly(2000, 12, 31),
                Premium = 100,
                Type = CoverType.BulkCarrier
            };
            var response = await _httpClient.PostAsJsonAsync("/Covers", cover);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        // total insurance period cannot exceed 1 year
        [Fact]
        public async Task Add_Cover_ValidateTotalInsurancePeriod()
        {
            Cover cover = new Cover()
            {
                Id = "8642",
                StartDate = new DateOnly(2099, 1, 1),
                EndDate = new DateOnly(2101, 12, 31),
                Premium = 100,
                Type = CoverType.BulkCarrier
            };
            var response = await _httpClient.PostAsJsonAsync("/Covers", cover);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Delete_Cover_ReturnsSuccessStatusCode()
        {
            var response = await _httpClient.DeleteAsync($"/Covers/4321");
            response.EnsureSuccessStatusCode();
        }
    }
}
