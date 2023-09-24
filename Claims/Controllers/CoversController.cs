using Claims.BLL.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly ILogger<CoversController> _logger;
    private readonly IAuditerService _auditer;
    private readonly ICosmosDbService<Cover> _cosmosDbService;
    private readonly ICoversService _coversService;

    public CoversController(ICosmosDbService<Cover> cosmosDbService, IAuditerService auditer, ICoversService coversService, ILogger<CoversController> logger)
    {
        _logger = logger;
        _auditer = auditer;
        _coversService = coversService;
        _cosmosDbService = cosmosDbService;
    }

    [Route("Premium")]
    [HttpPost]
    public async Task<ActionResult> ComputePremiumAsync(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        return Ok(_coversService.ComputePremium(startDate, endDate, coverType));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cover>>> GetAsync()
    {
        return Ok(await _cosmosDbService.GetItemsAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Cover>> GetAsync(string id)
    {
        try
        {
            return Ok(await _cosmosDbService.GetItemAsync(id));
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(Cover cover)
    {
        cover.Id = Guid.NewGuid().ToString();
        cover.Premium = _coversService.ComputePremium(cover.StartDate, cover.EndDate, cover.Type);
        await _cosmosDbService.AddItemAsync(cover);
        await _auditer.AuditCover(cover.Id, "POST");
        return Ok(cover);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(string id)
    {
        await _auditer.AuditCover(id, "DELETE");
        await _cosmosDbService.DeleteItemAsync(id);
        return Ok();
    }

    
}