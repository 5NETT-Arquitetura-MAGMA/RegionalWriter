using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CityData.Controllers.Dtos
{
    public class CityDto
    {
        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "O Nome da cidade é obrigatório.")]
        [JsonProperty("nomeCidade")]
        [JsonPropertyName("nomeCidade")]
        public string NomeCidade { get; set; }

        [Required(ErrorMessage = "O estado é obrigatório.")]
        [JsonProperty("estado")]
        [JsonPropertyName("estado")]
        public string Estado { get; set; }

        [JsonProperty("ddd")]
        [JsonPropertyName("ddd")]
        [Required(ErrorMessage = "O DDD é obrigatório.")]
        public string DDD { get; set; }
    }
}