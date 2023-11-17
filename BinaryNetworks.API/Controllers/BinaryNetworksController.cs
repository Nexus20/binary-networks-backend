using BinaryNetworks.Application.Interfaces.Services.BinaryNetworks;
using BinaryNetworks.Application.Models.Requests.BinaryNetworks;
using BinaryNetworks.Application.Models.Results.BinaryNetworks;
using Microsoft.AspNetCore.Mvc;

namespace BinaryNetworks.API.Controllers;

[ApiController]
[Route("[controller]")]
public class BinaryNetworksController : ControllerBase
{
    private readonly IBinaryNetworksService _binaryNetworksService;

    public BinaryNetworksController(IBinaryNetworksService binaryNetworksService)
    {
        _binaryNetworksService = binaryNetworksService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BinaryNetworkResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var result = await _binaryNetworksService.GetAsync(cancellationToken);
        
        return Ok(result);
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BinaryNetworkResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var result = await _binaryNetworksService.GetByIdAsync(id, cancellationToken);
        
        return Ok(result);
    }

    [HttpPost("save")]
    [Consumes("application/json")]
    public async Task<IActionResult> SaveAsync([FromBody]SaveBinaryNetworkRequest request)
    {
        await _binaryNetworksService.SaveAsync(request);
        
        return NoContent();
    }
}