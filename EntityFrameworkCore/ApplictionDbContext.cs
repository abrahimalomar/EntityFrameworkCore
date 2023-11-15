using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFrameworkCore
{
    class ApplictionDbContext:DbContext
    {
        public DbSet<wallet> wallets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .Build();
            var connction = configuration.GetConnectionString("Connection");

            optionsBuilder.UseSqlServer(connction);
        }
    }
}
