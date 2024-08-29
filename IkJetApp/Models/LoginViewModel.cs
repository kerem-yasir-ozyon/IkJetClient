using System.ComponentModel.DataAnnotations;

namespace IkJetApp.Models
{
    public class LoginViewModel
    {
        [EmailAddress]
        public string Email { get; set; }


        [DataType(DataType.Password)]
        public string Password { get; set; }

       

    }
}
