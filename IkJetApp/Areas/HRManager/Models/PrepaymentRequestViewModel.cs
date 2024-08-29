using IkJetApp.Enums;
using IkJetApp.Helpers;
using IkJetApp.Models;

namespace IkJetApp.Areas.HRManager.Models
{
    public class PrepaymentRequestViewModel
    {
        public  int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public double Amount { get; set; }
        public CurrencyType CurrencyType { get; set; }
        public string Description { get; set; }
        public PrepaymentType PrepaymentType { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }



        public int AppUserId { get; set; }
        public UserViewModel? AppUser { get; set; }

        public string CurrencyTypeDisplay => CurrencyType.GetDisplayName();
        public string PrepaymentTypeDisplay => PrepaymentType.GetDisplayName();
        public string ApprovalStatusDisplay => ApprovalStatus.GetDisplayName();
    }
}
