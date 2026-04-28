using Microsoft.EntityFrameworkCore;
using RugbyEngine.Api.Data.Entities;
using RugbyEngine.Shared.Entrenamientos;

namespace RugbyEngine.Api.Data.Repositories
{
    public class AsistenciaRepository : GenericRepository<Asistencia>
    {
        public AsistenciaRepository(ApiDbContext context, ILogger<AsistenciaRepository> logger) : base(context, logger) { }

        public async Task<List<Asistencia>> GetByEntrenamientoAsync(int entrenamientoId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Where(a => a.EntrenamientoId == entrenamientoId && !a.BorradoLogico)
                .ToListAsync(cancellationToken);
        }

        public async Task UpsertBulkAsync(int entrenamientoId, List<AsistenciaItemDto> items, Usuario by, CancellationToken cancellationToken = default)
        {
            var jugadorIds = items.Select(i => i.JugadorId).ToList();
            var existing = await _dbSet
                .Where(a => a.EntrenamientoId == entrenamientoId && jugadorIds.Contains(a.JugadorId))
                .ToListAsync(cancellationToken);

            foreach (var item in items)
            {
                var asistencia = existing.FirstOrDefault(a => a.JugadorId == item.JugadorId);
                if (asistencia == null)
                {
                    var nueva = new Asistencia
                    {
                        EntrenamientoId = entrenamientoId,
                        Entrenamiento = null!,
                        JugadorId = item.JugadorId,
                        Jugador = null!,
                        Estado = item.Estado,
                        CreatedBy = by,
                        CreatedById = by.Id,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _dbSet.AddAsync(nueva, cancellationToken);
                }
                else
                {
                    asistencia.Estado = item.Estado;
                    asistencia.UpdatedBy = by;
                    asistencia.UpdatedById = by.Id;
                    asistencia.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task EnsureForEntrenamientoAsync(int entrenamientoId, int categoriaId, Usuario by, CancellationToken cancellationToken = default)
        {
            var jugadores = await _context.Set<Jugador>()
                .Where(j => !j.BorradoLogico && j.CategoriaId == categoriaId)
                .ToListAsync(cancellationToken);

            var existing = await _dbSet
                .Where(a => a.EntrenamientoId == entrenamientoId && !a.BorradoLogico)
                .ToListAsync(cancellationToken);

            var jugadorIds = new HashSet<int>(jugadores.Select(j => j.Id));

            // Soft-delete asistencias que no pertenezcan a la categoría actual
            foreach (var ex in existing.Where(a => !jugadorIds.Contains(a.JugadorId)).ToList()) //NOSONAR
            {
                ex.BorradoLogico = true;
                ex.DeletedBy = by;
                ex.DeletedById = by.Id;
                ex.DeletedAt = DateTime.UtcNow;
            }

            // Recalcular existentes después del posible borrado lógico
            existing = existing.Where(a => !a.BorradoLogico).ToList();
            var existingMap = existing.ToDictionary(a => a.JugadorId);

            // Crear nuevas asistencias para jugadores que no existan aún
            var toCreate = jugadores
                .Where(j => !existingMap.ContainsKey(j.Id))
                .Select(j => new Asistencia
                {
                    EntrenamientoId = entrenamientoId,
                    Entrenamiento = null!,
                    JugadorId = j.Id,
                    Jugador = null!,
                    Estado = AsistenciaEstado.Ausente,
                    CreatedBy = by,
                    CreatedById = by.Id,
                    CreatedAt = DateTime.UtcNow
                })
                .ToList();

            if (toCreate.Any())
            {
                await _dbSet.AddRangeAsync(toCreate, cancellationToken);
            }

            // Actualizar asistencias existentes: si no están Presente o Lesionado, poner Ausente
            var toUpdate = existing
                .Where(a => jugadorIds.Contains(a.JugadorId) && a.Estado != AsistenciaEstado.Presente && a.Estado != AsistenciaEstado.Lesionado)
                .ToList();

            foreach (var asistencia in toUpdate)
            {
                asistencia.Estado = AsistenciaEstado.Ausente;
                asistencia.UpdatedBy = by;
                asistencia.UpdatedById = by.Id;
                asistencia.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
