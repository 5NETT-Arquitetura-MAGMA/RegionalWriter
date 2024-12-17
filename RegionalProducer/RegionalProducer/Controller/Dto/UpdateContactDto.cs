using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RegionalProducer.Controller.Dto
{
    public class UpdateContactDto
    {
        [Required]
        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonProperty("nome")]
        [JsonPropertyName("nome")]
        public string? Nome { get; set; }

        [Range(100000000, 999999999, ErrorMessage = "O telefone deve ter 9 digitos.")]
        [JsonProperty("telefone")]
        [JsonPropertyName("telefone")]
        public int? Telefone { get; set; }

        [Range(01, 99, ErrorMessage = "O DDD deve ter 2 digitos.")]
        [JsonProperty("ddd")]
        [JsonPropertyName("ddd")]
        public int? DDD { get; set; }

        [EmailAddress(ErrorMessage = "O e-mail informado não é válido.")]
        [JsonProperty("email")]
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [StringLength(2, MinimumLength = 2, ErrorMessage = "O Estado deve conter exatos 2 caracteres. Exemplo: SP")]
        [JsonProperty("estado")]
        [JsonPropertyName("estado")]
        public string? Estado { get; set; }

        [JsonProperty("cidade")]
        [JsonPropertyName("cidade")]
        public string? Cidade { get; set; }
    }
}