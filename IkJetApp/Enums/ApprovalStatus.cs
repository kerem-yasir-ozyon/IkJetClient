using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IkJetApp.Enums
{
    public enum ApprovalStatus
    {
        [Display(Name ="Onay bekliyor")]
        Pending = 1,
        [Display(Name = "Onaylandı")]
        Approved,
        [Display(Name = "Reddedildi")]
        Rejected,
        [Display(Name = "İptal Edildi")]
        Canceled

    }
}
