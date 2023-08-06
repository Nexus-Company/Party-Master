using Nexus.Party.Master.Dal.Models.Interact;

namespace Nexus.Party.Master.Api.Models;

public class InteractionResult
{
    public Guid Id { get; set; }
    public InteractionType InteractionType { get; set; }
    public DateTime Date { get; set; }
    public Guid Actor { get; set; }
    public string? TrackId { get; set; }

    public InteractionResult(Interaction inter)
    {
        Id = inter.Id;
        InteractionType = inter.InteractionType;
        Date = inter.Date;
        Actor = inter.Actor;
        TrackId = inter.TrackId;
    }
}