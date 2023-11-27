using System.Text;
using BinaryNetworks.Application.Interfaces.Services.BinaryNetworks;
using BinaryNetworks.Application.Models.Requests.BinaryNetworks;
using BinaryNetworks.Application.Models.Results.BinaryNetworks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        await _binaryNetworksService.DeleteAsync(id, cancellationToken);
        
        return NoContent();
    }
    
    [HttpPatch("{id}/rename")]
    [Consumes("application/json")]
    public async Task<IActionResult> RenameAsync(string id, [FromBody]RenameBinaryNetworkRequest request, CancellationToken cancellationToken)
    {
        await _binaryNetworksService.RenameAsync(id, request.NetworkName, cancellationToken);
        
        return NoContent();
    }
    
    [HttpPost("export")]
    [Consumes("application/json")]
    public IActionResult ExportAsync([FromBody]SaveBinaryNetworkRequest request)
    {
        var json = JsonConvert.SerializeObject(request, Formatting.Indented);
        var fileName = $"{request.NetworkName}.json";

        return File(Encoding.UTF8.GetBytes(json), "application/json", fileName);
    }

    [HttpPost("import")]
    public async Task<IActionResult> ImportAsync([FromForm] IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream);
        var json = await reader.ReadToEndAsync();
        
        var request = JsonConvert.DeserializeObject<SaveBinaryNetworkRequest>(json);
        request.Id = null;
        
        await _binaryNetworksService.SaveAsync(request);
        
        return NoContent();
    }
}