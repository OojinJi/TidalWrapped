using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TidalWrapped.Data.Models;

namespace TidalWrapped.Data
{
    public partial class TidalDbContext : DbContext
    {
        public TidalDbContext() { }

        public TidalDbContext(DbContextOptions<TidalDbContext> options)
            : base(options) { }

        public virtual DbSet<Track> Tracks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer("Data Source=OOJIND\\DISCORDBOTSERVER;Initial Catalog=TidalWrapped;Integrated Security=True;TrustServerCertificate=True");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Track>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

}
