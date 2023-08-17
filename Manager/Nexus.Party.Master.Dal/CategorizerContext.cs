using Microsoft.EntityFrameworkCore;

namespace Nexus.Party.Master.Dal;

public class CategorizerContext : DbContext
{
    public CategorizerContext() : base()
    {
    }

    public CategorizerContext(DbContextOptions<CategorizerContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }
}
