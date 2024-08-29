using IkJetApp.Areas.Personnel.Models.WorkOff;
using System.Text.Json.Serialization;

namespace IkJetApp.Services
{
    public class WorkOffRoot
    {
        [JsonPropertyName("$id")]
        public string Id { get; set; }
        [JsonPropertyName("$values")]
        public List<WorkOffViewModel> Values { get; set; }
    }
}
