namespace RugbyEngine.Api.Data.Entities
{
    public class PersonaCuenta : GenericEntity
    {
        public required int PersonaId { get; set; }
        public required virtual Persona Persona { get; set; }
        public required int CuentaId { get; set; }
        public required virtual Cuenta Cuenta { get; set; }
    }
}
