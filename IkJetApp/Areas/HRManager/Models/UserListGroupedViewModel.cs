namespace IkJetApp.Areas.HRManager.Models
{
    public class UserListGroupedViewModel
    {
        public string GroupName { get; set; }  // Aktif veya Pasif gibi
        public List<UserListByCompanyViewModel> Users { get; set; }
    }
}
