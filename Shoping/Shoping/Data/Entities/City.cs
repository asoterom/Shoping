using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Shoping.Data.Entities
{
    public class City
    {
        public int Id { get; set; }

        [Display(Name = "Ciudad")]
        [MaxLength(50, ErrorMessage = "El Campo {0} debe tener maximo {1} caracteres")]
        [Required(ErrorMessage = "El Campo {0} es Obligatorio")]
        public string Name { get; set; }
        //etiqueta usada para ignorar en la deserializacion del json la recurrencia de estado ciudad estado
        [JsonIgnore]
        public State State { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
