using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wiki___Footballer_Statics.Classes;

namespace Wiki___Footballer_Statics.Context
{
    public class AppDbContext:DbContext
    {
        public DbSet<Match> Matches { get; set; }
        public DbSet<MatchEvent> MatchEvents { get; set; }
        public DbSet<MatchLineUp> MatchLineUps { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=FootballStatics;Trusted_Connection=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MatchLineUp>().HasKey(x=>new { x.MatchId, x.PlayerId,x.TeamId });

        }
    }
}
