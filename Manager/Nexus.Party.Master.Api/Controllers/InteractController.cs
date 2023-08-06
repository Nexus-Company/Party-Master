using Newtonsoft.Json.Serialization;
using Nexus.Party.Master.Api.Models;
using Nexus.Party.Master.Dal.Models.Interact;
using Nexus.Party.Master.Domain.Middleware;
using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace Nexus.Party.Master.Api.Controllers;

[ApiController]
[Interact, RequireAuthentication]
public class InteractController : UseSyncController
{
    public delegate void Interact(object interact, EventArgs args);
    public static event Interact NewInteract;

    public InteractController(IConfiguration config, IServiceProvider serviceProvider)
        : base(config, serviceProvider)
    {
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    [Route("Vote/Skip")]
    public async Task<IActionResult> VoteSkipAsync()
    {
        await AddInteractionAsync(InteractionType.VoteSkip, syncService.Track!.Id);



        return Ok();
    }


    [HttpGet("Connect")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ConnectAsync()
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
            return BadRequest();

        await HandleConnectionAsync(await HttpContext.WebSockets.AcceptWebSocketAsync());

        return new EmptyResult();
    }

    private async Task HandleConnectionAsync(WebSocket socket)
    {
        // Send Music Change message to client
        void NewInteraction(object sender, EventArgs args)
        {
            var json = JsonConvert.SerializeObject(sender, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(json)), WebSocketMessageType.Text, true,
                    CancellationToken.None)
                .Wait();
        }

        NewInteract += NewInteraction;

        WebSocketReceiveResult? result = null;

        // Await close connection for client
        while (socket.State == WebSocketState.Open)
        {
            try
            {
                result = await socket.ReceiveAsync(new ArraySegment<byte>(Array.Empty<byte>()), CancellationToken.None);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        NewInteract -= NewInteraction;
    }

    private async Task AddInteractionAsync(InteractionType type, string? trackId = null)
    {
        var interaction = new Interaction()
        {
            Actor = User!.Id,
            InteractionType = type,
            Date = DateTime.UtcNow,
            TrackId = trackId
        };

        await interContext.Interactions.AddAsync(interaction);

        await interContext.SaveChangesAsync();

        NewInteract?.BeginInvoke(new InteractionResult(interaction), new(), null, null);
    }
}