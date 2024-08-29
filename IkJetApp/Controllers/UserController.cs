
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using IkJetApp.Helpers;
using IkJetApp.Models;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using IkJetApp.Areas.Admin.Models.Company;
using Newtonsoft.Json;


namespace IkJetApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticatedUser _authenticatedUser;

        public UserController(IHttpClientFactory httpClientFactory, AuthenticatedUser authenticatedUser)
        {
            _httpClientFactory = httpClientFactory;
            _authenticatedUser = authenticatedUser;
        }


        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {


            if (!ModelState.IsValid)
                return View(loginViewModel);

            var httpClient = _httpClientFactory.CreateClient("MyHttpClient");

            var response = await httpClient.PostAsJsonAsync("https://ikjet-api20240824103050.azurewebsites.net/api/Auth", loginViewModel);

            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var identity = new ClaimsIdentity(jwtToken.Claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);


                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                {
                    IsPersistent = true
                });

                Response.Cookies.Append("jwt", token, new CookieOptions { HttpOnly = true, Secure = true });


                var userRole = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value;



                // E-mailConfirmed 

                var eMailConfirmedString = principal.Claims.FirstOrDefault(c => c.Type == "EmailConfirmed").Value;


                bool eMailConfirmed;
                bool parseResult = bool.TryParse(eMailConfirmedString, out eMailConfirmed);

                if (!eMailConfirmed)
                {
                    TempData["Message"] = "Sistemde kaydınız var ancak tarafınızdan henüz mail doğrulaması yapılmamıştır, lütfen size gelen maildeki linke tıklarak onay veriniz.";
                    return View(loginViewModel);
                }

                //


                if (userRole == "Personnel")
                    return RedirectToAction("Index", "Home", new { area = "Personnel" });

                else if (userRole == "HRManager")
                    return RedirectToAction("Index", "Home", new { area = "HRManager" });

                else if (userRole == "Admin")
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                else
                    return RedirectToAction("Index", "Home");




            }


            ViewBag.Error = "Kullanıcı adı veya şifre hatalı";
            return View(loginViewModel);
        }



        [HttpGet]

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Login", "User", new { area = "" });
        }


        public void GetCompanies()
        {

            var httpClient = _httpClientFactory.CreateClient("MyHttpClient");
            var response = httpClient.GetAsync("https://ikjet-api20240824103050.azurewebsites.net/api/Company/async").Result;

            if (response.IsSuccessStatusCode)
            {

                var jsonResponse = response.Content.ReadAsStringAsync().Result;

                var companies = JsonConvert.DeserializeObject<List<CompanyViewModel>>(jsonResponse);

                ViewData["CompanyList"] = companies;
            }
            else
                ViewData["CompanyList"] = null;

        }



        [Authorize(Roles = "HRManager, Admin")]
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userViewModel = await _authenticatedUser.GetUserByIdAsync(userId);
            UserViewModel model = new UserViewModel();


            if (userViewModel.Role == "Admin")
                GetCompanies();
            else
                model.CompanyName = userViewModel.CompanyName;

            return View(model);
        }



        [Authorize(Roles = "HRManager, Admin")]
        [HttpPost]
        public async Task<IActionResult> Register(UserViewModel model, int? id)
        {
            ModelState.Remove("Id");
            ModelState.Remove("Role");
            ModelState.Remove("Email");
            ModelState.Remove("ConfirmPassword");
            ModelState.Remove("Password");
            ModelState.Remove("CompanyName");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userViewModel = await _authenticatedUser.GetUserByIdAsync(userId);

            if (userViewModel.Role == "HRManager")
            {
                model.CompanyName = userViewModel.CompanyName;
                model.Role = "Personnel";
            }
            else if (userViewModel.Role == "Admin")
            {
                model.Role = "HRManager";

                //id ye göre company gelsin
                var httpClient3 = _httpClientFactory.CreateClient("MyHttpClient");
                var response3 = httpClient3.GetAsync($"https://ikjet-api20240824103050.azurewebsites.net/api/Company/{id}").Result;

                if (response3.IsSuccessStatusCode)
                {

                    var jsonResponse = response3.Content.ReadAsStringAsync().Result;

                    var companie = JsonConvert.DeserializeObject<CompanyViewModel>(jsonResponse);

                    model.CompanyName = companie.CompanyName;

                }


            }

            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Lütfen * ile belirtilen zorunlu alanları doldurunuz";
                return View(model);
            }

            if (!RegisterHelpers.TCValidation(model.TCIdentityNumber))
            {
                ViewBag.ErrorMessage = "Girilen T.C. Kimlik Numarası geçerli değildir. Lütfen doğru bir şekilde giriniz";
                return View(model);
            }

            if (model.BirthDate > DateTime.Today.AddYears(-18) || model.BirthDate <= DateTime.Today.AddYears(-65))
            {
                ViewBag.ErrorMessage = "Personelin yaşı yasalar gereği 18'den küçük ve 65'ten büyük olamaz. Lütfen doğum tarihini doğru giriniz.";
                return View(model);
            }

            if (model.Salary <= 17002.12)
            {
                ViewBag.ErrorMessage = "Personelin maaşı yasalar gereği net asgari ücretten az olamaz. Lütfen aylık net maaşı doğru giriniz.";
                return View(model);
            }


            //TC 2. validasyonu (aynı tc ile ikinci user kaydolmamalı)
            var httpClient2 = _httpClientFactory.CreateClient("MyHttpClient");
            var response2 = await httpClient2.GetAsync($"https://ikjetapp20240824120603.azurewebsites.net/api/AppUser/by-tc/{model.TCIdentityNumber}");

            if (response2.IsSuccessStatusCode)
            {
                var jsonResponse = await response2.Content.ReadAsStringAsync();
                var userView = JsonConvert.DeserializeObject<UserViewModel>(jsonResponse);

                if (userView is not null)
                {
                    ViewBag.ErrorMessage = "Girilen T.C. Kimlik Numarası ile kayıtlı bir kullanıcı zaten mevcuttur!";
                    return View(model);
                }


            }


            model.Password = RegisterHelpers.GeneratePassword();
            model.Email = RegisterHelpers.CreateEmailAddress(model.FirstName, model.LastName, model.CompanyName);
            model.UserName = $"{model.FirstName}.{model.LastName}".ToLower();
            model.UserName = RegisterHelpers.ConvertTurkishCharacters(model.UserName);
            model.TerminationDate = null;



            if (model.ImageFile is not null)
            {
                var array = model.ImageFile.FileName.Split('.');
                var withoutExtension = array[0];
                var extension = array[1];

                Guid guid = Guid.NewGuid();
                model.ImageName = $"{withoutExtension}_{guid}.{extension}";
            }

            object newUser = new
            {
                Email = model.Email,
                Password = model.Password,
                Role = model.Role,
                ConfirmationEmail = model.ConfirmationEmail,
                UserName = model.UserName,
                PhoneNumber = model.PhoneNumber,
                ImageName = model.ImageName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                SecondName = model.SecondName,
                SecondLastName = model.SecondLastName,
                BirthDate = model.BirthDate,
                BirthPlace = model.BirthPlace,
                TCIdentityNumber = model.TCIdentityNumber,
                HireDate = model.HireDate,
                TerminationDate = model.TerminationDate,
                Address = model.Address,
                IsActive = model.IsActive,
                JobTitle = model.JobTitle,
                Department = model.Department,
                CompanyName = model.CompanyName,
                Salary = model.Salary
            };



            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(newUser), Encoding.UTF8, "application/json");

            var httpClient = _httpClientFactory.CreateClient("MyHttpClient");

            var postResponse = await httpClient.PostAsync("https://ikjet-api20240824103050.azurewebsites.net/api/AppUser", content);

            if (postResponse.IsSuccessStatusCode)
            {

                if (model.ImageFile is not null)
                {


                    var konum = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/user", model.ImageName);

                    using (var akisOrtami = new FileStream(konum, FileMode.Create))
                    {
                        model.ImageFile.CopyTo(akisOrtami);
                    }
                }

                TempData["Message"] = "Personel ekleme başarılı!";

                if (userViewModel.Role == "HRManager")
                    return RedirectToAction("Index", "Home", new { area = "HRManager" });
                else if (userViewModel.Role == "Admin")
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                else
                    return RedirectToAction("Index", "Home");



            }
            else
            {
                ViewBag.ErrorMessage = "Kayıt başarısız, lütfen gerekli (*) tüm alanları eksiksiz ve doğru bir şekilde doldurunuz";
                return View(model);
            }
        }




        [HttpGet]
        public async Task<IActionResult> Update()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userViewModel = await _authenticatedUser.GetUserByIdAsync(userId);
            return View(userViewModel);
        }




        [HttpPost]
        public async Task<IActionResult> Update(UserViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userViewModel = await _authenticatedUser.GetUserByIdAsync(userId);
            if (model.ImageFile is not null)
            {
                var array = model.ImageFile.FileName.Split('.');
                var withoutExtension = array[0];
                var extension = array[1];

                Guid guid = Guid.NewGuid();
                model.ImageName = $"{withoutExtension}_{guid}.{extension}";

                var konum = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/user", model.ImageName);

                using (var akisOrtami = new FileStream(konum, FileMode.Create))
                {
                    model.ImageFile.CopyTo(akisOrtami);
                }
            }

            Object updateUser = new
            {
                Email = userViewModel.Email,
                Password = userViewModel.Password,
                Role = userViewModel.Role,
                UserName = userViewModel.UserName,
                PhoneNumber = model.PhoneNumber,
                ImageName = model.ImageName,
                FirstName = userViewModel.FirstName ?? "",
                LastName = userViewModel.LastName ?? "",
                SecondName = userViewModel.SecondName,
                SecondLastName = userViewModel.SecondLastName,
                BirthDate = userViewModel.BirthDate,
                BirthPlace = userViewModel.BirthPlace ?? "",
                TCIdentityNumber = userViewModel.TCIdentityNumber ?? "",
                HireDate = userViewModel.HireDate,
                TerminationDate = userViewModel.TerminationDate,
                IsActive = userViewModel.IsActive,
                JobTitle = userViewModel.JobTitle ?? "",
                Department = userViewModel.Department ?? "",
                CompanyName = userViewModel.CompanyName ?? "",
                Address = model.Address,
                Salary = userViewModel.Salary,
                ConfirmationEmail = userViewModel.ConfirmationEmail

            };

            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(updateUser), Encoding.UTF8, "application/json");

            var httpClient2 = _httpClientFactory.CreateClient("MyHttpClient");

            var updatingId = int.Parse(userId);

            var putResponse = await httpClient2.PutAsync($"https://ikjet-api20240824103050.azurewebsites.net/api/AppUser/{updatingId}", content);



            if (putResponse.IsSuccessStatusCode)
            {
                TempData["Message"] = "Güncelleme başarılı!";
                return RedirectToAction("Profile", "Home");
            }
            else
            {
                var errorContent = await putResponse.Content.ReadAsStringAsync();
                ViewBag.ErrorMessage = "Güncelleme başarısız, lütfen tüm alanları eksiksiz ve doğru bir şekilde doldurunuz";
                return View(model);
            }

        }


        [HttpPost]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var httpClient = _httpClientFactory.CreateClient("MyHttpClient");


            var deleteResponse = await httpClient.DeleteAsync($"https://ikjetapp20240824120603.azurewebsites.net/api/AppUser/{userId}");

            if (deleteResponse.IsSuccessStatusCode)
            {
                TempData["Message"] = "Kullanıcı başarıyla silindi!";
                return RedirectToAction("Index", "Home", new { area = "Personnel" });
            }
            else
            {
                var responseContent = await deleteResponse.Content.ReadAsStringAsync();
                ViewBag.ErrorMessage = $"Kullanıcı silinirken bir hata oluştu: {responseContent}";
                return RedirectToAction("Index", "Home", new { area = "Personnel" });
            }
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            UserViewModel model = new UserViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(UserViewModel model)
        {
            var httpClientGetUser = _httpClientFactory.CreateClient("MyHttpClient");
            var responseGetUser = await httpClientGetUser.GetAsync($"https://ikjet-api20240824103050.azurewebsites.net/api/AppUser/by-tc/{model.TCIdentityNumber}");

            if (!responseGetUser.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Kullanıcı bilgileri alınamadı.";
                return View(model);
            }
            var userJson = await responseGetUser.Content.ReadAsStringAsync();

            var userViewModel = System.Text.Json.JsonSerializer.Deserialize<UserViewModel>(userJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });






            if (userViewModel == null)
            {
                ViewBag.ErrorMessage = "Girilen T.C. Kimlik numarası ile eşleşen bir kullanıcı bulunamadı.";
                return View(model);
            }

            if (userViewModel.Email != model.Email)
            {
                ViewBag.ErrorMessage = "Şirket e-mail adresinizi yanlış girdiniz";
                return View(model);
            }

            model.Password = RegisterHelpers.GeneratePassword();

            Object updateUser = new
            {
                Email = userViewModel.Email,
                Password = model.Password,
                Role = userViewModel.Role,
                UserName = userViewModel.UserName,
                PhoneNumber = userViewModel.PhoneNumber,
                ImageName = userViewModel.ImageName,
                FirstName = userViewModel.FirstName ?? "",
                LastName = userViewModel.LastName ?? "",
                SecondName = userViewModel.SecondName,
                SecondLastName = userViewModel.SecondLastName,
                BirthDate = userViewModel.BirthDate,
                BirthPlace = userViewModel.BirthPlace ?? "",
                TCIdentityNumber = userViewModel.TCIdentityNumber ?? "",
                HireDate = userViewModel.HireDate,
                TerminationDate = userViewModel.TerminationDate,
                IsActive = userViewModel.IsActive,
                JobTitle = userViewModel.JobTitle ?? "",
                Department = userViewModel.Department ?? "",
                CompanyName = userViewModel.CompanyName ?? "",
                Address = userViewModel.Address,
                Salary = userViewModel.Salary,
                ConfirmationEmail = model.ConfirmationEmail

            };

            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(updateUser), Encoding.UTF8, "application/json");

            var httpClientUpdate = _httpClientFactory.CreateClient("MyHttpClient");


            var putResponse = await httpClientUpdate.PutAsync($"https://ikjet-api20240824103050.azurewebsites.net/api/AppUser/{userViewModel.Id}", content);



            if (putResponse.IsSuccessStatusCode)
            {
                //api isteği mail isteği yazılacak

                var httpClientSendEmail = _httpClientFactory.CreateClient("MyHttpClient");

                var sendResponse = await httpClientSendEmail.GetAsync($"https://ikjet-api20240824103050.azurewebsites.net/api/AppUser/send-email?userId={userViewModel.Id}&password={model.Password}");


                TempData["Message"] = "Şifre sıfırlama başarılı! E-postanızı kontrol edebilirsiniz.";

                return RedirectToAction("Login", "User", new { area = "" });
            }
            else

            {
                var errorContent = await putResponse.Content.ReadAsStringAsync();
                ViewBag.ErrorMessage = "Şifre güncelleme başarısız!";
                return View(model);
            }




        }

        [HttpGet]
        public IActionResult ChangeMyPassword()
        {
            UserViewModel model = new UserViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeMyPassword(ChangePasswordModel model)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userViewModel = await _authenticatedUser.GetUserByIdAsync(userId);
            var httpClientCheck = _httpClientFactory.CreateClient("MyHttpClient");
            var passwordCheck = new { UserId = userId, Password = model.OldPassword };
            var passwordCheckContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(passwordCheck), Encoding.UTF8, "application/json");

            var responseCheck = await httpClientCheck.PostAsync($"https://ikjet-api20240824103050.azurewebsites.net/api/Auth/CheckPassword", passwordCheckContent);

            var responseContent = await responseCheck.Content.ReadAsStringAsync();
            var isPasswordValid = System.Text.Json.JsonSerializer.Deserialize<bool>(responseContent);

            if (!isPasswordValid)
            {
                ViewBag.ErrorMessage = "Eski şifrenizi hatalı girdiniz";
                return View(model);
            }

            if (model.NewPassword != model.RepeatPassword)
            {
                ViewBag.ErrorMessage = "Yeni şifreniz ve yeni şifre tekrarınız uyuşmuyor.";
                return View(model);
            }

            if (!RegisterHelpers.IsPasswordValid(model.NewPassword))
            {
                ViewBag.ErrorMessage = "Yeni şifreniz en az 1 büyük harf, 1 küçük harf, 1 rakam ve 9 karakter uzunluğunda olmalıdır.";
                return View(model);
            }


            Object updateUser = new
            {
                Email = userViewModel.Email,
                Password = model.NewPassword,
                Role = userViewModel.Role,
                UserName = userViewModel.UserName,
                PhoneNumber = userViewModel.PhoneNumber,
                ImageName = userViewModel.ImageName,
                FirstName = userViewModel.FirstName ?? "",
                LastName = userViewModel.LastName ?? "",
                SecondName = userViewModel.SecondName,
                SecondLastName = userViewModel.SecondLastName,
                BirthDate = userViewModel.BirthDate,
                BirthPlace = userViewModel.BirthPlace ?? "",
                TCIdentityNumber = userViewModel.TCIdentityNumber ?? "",
                HireDate = userViewModel.HireDate,
                TerminationDate = userViewModel.TerminationDate,
                IsActive = userViewModel.IsActive,
                JobTitle = userViewModel.JobTitle ?? "",
                Department = userViewModel.Department ?? "",
                CompanyName = userViewModel.CompanyName ?? "",
                Address = userViewModel.Address,
                Salary = userViewModel.Salary,
                ConfirmationEmail = userViewModel.ConfirmationEmail

            };





            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(updateUser), Encoding.UTF8, "application/json");

            var httpClient2 = _httpClientFactory.CreateClient("MyHttpClient");

            var updatingId = int.Parse(userId);

            var putResponse = await httpClient2.PutAsync($"https://ikjet-api20240824103050.azurewebsites.net/api/AppUser/{updatingId}", content);



            if (putResponse.IsSuccessStatusCode)
            {
                TempData["Message"] = "Şifreniz başarıyla değiştirildi!";
                return RedirectToAction("Profile", "Home");
            }
            else
            {
                var errorContent = await putResponse.Content.ReadAsStringAsync();
                ViewBag.ErrorMessage = "Güncelleme başarısız, lütfen tüm alanları eksiksiz ve doğru bir şekilde doldurunuz";
                return View(model);
            }




        }


        [Authorize(Roles = "Admin")]
        public IActionResult UserStatistics()
        {

            return View();
        }













    }
}
