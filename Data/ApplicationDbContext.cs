using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BiblioTek.Models;

namespace BiblioTek.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Livre> Livres { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Favori> Favoris { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Favori>()
                .HasKey(f => new { f.UtilisateurId, f.LivreId });
        }
    }
}
