using System.ComponentModel.DataAnnotations;

namespace Nexus.Party.Master.Dal.Models.Accounts;

public class Authorization
{
    [Key]
    public long Id { get; set; }

    [Required]
    public string Token { get; set; }

    [Required]
    public DateTime Generated { get; set; }
}