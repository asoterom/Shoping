using System.ComponentModel.DataAnnotations;

namespace Shoping.Data.Entities
{
    public class City
    {
        public int Id { get; set; }

        [Display(Name = "Ciudad")]
        [MaxLength(50, ErrorMessage = "El Campo {0} debe tener maximo {1} caracteres")]
        [Required(ErrorMessage = "El Campo {0} es Obligatorio")]
        public string Name { get; set; }
        public State State { get; set; }

    }
}
