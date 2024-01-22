using System.ComponentModel.DataAnnotations;

namespace Nexus.Party.Master.Dal.Models.Accounts;

public class Account
{
    [Key]
    public Guid Id { get; set; }

    public string? NexusId { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string PictureUrl { get; set; }

    [Required]
    public string ShortName { get; set; }
}