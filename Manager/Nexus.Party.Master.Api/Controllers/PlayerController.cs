using Newtonsoft.Json.Serialization;
using Nexus.Party.Master.Api.Models;
using Nexus.Party.Master.Dal.Models.Interact;
using Nexus.Spotify.Client.Models;
using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace Nexus.Party.Master.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/Player")]
public class PlayerController : UseSyncController
{
    Random random;
    public PlayerController(IConfiguration config, IServiceProvider serviceProvider)
        : base(config, serviceProvider)
    {
        random = new();
    }

    [HttpGet("Queue")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Track[]), (int)HttpStatusCode.OK)]
    public IActionResult Queue(string? key = null, int per_page = 25, int page = 1)
    {
        IEnumerable<Track> result = Array.Empty<Track>();

        if (per_page > 100 ||
            per_page < 1 ||
            page < 1)
            return BadRequest();

        if (!string.IsNullOrEmpty(key))
            result = (from msc in syncService.Queue
                      where msc.Name.Contains(key)
                      select msc);
        else
            result = syncService.Queue;


        return Ok(result.Skip((page - 1) * per_page)
            .Take(per_page));
    }

    [HttpGet("Actual")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.ServiceUnavailable)]
    [ProducesResponseType(typeof(TrackActual), (int)HttpStatusCode.OK)]
    public IActionResult ActualAsync()
    {
        if (syncService.Player is null)
            return StatusCode(HttpStatusCode.ServiceUnavailable);

        return Ok(new TrackActual(syncService.Player));
    }

    [HttpGet("Connect")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ConnectAsync()
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
            return BadRequest();

        ConnectedUser? con = null;

        if (User != null)
        {
            con = new ConnectedUser()
            {
                AccountId = User.Id,
                ConnectedAt = DateTime.UtcNow
            };

            await interContext.Connecteds.AddAsync(con);

            await interContext.SaveChangesAsync();
        }

        await HandleConnectionAsync(await HttpContext.WebSockets.AcceptWebSocketAsync());

        if (con != null)
        {
            interContext.Remove(con);

            await interContext.SaveChangesAsync();
        }

        return new EmptyResult();
    }

    private async Task HandleConnectionAsync(WebSocket socket)
    {
        // Send Music Change message to client
        void MusicChange(object sender, EventArgs args)
        {
            var json = JsonConvert.SerializeObject(sender, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(json)), WebSocketMessageType.Text, true,
                    CancellationToken.None)
                .Wait();
        }

        syncService.MusicChange += MusicChange;

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

        syncService.MusicChange -= MusicChange;
    }
}