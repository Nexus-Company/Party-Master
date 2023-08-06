using Nexus.Party.Master.Dal.Models.Interact;

namespace Nexus.Party.Master.Dal;

public class InteractContext : DbContext
{
    public virtual DbSet<ConnectedUser> Connecteds { get; set; }
    public virtual DbSet<Interaction> Interactions { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseInMemoryDatabase("InteractData");
    }
}