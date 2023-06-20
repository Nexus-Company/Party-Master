namespace Nexus.Party.Master.Dal.Models.Accounts;

public class Account
{
    public Guid Id { get; set; }

    public int GoogleId { get; set; }

    public string Name { get; set; }

    public string PictureUrl { get; set; }
}