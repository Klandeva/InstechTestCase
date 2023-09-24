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
        public async Task<ActionResult<IEnumerable<Claim>>> GetAsync()
        {
            var result = await _claimCosmosDbService.GetItemsAsync();
            return Ok(result);
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
            await _auditer.AuditClaim(claim.Id, "POST");
            return Ok(claim);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            await _auditer.AuditClaim(id, "DELETE");
            await _claimCosmosDbService.DeleteItemAsync(id);
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Claim>> GetAsync(string id)
        {
            var result = await _claimCosmosDbService.GetItemAsync(id);
            return Ok(result);
        }
    }

}