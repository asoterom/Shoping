using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Shoping.Data.Entities
{
    public class State
    {
        public int Id { get; set; }

        [Display(Name = "Departamento/Estado")]
        [MaxLength(50, ErrorMessage = "El Campo {0} debe tener maximo {1} caracteres")]
        [Required(ErrorMessage = "El Campo {0} es Obligatorio")]
        public string Name { get; set; }
        
        //ya que genera un ciclo infinito de pais estado pais estado, la etiqueta JsonIgnore 
        //permite ignorar la recurrencia y se pueda deserialiar el objeto json
        [JsonIgnore]
        public Country Country { get; set; }

        [Display(Name = "Ciudades")]
        public ICollection<City> Cities { get; set; }
        public int CitiesNumber => Cities == null ? 0 : Cities.Count; 

    }
}
