using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Nexus.Party.Master.Api.Controllers;

[ApiController]
[RequireAuthentication]
public class InteractController : UseSyncController
{
    private protected InteractController(IConfiguration config, IServiceProvider serviceProvider) 
        : base(config, serviceProvider)
    {
    }

    [HttpPost]
    [Route("Vote/Skip")]
    public async Task VoteSkipAsync()
    {

    }
}