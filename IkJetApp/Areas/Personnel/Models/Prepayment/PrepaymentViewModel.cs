using IkJetApp.Enums;
using IkJetApp.Helpers;
using IkJetApp.Models;

namespace IkJetApp.Areas.Personnel.Models.Prepayment
{
    public class PrepaymentViewModel:BaseViewModel
    {
        public double Amount { get; set; }
        public CurrencyType CurrencyType { get; set; }
        public string Description { get; set; }
        public PrepaymentType PrepaymentType { get; set; }


        public int AppUserId { get; set; }
        public UserViewModel? AppUser { get; set; }

        public string CurrencyTypeDisplay => CurrencyType.GetDisplayName();
        public string PrepaymentTypeDisplay => PrepaymentType.GetDisplayName();

    }
}
