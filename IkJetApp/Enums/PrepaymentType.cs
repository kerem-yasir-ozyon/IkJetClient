using System.ComponentModel.DataAnnotations;

namespace IkJetApp.Enums
{
    public enum PrepaymentType
    {
        [Display(Name = "Seyahat")]
        Travel = 1,

        [Display(Name = "Konaklama")]
        Accommodation,

        [Display(Name = "Eğitim")]
        Training,

        [Display(Name = "Diğer")]
        Other

    }
}
