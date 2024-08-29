using IkJetApp.Enums;
using IkJetApp.Helpers;
using IkJetApp.Models;

namespace IkJetApp.Areas.HRManager.Models
{
    public class WorkOffRequestViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public WorkOfType WorkOfType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RequestedLeaveDay { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }

        public int AppUserId { get; set; }
        public UserViewModel? AppUser { get; set; }

        public string ApprovalStatusDisplay => ApprovalStatus.GetDisplayName();
        public string WorkOfTypeDisplay => WorkOfType.GetDisplayName();
    }
}

