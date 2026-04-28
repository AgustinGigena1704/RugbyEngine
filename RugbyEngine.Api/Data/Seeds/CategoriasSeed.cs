using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RugbyEngine.Api.Data.Entities;
using RugbyEngine.Api.Data.Attributes;

namespace RugbyEngine.Api.Data.Seeds
{
    [Seed]
    public class CategoriasSeed : IEntityTypeConfiguration<Categoria>
    {
        private readonly DateTime seedDate = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public void Configure(EntityTypeBuilder<Categoria> builder)
        {
            builder.HasData(
                new Categoria { Id = 1, Nombre = "Superior", Abreviatura = "SUP", EdadMinima = 20, EdadMaxima = 0, CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Categoria { Id = 2, Nombre = "M19", Abreviatura = "M19", EdadMinima = 18, EdadMaxima = 19, CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Categoria { Id = 3, Nombre = "M17", Abreviatura = "M17", EdadMinima = 17, EdadMaxima = 17, CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Categoria { Id = 4, Nombre = "M16", Abreviatura = "M16", EdadMinima = 16, EdadMaxima = 16, CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Categoria { Id = 5, Nombre = "M15", Abreviatura = "M15", EdadMinima = 15, EdadMaxima = 15, CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Categoria { Id = 6, Nombre = "M14", Abreviatura = "M14", EdadMinima = 14, EdadMaxima = 14, CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Categoria { Id = 7, Nombre = "M13", Abreviatura = "M13", EdadMinima = 13, EdadMaxima = 13, CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Categoria { Id = 8, Nombre = "M10", Abreviatura = "M10", EdadMinima = 10, EdadMaxima = 10, CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Categoria { Id = 9, Nombre = "M9", Abreviatura = "M9", EdadMinima = 9, EdadMaxima = 9, CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Categoria { Id = 10, Nombre = "M8", Abreviatura = "M8", EdadMinima = 8, EdadMaxima = 8, CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Categoria { Id = 11, Nombre = "M7", Abreviatura = "M7", EdadMinima = 7, EdadMaxima = 7, CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Categoria { Id = 12, Nombre = "Escuelita ", Abreviatura = "ESC", EdadMinima = 0, EdadMaxima = 6, CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! }
            );
        }
    }
}
