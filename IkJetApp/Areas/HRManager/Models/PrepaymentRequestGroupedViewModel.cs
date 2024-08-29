using IkJetApp.Areas.Personnel.Models.Prepayment;

namespace IkJetApp.Areas.HRManager.Models
{
    public class PrepaymentRequestGroupedViewModel
    {
        public string FullName { get; set; }
        public List<PrepaymentRequestViewModel> Requests { get; set; }
    }
}
