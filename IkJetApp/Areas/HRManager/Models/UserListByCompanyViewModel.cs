namespace IkJetApp.Areas.HRManager.Models
{
    public class UserListByCompanyViewModel
    {
        public int Id { get; set; } // Id alanı gizlenecek

        public string? ImageName { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SecondName { get; set; }
        public string SecondLastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public string TCIdentityNumber { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public bool IsActive { get; set; }
        public string JobTitle { get; set; }
        public string Department { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public double Salary { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
