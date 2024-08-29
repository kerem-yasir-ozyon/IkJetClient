using IkJetApp.Enums;
using IkJetApp.Helpers;
using IkJetApp.Models;

namespace IkJetApp.Areas.Personnel.Models.WorkOff
{
    public class WorkOffViewModel : BaseViewModel
    {
        public WorkOfType WorkOfType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RequestedLeaveDay { get; set; }

        public int AppUserId { get; set; }
        public UserViewModel? AppUser { get; set; }

        public string WorkOfTypeDisplay => WorkOfType.GetDisplayName();
    }
}
