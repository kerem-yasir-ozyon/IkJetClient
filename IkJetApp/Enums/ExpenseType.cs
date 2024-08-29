using System.ComponentModel.DataAnnotations;

namespace IkJetApp.Enums
{
    public enum ExpenseType
    {
        [Display(Name = "Seyahat")]
        Travel = 1,

        [Display(Name = "Konaklama")]
        Accommodation,

        [Display(Name = "Yemek Masrafları")]
        MealExpenses,

        [Display(Name = "Ofis Malzemeleri")]
        OfficeSupplies,

        [Display(Name = "Telefon ve İnternet")]
        PhoneaAndInternet,

        [Display(Name = "Eğitim")]
        Training,

        [Display(Name = "Araç Kiralama")]
        VehicleRental,

        [Display(Name = "Diğer")]
        Other

    }
}
