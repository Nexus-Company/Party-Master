using Microsoft.EntityFrameworkCore;
using Nexus.Party.Master.Dal.Models.Accounts;

namespace Nexus.Party.Master.Dal;

public partial class AuthenticationContext : DbContext
{
    public virtual DbSet<Account> Accounts { get; set; }

    public string ConnectionString { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlServer(ConnectionString);
    }
}