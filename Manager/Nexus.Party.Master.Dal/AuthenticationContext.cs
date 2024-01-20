global using Microsoft.EntityFrameworkCore;
using Nexus.Party.Master.Dal.Models.Accounts;

namespace Nexus.Party.Master.Dal;

public class AuthenticationContext : DbContext
{
    public const string ConnectionName = "Authentication";
    public virtual DbSet<Account> Accounts { get; set; }
    public virtual DbSet<Authentication> Authentications { get; set; }

    public DbContextOptions Options { get; set; }

    public AuthenticationContext()
    {
    }

    public AuthenticationContext(DbContextOptions options)
        : base(options)
    {
        Options = options;
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlite();
    }
}