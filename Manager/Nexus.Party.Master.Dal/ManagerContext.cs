using Microsoft.EntityFrameworkCore;

namespace Nexus.Party.Master.Dal;

public class ManagerContext : DbContext
{
    public bool UseSQLite { get; set; } = true;
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (UseSQLite)
            optionsBuilder.UseSqlite("Data Source=Party_Manager.db");
    }
}
