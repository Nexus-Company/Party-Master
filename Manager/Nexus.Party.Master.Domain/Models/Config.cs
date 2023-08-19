namespace Nexus.Party.Master.Domain.Models;

public class Config
{
    public byte MaxFillingRepeat { get; set; }
    public int MinInteract { get; set; }
    public double PercentageInteract { get; set; }
    public bool AllowExplict { get; set; }
    public string WebUrl { get; set; }
    public byte MaxAddRate { get; set; }
}