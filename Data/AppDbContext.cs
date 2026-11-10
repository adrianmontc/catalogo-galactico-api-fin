using System.Text.Json;
using ApiCatalogo_Galactico.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ApiCatalogo_Galactico.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Personaje> Personajes => Set<Personaje>();
        public DbSet<CardPersonaje> Cards => Set<CardPersonaje>();
        public DbSet<Evento> Eventos => Set<Evento>();
        public DbSet<Nave> Naves => Set<Nave>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var intListConverter = new ValueConverter<List<int>, string>(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => string.IsNullOrWhiteSpace(v)
                    ? new List<int>()
                    : JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions?)null) ?? new List<int>());

            var intListComparer = new ValueComparer<List<int>>(
                (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList());

            modelBuilder.Entity<Personaje>(e =>
            {
                e.Property(p => p.Nombre).IsRequired().HasMaxLength(100);
                e.Property(p => p.Especie).IsRequired().HasMaxLength(80);
                e.Property(p => p.Faccion).IsRequired().HasMaxLength(40);
                e.Property(p => p.Afiliacion).IsRequired().HasMaxLength(100);
                e.Property(p => p.Estado).IsRequired().HasMaxLength(30);
            });

            modelBuilder.Entity<CardPersonaje>(e =>
            {
                e.Property(c => c.HabilidadEspecial).IsRequired().HasMaxLength(150);
                e.Property(c => c.Arma).IsRequired().HasMaxLength(100);
                e.Property(c => c.ImagenUrl).IsRequired().HasMaxLength(255);
                e.HasIndex(c => c.PersonajeId).IsUnique();
                e.HasOne(c => c.Personaje)
                    .WithOne(p => p.Card)
                    .HasForeignKey<CardPersonaje>(c => c.PersonajeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Evento>(e =>
            {
                e.Property(ev => ev.Nombre).IsRequired().HasMaxLength(120);
                e.Property(ev => ev.Ubicacion).IsRequired().HasMaxLength(120);
                e.Property(ev => ev.Descripcion).IsRequired().HasMaxLength(500);
                e.Property(ev => ev.Resultado).HasMaxLength(200);
                e.Property(ev => ev.Ganador).HasMaxLength(100);

                e.Property(ev => ev.Participantes)
                    .HasConversion(intListConverter)
                    .Metadata.SetValueComparer(intListComparer);

                e.Property(ev => ev.Muertos)
                    .HasConversion(intListConverter)
                    .Metadata.SetValueComparer(intListComparer);
            });

            modelBuilder.Entity<Nave>(e =>
            {
                e.Property(n => n.Nombre).IsRequired().HasMaxLength(100);
                e.Property(n => n.Modelo).IsRequired().HasMaxLength(80);
                e.Property(n => n.CodigoIgnicionHash).IsRequired().HasMaxLength(255);
                e.Property(n => n.CoordenadasHangar).IsRequired().HasMaxLength(255);
                e.HasOne(n => n.Personaje)
                    .WithMany(p => p.Naves)
                    .HasForeignKey(n => n.PersonajeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}