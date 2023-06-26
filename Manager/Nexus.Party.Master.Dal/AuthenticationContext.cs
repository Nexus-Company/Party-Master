using Microsoft.EntityFrameworkCore;
using Nexus.Party.Master.Dal.Models.Accounts;

namespace Nexus.Party.Master.Dal;

public class AuthenticationContext : DbContext
{
    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Authentication> Authentications { get; set; }

    private readonly string ConnectionString;

    public AuthenticationContext()
    {
        ConnectionString = "Data Source=.\\Databases\\Authentication.db";
    }

    public AuthenticationContext(string conn)
    {
        ConnectionString = conn;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlite(ConnectionString);
    }
}