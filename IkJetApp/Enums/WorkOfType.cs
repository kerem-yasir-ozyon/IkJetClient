using System.ComponentModel.DataAnnotations;

namespace IkJetApp.Enums
{
    public enum WorkOfType
    {
        [Display(Name = "Yıllık İzin")]
        AnnualLeave = 1,

        [Display(Name = "Hastalık İzni")]
        SickLeave,

        [Display(Name = "Sağlık Raporu")]
        MedicalReport,

        [Display(Name = "Ücretli İzin")]
        PaidLeave,

        [Display(Name = "Ücretsiz İzin")]
        UnpaidLeave,

        [Display(Name = "Evlilik İzni")]
        MarriageLeave,

        [Display(Name = "Vefat İzni")]
        BereavementLeave

    }
}
