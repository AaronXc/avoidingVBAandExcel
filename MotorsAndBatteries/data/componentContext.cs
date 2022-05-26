using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MotorsAndBatteries.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MotorsAndBatteries.data
{
    public class ComponentContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public ComponentContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sql server with connection string from app settings
            options.UseSqlServer(Configuration.GetConnectionString("Database"));
        }

        public DbSet<Motors> motor { get; set; }
        public DbSet<Batteries> battery { get; set; }
        public DbSet<SingleBattery> singleBattery { get; set; }
        public DbSet<SeriesBatteries> seriesBattery { get; set; }
        public DbSet<ParallelBatteries> parallelBattery { get; set; }
        public DbSet<ParallelBatteriesPE> parallelBatteryPE { get; set; }
        public DbSet<SeriesBatteriesPE> SeriesBatteryPE { get; set; }

    }
}
