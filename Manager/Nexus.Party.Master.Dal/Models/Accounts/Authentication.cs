using System.ComponentModel.DataAnnotations;

namespace Nexus.Party.Master.Dal.Models.Accounts;

public class Authentication
{
    [Key]
    public long Id { get; set; }

    [Required]
    public string Token { get; set; }

    public string? Refresh { get; set; }

    [Required]
    public DateTime Generated { get; set; }

    public double MaxAge { get; set; }

    [Required]
    public Guid AccountId { get; set; }

    public Authentication()
    {

    }

    public Authentication(string token, Guid accountId)
    {
        Token = token;
        Generated = DateTime.UtcNow;
        AccountId = accountId;
    }
}