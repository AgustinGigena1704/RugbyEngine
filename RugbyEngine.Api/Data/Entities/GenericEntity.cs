using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace RugbyEngine.Api.Data.Entities
{
    public abstract class GenericEntity
    {
        [Key]
        [Required]
        public int Id { get; set; }
        public bool BorradoLogico { get; set; }
    }
}
