using Nexus.Party.Master.Dal.Models.Accounts;

namespace Nexus.Party.Master.Api.Models;

public class AccountResult
{
    public Guid Id { get; set; }
    public string? GoogleId { get; set; }
    public string Name { get; set; }
    public string PictureUrl { get; set; }
    public AccountResult(Account account)
    {
        Id = account.Id;
        GoogleId = account.GoogleId;
        Name = account.Name;
        PictureUrl = account.PictureUrl;
    }
}