namespace Nexus.Party.Master.Dal.Models.Manager;

public class Spotify
{
    public string Token { get; set; }
    public string Type { get; set; }
    private string Refresh { get; set; }
    public double ExpiresIn { get; set; }
    public DateTime Auth { get; set; }
}