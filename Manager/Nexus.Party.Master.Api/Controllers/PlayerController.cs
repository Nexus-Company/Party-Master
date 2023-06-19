using Newtonsoft.Json.Serialization;
using Nexus.Party.Master.Domain.Models.Spotify;
using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace Nexus.Party.Master.Api.Controllers;

[ApiController]
[Route("api/Player")]
public class PlayerController : UseSyncController
{
    public PlayerController(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
    }

    [HttpGet("Actual")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(Track), (int)HttpStatusCode.OK)]
    public IActionResult ActualAsync()
    {
        if (SyncService.LastTrack is not null)
            return Ok(SyncService.LastTrack);

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