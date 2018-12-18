using Microsoft.EntityFrameworkCore;

namespace PortariaRemotaAPI.Models.DataContext
{
    public class KiperContext : DbContext
    {
        public KiperContext(DbContextOptions<KiperContext> options) : base(options) { }
        public DbSet<Login> Logins { get; set; }
        public DbSet<Morador> Moradores { get; set; }
        public DbSet<Apartamento> Apartamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Login>().HasData(new Login { LoginId = 1, User = "Kiper", Pass = "Kiper@2018" });
        }
    }
}
