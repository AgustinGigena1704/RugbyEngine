using System.Reflection;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RugbyEngine.Api.Data.Entities;
using RugbyEngine.Api.Data.Seeds;
using RugbyEngine.Api.Data.Attributes;
using RugbyEngine.Api.Services;
using RugbyEngine.Api.Services.Interfaces;

namespace RugbyEngine.Api.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }

        public DbSet<Persona> Personas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Perfil> Perfiles { get; set; }
        public DbSet<Permiso> Permisos { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Posicion> Posiciones { get; set; }
        public DbSet<Entrenamiento> Entrenamientos { get; set; }
        public DbSet<Jugador> Jugadores { get; set; }
        public DbSet<Asistencia> Asistencias { get; set; }

        public DbSet<Cuenta> Cuentas { get; set; }
        public DbSet<PersonaCuenta> PersonaCuentas { get; set; }
        public DbSet<TipoMovimiento> TiposMovimiento { get; set; }
        public DbSet<Movimiento> Movimientos { get; set; }
        public DbSet<MovimientoItem> MovimientoItems { get; set; }
        public DbSet<TipoEvento> TiposEvento { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<EventoMovimientos> EventoMovimientos { get; set; }
        public DbSet<EventoCuenta> EventoCuentas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureAuditoryEntities(modelBuilder);

            modelBuilder.Entity<Persona>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombres).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Apellidos).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Documento).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.BorradoLogico).HasDefaultValue(false);
                entity.HasIndex(e => e.Documento).IsUnique();
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(30);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.BorradoLogico).HasDefaultValue(false);
                entity.HasIndex(e => e.Username).IsUnique();
            });

            modelBuilder.Entity<Perfil>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Descripcion).HasMaxLength(200);
                entity.Property(e => e.BorradoLogico).HasDefaultValue(false);

                entity.HasMany(p => p.Usuarios)
                    .WithMany(u => u.Perfiles)
                    .UsingEntity<Dictionary<string, object>>(
                        "UsuarioPerfil",
                        j => j.HasOne<Usuario>().WithMany().HasForeignKey("UsuarioId").OnDelete(DeleteBehavior.Cascade),
                        j => j.HasOne<Perfil>().WithMany().HasForeignKey("PerfilId").OnDelete(DeleteBehavior.Cascade),
                        j =>
                        {
                            j.HasKey("UsuarioId", "PerfilId");
                            j.ToTable("UsuarioPerfil");
                        });
            });

            modelBuilder.Entity<Permiso>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Descripcion).HasMaxLength(200);
                entity.Property(e => e.BorradoLogico).HasDefaultValue(false);
                entity.HasIndex(e => e.Codigo).IsUnique();

                entity.HasMany(p => p.Perfiles)
                    .WithMany(u => u.Permisos)
                    .UsingEntity<Dictionary<string, object>>(
                        "UsuarioPermiso",
                        j => j.HasOne<Perfil>().WithMany().HasForeignKey("PerfilId").OnDelete(DeleteBehavior.Cascade),
                        j => j.HasOne<Permiso>().WithMany().HasForeignKey("PermisoId").OnDelete(DeleteBehavior.Cascade),
                        j =>
                        {
                            j.HasKey("PerfilId", "PermisoId");
                            j.ToTable("PerfilPermiso");
                        });
            });

            modelBuilder.Entity<Menu>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Titulo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.BorradoLogico).HasDefaultValue(false);
            });

            // ── Categoria ─────────────────────────────────────────────────────────
            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Abreviatura).IsRequired().HasMaxLength(20);
                entity.Property(e => e.BorradoLogico).HasDefaultValue(false);
            });

            // ── Posicion ───────────────────────────────────────────────────────────
            modelBuilder.Entity<Posicion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Numero).IsRequired();
                entity.Property(e => e.BorradoLogico).HasDefaultValue(false);
            });

            // ── Entrenamiento ──────────────────────────────────────────────────────
            modelBuilder.Entity<Entrenamiento>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BorradoLogico).HasDefaultValue(false);

                entity.HasOne(e => e.Categoria)
                    .WithMany()
                    .HasForeignKey(e => e.CategoriaId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
            });

            // ── Jugador ────────────────────────────────────────────────────────────
            modelBuilder.Entity<Jugador>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BorradoLogico).HasDefaultValue(false);

                entity.HasOne(e => e.Persona)
                    .WithMany()
                    .HasForeignKey(e => e.PersonaId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                entity.HasOne(e => e.Categoria)
                    .WithMany()
                    .HasForeignKey(e => e.CategoriaId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                entity.HasOne(e => e.PosicionPrincipal)
                    .WithMany()
                    .HasForeignKey(e => e.PosicionPrincipalId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                entity.HasOne(e => e.PosicionSecundaria)
                    .WithMany()
                    .HasForeignKey(e => e.PosicionSecundariaId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);

                entity.HasOne(e => e.PosicionTerciaria)
                    .WithMany()
                    .HasForeignKey(e => e.PosicionTerciariaId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);

                entity.HasIndex(e => new { e.PersonaId, e.CategoriaId }).IsUnique();
            });

            // ── Asistencia ─────────────────────────────────────────────────────────
            modelBuilder.Entity<Asistencia>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BorradoLogico).HasDefaultValue(false);
                entity.Property(e => e.Estado).HasDefaultValue(RugbyEngine.Shared.Entrenamientos.AsistenciaEstado.Ausente);

                entity.HasOne(e => e.Entrenamiento)
                    .WithMany()
                    .HasForeignKey(e => e.EntrenamientoId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                entity.HasOne(e => e.Jugador)
                    .WithMany()
                    .HasForeignKey(e => e.JugadorId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                entity.HasIndex(e => new { e.EntrenamientoId, e.JugadorId }).IsUnique();
            });

            // ── Cuenta ────────────────────────────────────────────────────────────
            modelBuilder.Entity<Cuenta>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.DType).IsRequired();
                entity.Property(e => e.BorradoLogico).HasDefaultValue(false);
            });

            // ── PersonaCuenta (join Persona ↔ Cuenta, M:N) ───────────────────────
            modelBuilder.Entity<PersonaCuenta>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BorradoLogico).HasDefaultValue(false);

                entity.HasOne(pc => pc.Persona)
                    .WithMany(p => p.PersonaCuentas)
                    .HasForeignKey(pc => pc.PersonaId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                entity.HasOne(pc => pc.Cuenta)
                    .WithMany(c => c.PersonaCuentas)
                    .HasForeignKey(pc => pc.CuentaId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                entity.HasIndex(pc => new { pc.PersonaId, pc.CuentaId }).IsUnique();
            });

            // ── TipoMovimiento ────────────────────────────────────────────────────
            modelBuilder.Entity<TipoMovimiento>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.BorradoLogico).HasDefaultValue(false);
            });

            // ── Movimiento ────────────────────────────────────────────────────────
            modelBuilder.Entity<Movimiento>(entity =>
            {
                entity.HasOne(m => m.Envia)
                    .WithMany()
                    .HasForeignKey(m => m.EnviaId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                entity.HasOne(m => m.Recibe)
                    .WithMany()
                    .HasForeignKey(m => m.RecibeId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                entity.HasOne(m => m.TipoMovimiento)
                    .WithMany()
                    .HasForeignKey(m => m.TipoMovimientoId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                entity.HasIndex(m => new { m.EnviaId, m.RecibeId, m.TipoMovimientoId, m.Fecha })
                    .IsUnique()
                    .HasDatabaseName("IX_Movimiento_Unicidad");
            });

            // ── MovimientoItem ────────────────────────────────────────────────────
            modelBuilder.Entity<MovimientoItem>(entity =>
            {
                entity.Property(e => e.Producto).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Monto).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Descripcion).HasMaxLength(500);

                entity.HasOne(mi => mi.Movimiento)
                    .WithMany(m => m.Items)
                    .HasForeignKey(mi => mi.MovimientoId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            // ── TipoEvento ────────────────────────────────────────────────────────
            modelBuilder.Entity<TipoEvento>(entity =>
            {
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Descripcion).HasMaxLength(300);
                entity.HasIndex(e => e.Codigo).IsUnique();
            });

            // ── Evento ────────────────────────────────────────────────────────────
            modelBuilder.Entity<Evento>(entity =>
            {
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);

                entity.HasOne(e => e.TipoEvento)
                    .WithMany()
                    .HasForeignKey(e => e.TipoEventoId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
            });

            // ── EventoMovimientos (join Evento ↔ Movimiento) ──────────────────────
            modelBuilder.Entity<EventoMovimientos>(entity =>
            {
                entity.HasOne(em => em.Evento)
                    .WithMany()
                    .HasForeignKey(em => em.EventoId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                entity.HasOne(em => em.Movimiento)
                    .WithMany()
                    .HasForeignKey(em => em.MovimientoId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                entity.HasIndex(em => new { em.EventoId, em.MovimientoId }).IsUnique();
            });

            // ── EventoCuenta (join Evento ↔ Cuenta) ───────────────────────────────
            modelBuilder.Entity<EventoCuenta>(entity =>
            {
                entity.Property(e => e.Concepto).HasMaxLength(200);

                entity.HasOne(ec => ec.Evento)
                    .WithMany()
                    .HasForeignKey(ec => ec.EventoId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                entity.HasOne(ec => ec.Cuenta)
                    .WithMany()
                    .HasForeignKey(ec => ec.CuentaId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                entity.HasIndex(ec => new { ec.EventoId, ec.CuentaId }).IsUnique();
            });




            SeedAsync(modelBuilder).Wait();

        }

        /// <summary>
        /// Configura automáticamente las relaciones de auditoría para todas las entidades que implementan IAudithory
        /// </summary>
        private static void ConfigureAuditoryEntities(ModelBuilder modelBuilder)
        {
            var auditoryEntityTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass
                         && !t.IsAbstract
                         && typeof(IAudithory).IsAssignableFrom(t)
                         && typeof(GenericEntity).IsAssignableFrom(t))
                .ToList();

            foreach (var entityType in auditoryEntityTypes)
            {
                var entity = modelBuilder.Model.FindEntityType(entityType);

                if (entity == null)
                {
                    modelBuilder.Entity(entityType);
                    entity = modelBuilder.Model.FindEntityType(entityType);
                }

                if (entity != null)
                {
                    modelBuilder.Entity(entityType)
                        .HasOne(typeof(Usuario), "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    modelBuilder.Entity(entityType)
                        .HasOne(typeof(Usuario), "UpdatedBy")
                        .WithMany()
                        .HasForeignKey("UpdatedById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired(false);
                    modelBuilder.Entity(entityType)
                        .HasOne(typeof(Usuario), "DeletedBy")
                        .WithMany()
                        .HasForeignKey("DeletedById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired(false);

                    modelBuilder.Entity(entityType)
                        .Property("BorradoLogico")
                        .HasDefaultValue(false);

                    modelBuilder.Entity(entityType)
                        .Property("CreatedAt")
                        .IsRequired();

                    modelBuilder.Entity(entityType)
                        .Property("UpdatedAt")
                        .IsRequired(false);

                    modelBuilder.Entity(entityType)
                        .Property("DeletedAt")
                        .IsRequired(false);
                }
            }
        }

        public static async Task SeedAsync(ModelBuilder modelBuilder)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var applyConfigMethod = typeof(ModelBuilder)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(m => m.Name == "ApplyConfiguration" && m.IsGenericMethodDefinition);

            if (applyConfigMethod == null)
            {
                await Task.CompletedTask;
                return;
            }

            var seedTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttributes(typeof(SeedAttribute), inherit: false).Any())
                .ToList();

            foreach (var seedType in seedTypes)
            {
                var iface = seedType.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>));

                if (iface == null) continue;

                var entityType = iface.GetGenericArguments()[0];
                var instance = Activator.CreateInstance(seedType);
                if (instance == null) continue;

                var generic = applyConfigMethod.MakeGenericMethod(entityType);
                generic.Invoke(modelBuilder, new[] { instance });
            }

            await Task.CompletedTask;
        }
    }
}
