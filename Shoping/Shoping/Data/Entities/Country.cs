﻿using System.ComponentModel.DataAnnotations;

namespace Shoping.Data.Entities
{
    public class Country
    {
        public int Id { get; set; }
        
        [Display(Name = "Pais")]
        [MaxLength(50,ErrorMessage ="El Campo {0} debe tener maximo {1} caracteres")]
        [Required(ErrorMessage ="El Campo {0} es Obligatorio")]
        public string Name { get; set; }

    }
}
