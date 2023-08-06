﻿using Microsoft.EntityFrameworkCore;

namespace Nexus.Party.Master.Dal;

public class ManagerContext : DbContext
{
    public bool UseSQLite { get; set; } = true;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlite("Data Source=.\\Databases\\Authentication.db");
    }
}
