using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RugbyEngine.Api.Data.Entities;

namespace RugbyEngine.Api.Data.Seeds
{
    public class UsuariosSeed : IEntityTypeConfiguration<Usuario>
    {
        private readonly DateTime seedDate = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            var agigena = "$2a$11$knIWvYSD.JF1fM.2GKgZ4eMdCFDV26H/hEVyj5A9upJwIPIVFXZI.";

            builder.HasData(
                new Usuario
                {
                    Id = 1,
                    Username = "agigena",
                    PasswordHash = agigena,
                    Email = "agustingigena1704@gmail.com",
                    PersonaId = 1,
                    Persona = null!,
                    CreatedAt = seedDate,
                    CreatedById = 1,
                    CreatedBy = null!,
                }
            );
        }
    }
}
