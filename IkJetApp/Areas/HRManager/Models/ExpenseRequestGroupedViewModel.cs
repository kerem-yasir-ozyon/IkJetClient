namespace IkJetApp.Areas.HRManager.Models
{
    public class ExpenseRequestGroupedViewModel
    {
        public string FullName { get; set; }
        public List<ExpenseRequestViewModel> Expenses { get; set; }
    }
}
