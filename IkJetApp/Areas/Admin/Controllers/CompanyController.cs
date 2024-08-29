using IkJetApp.Areas.Admin.Models.Company;
using IkJetApp.Enums;
using IkJetApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Reflection;

namespace IkJetApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CompanyController : Controller
    {
        private readonly HttpClient _httpClient;

        public CompanyController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }





        public async Task<IActionResult> AllHRManagers()
        {
            var url = "https://ikjet-api20240824103050.azurewebsites.net/api/AppUser/GetAllHRManager";

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var userList = JsonConvert.DeserializeObject<List<UserViewModel>>(jsonResponse);

                var groupedUsers = userList.GroupBy(user => user.CompanyName)
                                   .ToDictionary(g => g.Key, g => g.ToList());

                return View(groupedUsers);
            }
            else
                return View("Error");



        }



        // GET: /Admin/Company/Create
        public IActionResult Create()
        {

            var viewModel = new CompanyViewModel();
            return View(viewModel);
        }

        // POST: /Admin/Company/Create
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompanyViewModel viewModel)
        {
            // Mersis numarası kontrolü
            if (viewModel.MersisNumber.ToString().Length != 16)
            {
                ViewBag.ErrorMessage = "Mersis numarası 16 haneli olmalıdır. !!!";
                return View(viewModel);
            }

            // Vergi numarası kontrolü
            if (viewModel.TaxNumber.ToString().Length != 10)
            {
                ViewBag.ErrorMessage = "Vergi numarası 10 haneli olmalıdır. !!!";
                return View(viewModel);
            }

            //Vergi Numarası uniquelik

            var response = await _httpClient.GetAsync($"https://ikjet-api20240824103050.azurewebsites.net/api/Company/by-taxnumber/{viewModel.TaxNumber}");
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var company = JsonConvert.DeserializeObject<CompanyViewModel>(jsonResponse);

            if (company is not null)
            {
                ViewBag.ErrorMessage = "Bu vergi numarasına ait şirket kaydı mevcuttur !!!";
                return View(viewModel);
            }

            if (viewModel.ImageFile is not null)
            {
                var array = viewModel.ImageFile.FileName.Split('.');
                var withoutExtension = array[0];
                var extension = array[1];

                Guid guid = Guid.NewGuid();
                viewModel.ImageName = $"{withoutExtension}_{guid}.{extension}";

                var konum = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/user", viewModel.ImageName);

                using (var akisOrtami = new FileStream(konum, FileMode.Create))
                {
                    viewModel.ImageFile.CopyTo(akisOrtami);
                }
            }
            else
            {
                viewModel.ImageName = null;
            }

            viewModel.IsActive = true;

            if (ModelState.IsValid)
            {
                var response2 = await _httpClient.PostAsJsonAsync("https://ikjet-api20240824103050.azurewebsites.net/api/Company", viewModel);

                if (response2.IsSuccessStatusCode)
                {
                    TempData["Message"] = "Şirket başarıyla kaydedildi.";
                    return RedirectToAction(nameof(List));
                }

                ModelState.AddModelError(string.Empty, "An error occurred while creating the work off.");
            }

            return View(viewModel);
        }


        // GET: /Admin/Company/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"https://ikjet-api20240824103050.azurewebsites.net/api/Company/{id}");
            if (response.IsSuccessStatusCode)
            {
                //var company = await response.Content.ReadFromJsonAsync<CompanyViewModel>();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var company = JsonConvert.DeserializeObject<CompanyViewModel>(jsonResponse);

                if (company != null)
                {
                    return View(company);
                }
                return NotFound();
            }

            ModelState.AddModelError(string.Empty, "An error occurred while retrieving the company details.");
            return RedirectToAction(nameof(List));
        }

        // POST: /Admin/Company/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, CompanyViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return BadRequest();
            }


            // Mersis numarası kontrolü
            if (viewModel.MersisNumber.ToString().Length != 16)
            {
                ViewBag.ErrorMessage = "Mersis numarası 16 haneli olmalıdır. !!!";
                return View(viewModel);
            }

            // Vergi numarası kontrolü
            if (viewModel.TaxNumber.ToString().Length != 10)
            {
                ViewBag.ErrorMessage = "Vergi numarası 10 haneli olmalıdır. !!!";
                return View(viewModel);
            }

          

            var response = await _httpClient.GetAsync($"https://ikjet-api20240824103050.azurewebsites.net/api/Company/{id}");

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var company = JsonConvert.DeserializeObject<CompanyViewModel>(jsonResponse);

            viewModel.ImageName = company.ImageName;

            if (ModelState.IsValid)
            {
                if (viewModel.ImageFile is not null)
                {
                    var array = viewModel.ImageFile.FileName.Split('.');
                    var withoutExtension = array[0];
                    var extension = array[1];

                    Guid guid = Guid.NewGuid();
                    viewModel.ImageName = $"{withoutExtension}_{guid}.{extension}";

                    var konum = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/user", viewModel.ImageName);

                    using (var akisOrtami = new FileStream(konum, FileMode.Create))
                    {
                        viewModel.ImageFile.CopyTo(akisOrtami);
                    }
                }
                

                var response2 = await _httpClient.PutAsJsonAsync($"https://ikjet-api20240824103050.azurewebsites.net/api/Company/{id}", viewModel);

                if (response2.IsSuccessStatusCode)
                {
                    TempData["Message"] = "Değişiklikler kaydedildi!";
                    return RedirectToAction(nameof(List));
                }

                ModelState.AddModelError(string.Empty, "An error occurred while updating the company.");
            }
            return View(viewModel);
        }

        // GET: /Admin/Company/List
        public async Task<IActionResult> List()
        {
            var response = await _httpClient.GetAsync("https://ikjet-api20240824103050.azurewebsites.net/api/Company");
            if (response.IsSuccessStatusCode)
            {

                //var companies = await response.Content.ReadFromJsonAsync<IEnumerable<CompanyViewModel>>();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var companies = JsonConvert.DeserializeObject<IEnumerable<CompanyViewModel>>(jsonResponse);

                return View(companies);
            }

            ModelState.AddModelError(string.Empty, "An error occurred while retrieving the companies.");
            return View(new List<CompanyViewModel>());
        }


        // POST: /Admin/Company/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"https://ikjet-api20240824103050.azurewebsites.net/api/Company/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Şirket başarıyla silindi";
                return RedirectToAction(nameof(List));
            }

            ModelState.AddModelError(string.Empty, "An error occurred while deleting the company.");
            return RedirectToAction(nameof(List));
        }



        
       







    }
}
