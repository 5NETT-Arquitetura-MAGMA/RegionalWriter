using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityData.Core.Models.Entity
{
    [Table("Cidades")]
    public class Cidade : EntityBase
    {
        [Required(ErrorMessage = "O Nome da cidade é obrigatório.")]
        public string NomeCidade { get; set; }

        [Required(ErrorMessage = "O estado é obrigatório.")]
        public string Estado { get; set; }

        [Required(ErrorMessage = "O DDD é obrigatório.")]
        public string DDD { get; set; }
    }
}