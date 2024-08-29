using IkJetApp.Enums;
using IkJetApp.Helpers;

namespace IkJetApp.Areas.Personnel.Models
{
    public abstract class BaseViewModel
    {
        public int Id { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }
        public string ApprovalStatusDisplay => ApprovalStatus.GetDisplayName();
    }
}
