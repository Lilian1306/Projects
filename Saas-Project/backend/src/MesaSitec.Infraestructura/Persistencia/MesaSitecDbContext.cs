using MesaSitec.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MesaSitec.Infraestructura.Persistencia;

public class MesaSitecDbContext : DbContext
{
    public MesaSitecDbContext(DbContextOptions<MesaSitecDbContext> options) : base(options)
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Solicitud> Solicitudes => Set<Solicitud>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- Tenant ---
        modelBuilder.Entity<Tenant>(builder =>
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Nombre).IsRequired().HasMaxLength(150);
        });

        // --- Usuario ---
        modelBuilder.Entity<Usuario>(builder =>
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(200);
            builder.HasIndex(u => u.Email).IsUnique(); // Único a nivel global
            builder.Property(u => u.Nombre).IsRequired().HasMaxLength(150);
            builder.Property(u => u.PasswordHash).IsRequired();
            builder.Property(u => u.Rol).HasConversion<string>();

            builder.HasOne(u => u.Tenant)
                .WithMany()
                .HasForeignKey(u => u.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- Categoria ---
        modelBuilder.Entity<Categoria>(builder =>
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
            builder.Property(c => c.SlaHoras).IsRequired();

            builder.HasOne(c => c.Tenant)
                .WithMany()
                .HasForeignKey(c => c.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- Solicitud ---
        modelBuilder.Entity<Solicitud>(builder =>
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Codigo).IsRequired().HasMaxLength(50);
            builder.HasIndex(s => new { s.TenantId, s.Codigo }).IsUnique();

            builder.Property(s => s.Titulo).IsRequired().HasMaxLength(120);
            builder.Property(s => s.Descripcion).IsRequired().HasMaxLength(4000);

            builder.Property(s => s.Prioridad).HasConversion<string>();
            builder.Property(s => s.Estado).HasConversion<string>();

            builder.HasOne(s => s.Tenant)
                .WithMany()
                .HasForeignKey(s => s.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Categoria)
                .WithMany()
                .HasForeignKey(s => s.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Solicitante)
                .WithMany()
                .HasForeignKey(s => s.SolicitanteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Agente)
                .WithMany()
                .HasForeignKey(s => s.AgenteId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // SQLite no conserva DateTimeKind: al leer TEXT devuelve Kind=Unspecified,
        // y System.Text.Json solo agrega el sufijo "Z" cuando Kind=Utc.
        // Como todas las fechas del dominio se generan en UTC, se fuerza el Kind al leer.
        var utcConverter = new ValueConverter<DateTime, DateTime>(
            v => v,
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        var utcConverterNullable = new ValueConverter<DateTime?, DateTime?>(
            v => v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(utcConverter);
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(utcConverterNullable);
                }
            }
        }
    }
}
