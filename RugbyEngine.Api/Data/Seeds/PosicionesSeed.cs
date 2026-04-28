using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RugbyEngine.Api.Data.Entities;
using RugbyEngine.Api.Data.Attributes;

namespace RugbyEngine.Api.Data.Seeds
{
    [Seed]
    public class PosicionesSeed : IEntityTypeConfiguration<Posicion>
    {
        private readonly DateTime seedDate = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public void Configure(EntityTypeBuilder<Posicion> builder)
        {
            builder.HasData(
                new Posicion { Id = 1, Nombre = "Pilar", Numero = 1, CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Posicion { Id = 2, Nombre = "Hocker", Numero = 2, CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Posicion { Id = 3, Nombre = "Pilar", Numero = 3, CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Posicion { Id = 4, Nombre = "Segunda linea", Numero = 4, CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Posicion { Id = 5, Nombre = "Segunda linea", Numero = 5, CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Posicion { Id = 6, Nombre = "Ala ciega", Numero = 6, CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Posicion { Id = 7, Nombre = "Ala abierta", Numero = 7, CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Posicion { Id = 8, Nombre = "Octavo", Numero = 8, CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Posicion { Id = 9, Nombre = "Medio scrum", Numero = 9, CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Posicion { Id = 10, Nombre = "Apertura", Numero = 10, CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Posicion { Id = 11, Nombre = "Wing izquierdo", Numero = 11, CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Posicion { Id = 12, Nombre = "Centro interno", Numero = 12, CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Posicion { Id = 13, Nombre = "Centro externo", Numero = 13, CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Posicion { Id = 14, Nombre = "Wing derecho", Numero = 14, CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Posicion { Id = 15, Nombre = "FullBack", Numero = 15, CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! }
            );
        }
    }
}
