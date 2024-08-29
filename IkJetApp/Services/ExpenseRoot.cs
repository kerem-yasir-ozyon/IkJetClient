using IkJetApp.Areas.Personnel.Models.Expense;
using IkJetApp.Areas.Personnel.Models.Prepayment;
using System.Text.Json.Serialization;

namespace IkJetApp.Services
{
    public class ExpenseRoot
    {
        [JsonPropertyName("$id")]
        public string Id { get; set; }
        [JsonPropertyName("$values")]
        public List<ExpenseViewModel> Values { get; set; }
    }
}
