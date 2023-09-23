using Claims.BLL.Services.IServices;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Claims.Tests
{
    public class CoversServiceTests
    {
        private readonly WebApplicationFactory<Program> _application;
        private readonly ICoversService _coversService;

        public CoversServiceTests() 
        {
            _application = new WebApplicationFactory<Program>()
                    .WithWebHostBuilder(_ =>
                    { });

            _coversService = _application.Services.GetService<ICoversService>();
        }

        // Yacht
        [InlineData("2100-01-01", "2100-01-21", CoverType.Yacht, 27500)] // 20 days
        [InlineData("2100-01-01", "2100-02-20", CoverType.Yacht, 67375)] // 50 days
        [InlineData("2100-01-01", "2100-07-20", CoverType.Yacht, 262487.500)] // 200 days
        // Tanker
        [InlineData("2100-01-01", "2100-01-26", CoverType.Tanker, 46875)] // 25 days
        [InlineData("2100-01-01", "2100-03-02", CoverType.Tanker, 111375)] // 60 days
        [InlineData("2100-01-01", "2100-10-28", CoverType.Tanker, 550125)] // 300 days
        [Theory]
        public async Task ComputePremium(string startDate, string endDate, CoverType coverType, decimal computedPremium)
        {
            var response = _coversService.ComputePremium(DateOnly.Parse(startDate), DateOnly.Parse(endDate), coverType); 
            response.Should().Be(computedPremium);
        }
    }
}
