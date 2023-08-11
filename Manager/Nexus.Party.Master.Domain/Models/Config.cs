namespace Nexus.Party.Master.Domain.Models;

public class Config
{
    public int MaxFillingRepeat { get; set; }
    public int MinInteract { get; set; }
    public int PercentageInteract { get; set; }
    public bool AllowExplict { get; set; }
    public string WebUrl { get; set; }
}