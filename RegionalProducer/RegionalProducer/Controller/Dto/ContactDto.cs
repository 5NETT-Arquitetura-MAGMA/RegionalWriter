using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace RegionalProducer.Controller.Dto
{
    public class ContactDto
    {
        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonProperty("nome")]
        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonProperty("telefone")]
        [JsonPropertyName("telefone")]
        public int Telefone { get; set; }

        [JsonProperty("ddd")]
        [JsonPropertyName("ddd")]
        public int DDD { get; set; }

        [JsonProperty("email")]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonProperty("estado")]
        [JsonPropertyName("estado")]
        public string Estado { get; set; }

        [JsonProperty("cidade")]
        [JsonPropertyName("cidade")]
        public string Cidade { get; set; }
    }
}