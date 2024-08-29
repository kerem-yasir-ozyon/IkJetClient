using IkJetApp.Enums;
using IkJetApp.Helpers;
using IkJetApp.Models;

namespace IkJetApp.Areas.Personnel.Models.Expense
{
    public class ExpenseViewModel:BaseViewModel
    {
        public ExpenseType ExpenseType { get; set; }
        public double Amount { get; set; }
        public CurrencyType CurrencyType { get; set; }
        public string? ImageName { get; set; }

        public IFormFile? ImageFile { get; set; }


        public int AppUserId { get; set; }
        public UserViewModel? AppUser { get; set; }

        //Enumlarda DisplayName olarak kullanmak için gerekli
        public string ExpenseTypeDisplay => ExpenseType.GetDisplayName();
        public string CurrencyTypeDisplay => CurrencyType.GetDisplayName();


    }
}
