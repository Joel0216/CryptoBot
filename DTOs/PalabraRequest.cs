using System.ComponentModel.DataAnnotations;

namespace CryptoBot.DTOs
{
    public class PalabraRequest
    {
        [Required]
        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "El texto solo puede contener letras y espacios.")]
        public string Texto { get; set; } = string.Empty;

        [Required]
        [Range(0, 25, ErrorMessage = "El desplazamiento debe ser un número entero positivo entre 0 y 25.")]
        public int Desplazamiento { get; set; }
    }
}
