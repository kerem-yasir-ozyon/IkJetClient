using IkJetApp.Enums;
using IkJetApp.Helpers;

namespace IkJetApp.Areas.HRManager.Models
{
    public class ExpenseRequestViewModel
    {
        public  int Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public ExpenseType ExpenseType { get; set; }
        public double Amount { get; set; }
        public CurrencyType CurrencyType { get; set; }
        public string? ImageName { get; set; }

        public IFormFile? ImageFile { get; set; }   
        public ApprovalStatus ApprovalStatus { get; set; }

        //Enumlarda DisplayName olarak kullanmak için gerekli
        public string ExpenseTypeDisplay => ExpenseType.GetDisplayName();
        public string CurrencyTypeDisplay => CurrencyType.GetDisplayName();
        public string ApprovalStatusDisplay => ApprovalStatus.GetDisplayName();
    }
}
