using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RugbyEngine.Api.Data.Entities;

namespace RugbyEngine.Api.Data.Seeds
{
    public class PersonasSeed : IEntityTypeConfiguration<Persona>
    {
        private readonly DateTime seedDate = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public void Configure(EntityTypeBuilder<Persona> builder)
        {
            builder.HasData(
                new Persona
                {
                    Id = 1,
                    Nombres = "Agustin",
                    Apellidos = "Gigena",
                    Documento = "44590912",
                    Email = "agustingigena1704@gmail.com",
                    FechaNacimiento = new DateOnly(2000, 1, 1),
                    CreatedAt = seedDate,
                    CreatedById = 1,
                    CreatedBy = null!,
                }
            );
        }
    }
}
