namespace Nexus.Party.Master.Api.Controllers;


[Route("api/Search")]
public class SearchController : UseSyncController
{
    public SearchController(IConfiguration config, IServiceProvider serviceProvider)
        : base(config, serviceProvider)
    {
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> ByQuery(string q, int page = 1, int per_page = 50)
    {
        var response = await syncService.SpotifyClient!.SearchAsync(q);

        return Ok(response);
    }
}