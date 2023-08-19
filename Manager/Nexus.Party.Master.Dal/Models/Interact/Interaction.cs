using System.ComponentModel.DataAnnotations;

namespace Nexus.Party.Master.Dal.Models.Interact;
public class Interaction
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public InteractionType InteractionType { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public Guid Actor { get; set; }

    public string? TrackId { get; set; }
}
