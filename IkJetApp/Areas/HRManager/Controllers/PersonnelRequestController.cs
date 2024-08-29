using IkJetApp.Areas.HRManager.Models;
using IkJetApp.Areas.Personnel.Models.Expense;
using IkJetApp.Areas.Personnel.Models.Prepayment;
using IkJetApp.Enums;
using IkJetApp.Helpers;
using IkJetApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;

namespace IkJetApp.Areas.HRManager.Controllers
{
    [Area("HRManager")]
    [Authorize(Roles = "HRManager")]
    public class PersonnelRequestController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticatedUser _authenticatedUser;
        public PersonnelRequestController(IHttpClientFactory httpClientFactory, AuthenticatedUser authenticatedUser)
        {
            _httpClientFactory = httpClientFactory;
            _authenticatedUser = authenticatedUser;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetUsersByCompany(string companyName)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userViewModel = await _authenticatedUser.GetUserByIdAsync(userId);
            var httpClient = _httpClientFactory.CreateClient("MyHttpClient");
            var response = await httpClient.GetAsync($"https://ikjet-api20240824103050.azurewebsites.net/api/HRManager/GetUsersByCompany?companyName={userViewModel.CompanyName}");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var userList = JsonConvert.DeserializeObject<List<UserListByCompanyViewModel>>(jsonResponse);

                var groupedUsers = userList
                    .GroupBy(u => u.IsActive ? "Aktif" : "Pasif")
                    .Select(g => new UserListGroupedViewModel
                    {
                        GroupName = g.Key,
                        Users = g.ToList()
                    })
                    .ToList();

                return View(groupedUsers);
            }
            else
            {
                // Hata durumu
                return View("Error");
            }
        }
    
        public async Task<IActionResult> GetExpenseRequestsByCompany()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userViewModel = await _authenticatedUser.GetUserByIdAsync(userId);
            var httpClient = _httpClientFactory.CreateClient("MyHttpClient");
            var response = await httpClient.GetAsync($"https://ikjet-api20240824103050.azurewebsites.net/api/HRManager/GetExpenseRequestsByCompany?companyName={userViewModel.CompanyName}");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var expenseList = JsonConvert.DeserializeObject<List<ExpenseRequestViewModel>>(jsonResponse);

                var groupedExpenses = expenseList
                    .GroupBy(e => e.FirstName + " " + e.LastName)
                    .Select(g => new ExpenseRequestGroupedViewModel
                    {
                        FullName = g.Key,
                        Expenses = g.ToList()
                    });

                return View(groupedExpenses);
            }
            else
            {
                TempData["Message"] = "Henüz bekleyen bir aktif harcama talebi yok!";
                return RedirectToAction("Index", "Home", new { area = "HRManager" });
            }
        }
        public async Task<IActionResult> GetPrepaymentRequestsByCompany()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userViewModel = await _authenticatedUser.GetUserByIdAsync(userId);
            var httpClient = _httpClientFactory.CreateClient("MyHttpClient");
            var response = await httpClient.GetAsync($"https://ikjet-api20240824103050.azurewebsites.net/api/HRManager/GetPrepaymentRequestsByCompany?companyName={userViewModel.CompanyName}");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var prepaymentList = JsonConvert.DeserializeObject<List<PrepaymentRequestViewModel>>(jsonResponse);

                var groupedRequests = prepaymentList
                    .GroupBy(x => x.FirstName + " " + x.LastName)
                    .Select(g => new PrepaymentRequestGroupedViewModel
                    {
                        FullName = g.Key,
                        Requests = g.ToList()
                    })
                    .ToList();

                return View(groupedRequests);
            }
            else
            {
                TempData["Message"] = "Henüz bekleyen bir aktif avans talebi yok!";
                return RedirectToAction("Index", "Home", new { area = "HRManager" });
            }
        }


        public async Task<IActionResult> GetWorkOffRequestsByCompany()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userViewModel = await _authenticatedUser.GetUserByIdAsync(userId);
            var httpClient = _httpClientFactory.CreateClient("MyHttpClient");
            var response = await httpClient.GetAsync($"https://ikjet-api20240824103050.azurewebsites.net/api/HRManager/GetWorkOffRequestsByCompany?companyName={userViewModel.CompanyName}");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var workOffList = JsonConvert.DeserializeObject<List<WorkOffRequestViewModel>>(jsonResponse);

                var groupedRequests = workOffList
                    .GroupBy(x => x.FirstName + " " + x.LastName)
                    .Select(g => new WorkOffRequestGroupViewModel
                    {
                        FullName = g.Key,
                        Requests = g.ToList()
                    })
                    .ToList();

                return View(groupedRequests);
            }
            else
            {
                TempData["Message"] = "Henüz bekleyen bir aktif izin talebi yok!";
                return RedirectToAction("Index", "Home", new { area = "HRManager" });
            }
        }


        [HttpPost]
        public async Task<IActionResult> ChangeExpenseStatus(int id, string status)
        {

            if (!Enum.TryParse<ApprovalStatus>(status, true, out var approvalStatus))
                return BadRequest("Geçersiz onay durumu.");

            var httpClient = _httpClientFactory.CreateClient("MyHttpClient");


            var url = $"https://ikjet-api20240824103050.azurewebsites.net/api/HRManager/ExpenseStatusChange?id={id}&newStatus={approvalStatus}";

            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "İşlem başarılı!";
                return RedirectToAction("GetExpenseRequestsByCompany");
            }

            else
                return View("Error");


        }



        [HttpPost]
        public async Task<IActionResult> ChangePrepaymentStatus(int id, string status)
        {

            if (!Enum.TryParse<ApprovalStatus>(status, true, out var approvalStatus))
                return BadRequest("Geçersiz onay durumu.");

            var httpClient = _httpClientFactory.CreateClient("MyHttpClient");


            var url = $"https://ikjet-api20240824103050.azurewebsites.net/api/HRManager/PrePaymentStatusChange?id={id}&newStatus={approvalStatus}";

            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "İşlem başarılı!";
                return RedirectToAction("GetPrepaymentRequestsByCompany");
            }

            else
                return View("Error");


        }


        [HttpPost]
        public async Task<IActionResult> ChangeWorkOffStatus(int id, string status)
        {

            if (!Enum.TryParse<ApprovalStatus>(status, true, out var approvalStatus))
                return BadRequest("Geçersiz onay durumu.");

            var httpClient = _httpClientFactory.CreateClient("MyHttpClient");


            var url = $"https://ikjet-api20240824103050.azurewebsites.net/api/HRManager/WorkOffStatusChange?id={id}&newStatus={approvalStatus}";
       
        

            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "İşlem başarılı!";
                return RedirectToAction("GetWorkOffRequestsByCompany");
            }

            else
                return View("Error");


        }


        [HttpPost]
        public async Task<IActionResult> ChangeUserStatus(int id)
        {

            var url = $"https://ikjet-api20240824103050.azurewebsites.net/api/AppUser/UserStatusChange?id={id}&status=false";
            var httpClient = _httpClientFactory.CreateClient("MyHttpClient");
            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "İşlem başarılı!";
                return RedirectToAction("GetUsersByCompany");
            }

            else
                return View("Error");


        }






    }
}
