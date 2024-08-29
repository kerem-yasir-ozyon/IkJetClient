namespace IkJetApp.Areas.Admin.Models
{
    public abstract class BaseViewModel
    {
        public int? Id { get; set; }
        public bool IsDeleted { get; set; }
    }
}
