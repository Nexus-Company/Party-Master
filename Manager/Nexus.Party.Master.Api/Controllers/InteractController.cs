using Microsoft.EntityFrameworkCore;
using Nexus.Party.Master.Api.Models;
using Nexus.Party.Master.Dal.Models.Interact;
using Nexus.Party.Master.Domain.Middleware;
using Nexus.Spotify.Client.Models;
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
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.NotAcceptable)]
    [Route("Vote/Add")]
    public async Task<IActionResult> AddMusicAsync(string trackId)
    {
        int mscCount = syncService.Queue.Count(msc => msc.Id == trackId);

        if (mscCount > Config.MaxFillingRepeat)
            return Conflict();

        // Add Genre trate 

        Track? track = await syncService.SpotifyClient!.GetTrackAsync(trackId);

        if (track == null)
            return NotFound();

        await syncService.SpotifyClient!.AddToQueueAsync(trackId, syncService.Player!.Device.Id);

        await AddInteractionAsync(InteractionType.Add, trackId);

        await syncService.AddTrackInQueue(track);

        return NoContent();
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    [Route("Vote/Skip")]
    public async Task<IActionResult> VoteSkipAsync()
    {
        await AddInteractionAsync(InteractionType.VoteSkip, syncService.Track!.Id);

        int skipVottings = await (from interact in interContext.Interactions
                                  where interact.Date > syncService.Started &&
                                        interact.InteractionType == InteractionType.VoteSkip
                                  select interact.Id).CountAsync();

        if (((double)skipVottings / (double)Connecteds) * 100.0 > // Skip Vottings Percentage
            Config.PercentageInteract)
            await syncService.Player!.SkipAsync();

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
        async void NewInteraction(object sender, EventArgs args)
        {
            var json = JsonConvert.SerializeObject(sender);

            await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(json)), WebSocketMessageType.Text, true,
                    CancellationToken.None);
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

        // Será necessário a criação de um método especial para o envio de interações aos clientes conectados.
        // O Método atual pode ser sobrecarregado gerando uma resposta lenta ou até mesmo timeout para o 
        // ultimo cliente que solicitou a interação.
#warning Isso deverá ser corrigido futuramente 
        NewInteract?.Invoke(new InteractionResult(interaction), new());
    }
}