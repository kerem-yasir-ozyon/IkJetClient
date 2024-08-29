using IkJetApp.Areas.Personnel.Models.Prepayment;
using IkJetApp.Areas.Personnel.Models.WorkOff;
using System.Text.Json.Serialization;

namespace IkJetApp.Services
{
    public class PrepaymentRoot
    {

        [JsonPropertyName("$id")]
        public string Id { get; set; }
        [JsonPropertyName("$values")]
        public List<PrepaymentViewModel> Values { get; set; }
    }
}
