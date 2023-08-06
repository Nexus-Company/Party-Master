using Newtonsoft.Json.Serialization;
using Nexus.Party.Master.Domain.Models.Spotify;
using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace Nexus.Party.Master.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/Player")]
public class PlayerController : UseSyncController
{
    public PlayerController(IConfiguration config, IServiceProvider serviceProvider)
        : base(config, serviceProvider)
    {
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
            result = (from msc in SyncService.Queue
                      where msc.Name.Contains(key)
                      select msc);
        else
            result = SyncService.Queue;


        return Ok(result.Skip((page - 1) * per_page)
            .Take(per_page));
    }

    [HttpGet("Actual")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.ServiceUnavailable)]
    [ProducesResponseType(typeof(Track), (int)HttpStatusCode.OK)]
    public IActionResult ActualAsync()
    {
        if (SyncService.Track is not null)
            return Ok(SyncService.Track);

        if (SyncService.Online)
            return StatusCode(HttpStatusCode.ServiceUnavailable);

        return NoContent();
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

        SyncService.MusicChange += MusicChange;

        WebSocketReceiveResult? result = null;

        // Await close connection for client
        while (!(result?.CloseStatus.HasValue ?? socket.CloseStatus.HasValue))
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

        SyncService.MusicChange -= MusicChange;
    }
}