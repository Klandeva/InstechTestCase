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

    /// <summary>
    /// Compute and return cover premium for the insurance length and appropertiate cover type
    /// </summary>
    /// <param name="startDate">Cover start date</param>
    /// <param name="endDate">Cover end date</param>
    /// <param name="coverType">Cover Type</param>
    /// <returns>Calculated premium</returns>
    [Route("Premium")]
    [HttpPost]
    public async Task<ActionResult> ComputePremiumAsync(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        return Ok(_coversService.ComputePremium(startDate, endDate, coverType));
    }

    /// <summary>
    /// Get whole list of covers
    /// </summary>
    /// <returns>List of covers</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cover>>> GetAsync()
    {
        return Ok(await _cosmosDbService.GetItemsAsync());
    }

    /// <summary>
    /// Get cover per id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Cover</returns>
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

    /// <summary>
    /// Create Cover with the given properties
    /// </summary>
    /// <param name="cover"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> CreateAsync(Cover cover)
    {
        cover.Id = Guid.NewGuid().ToString();
        cover.Premium = _coversService.ComputePremium(cover.StartDate, cover.EndDate, cover.Type);
        await _cosmosDbService.AddItemAsync(cover);
        await _auditer.AuditCover(cover.Id, "POST");
        return Ok(cover);
    }

    /// <summary>
    /// Delete cover
    /// </summary>
    /// <param name="id">Cover id</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(string id)
    {
        await _auditer.AuditCover(id, "DELETE");
        await _cosmosDbService.DeleteItemAsync(id);
        return Ok();
    }

    
}