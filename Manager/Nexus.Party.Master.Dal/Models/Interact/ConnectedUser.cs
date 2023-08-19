using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace Nexus.Party.Master.Dal.Models.Interact;

[Index(nameof(AccountId), IsUnique = true)]
public class ConnectedUser
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid AccountId { get; set; }

    public DateTime ConnectedAt { get; set; }
}