using System.ComponentModel.DataAnnotations;

namespace Shoping.Models
{
    public class CityViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Departamento/Estado")]
        [MaxLength(50, ErrorMessage = "El Campo {0} debe tener maximo {1} caracteres")]
        [Required(ErrorMessage = "El Campo {0} es Obligatorio")]
        public string Name { get; set; }

        public int StateId { get; set; }
    }
}
