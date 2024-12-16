using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace RegionalWriter.Model.View
{
    public class RegionalDeleteDto
    {
        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public int Id { get; set; }
    }
}