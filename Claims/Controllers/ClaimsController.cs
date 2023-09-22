using Claims.BLL.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController : ControllerBase
    {
        
        private readonly ILogger<ClaimsController> _logger;
        private readonly ICosmosDbService<Claim> _claimCosmosDbService;
        private readonly ICosmosDbService<Cover> _coverCosmosDbService;
        private readonly IAuditerService _auditer;

        public ClaimsController(ILogger<ClaimsController> logger, ICosmosDbService<Claim> claimCosmosDbService, ICosmosDbService<Cover> coverCosmosDbService, IAuditerService auditer)
        {
            _logger = logger;
            _claimCosmosDbService = claimCosmosDbService;
            _coverCosmosDbService = coverCosmosDbService;
            _auditer = auditer;
        }

        [HttpGet]
        public Task<IEnumerable<Claim>> GetAsync()
        {
            return _claimCosmosDbService.GetItemsAsync();
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(Claim claim)
        {
            Cover cover = await _coverCosmosDbService.GetItemAsync(claim.CoverId);
            if (cover != null && (claim.Created < cover.StartDate.ToDateTime(TimeOnly.Parse("00:00")) || 
                                  claim.Created > cover.EndDate.ToDateTime(TimeOnly.Parse("00:00"))))
                return BadRequest("Created date must be within the period of the related Cover");


            claim.Id = Guid.NewGuid().ToString();
            await _claimCosmosDbService.AddItemAsync(claim);
            _auditer.AuditClaim(claim.Id, "POST");
            return Ok(claim);
        }

        [HttpDelete("{id}")]
        public Task DeleteAsync(string id)
        {
            _auditer.AuditClaim(id, "DELETE");
            return _claimCosmosDbService.DeleteItemAsync(id);
        }

        [HttpGet("{id}")]
        public Task<Claim> GetAsync(string id)
        {
            return _claimCosmosDbService.GetItemAsync(id);
        }
    }

}