using System.ComponentModel.DataAnnotations.Schema;

namespace RugbyEngine.Api.Data.Entities
{
    [Table("Cuenta")]
    public class Cuenta : GenericEntity
    {
        public required string Nombre { get; set; }
        public required int DType { get; set; }
        public virtual List<PersonaCuenta>? PersonaCuentas { get; set; } = null;
    }
}
