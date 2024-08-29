using IkJetApp.Areas.Personnel.Models.Prepayment;
using IkJetApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;
using IkJetApp.Areas.Personnel.Models.Expense;
using System.Reflection;
using IkJetApp.Models;
using Microsoft.AspNetCore.Authorization;
using IkJetApp.Helpers;

namespace IkJetApp.Areas.Personnel.Controllers
{
    [Area("Personnel")]
    [Authorize(Roles = "Personnel")]

    public class ExpenseController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticatedUser _authenticatedUser;

        public ExpenseController(HttpClient httpClient, AuthenticatedUser authenticatedUser)
        {
            _httpClient = httpClient;
            _authenticatedUser = authenticatedUser;
        }

        private async Task<List<ExpenseViewModel>> GetExpenseListAsync(string userId)
        {
            var response = await _httpClient.GetAsync($"https://ikjet-api20240824103050.azurewebsites.net/api/Expense/user/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                Console.WriteLine(jsonString);

                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new JsonStringEnumConverter() }
                    };

                    var expenseRoot = System.Text.Json.JsonSerializer.Deserialize<ExpenseRoot>(jsonString, options);
                    return expenseRoot?.Values ?? new List<ExpenseViewModel>();
                }
                catch (System.Text.Json.JsonException ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }

            return null;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _authenticatedUser.GetCurrentUserId();
            var prepaymentList = await GetExpenseListAsync(userId.ToString());
            if (prepaymentList != null)
            {
                return View(prepaymentList);
            }

            TempData["ExpenseMessage"] = "Henüz bir harcama talebiniz yok, buradan yeni talep oluşturabilirsiniz.";
            return RedirectToAction(nameof(Create));
        }

        public async Task<IActionResult> ApprovalStatusPendingList()
        {
            var userId = _authenticatedUser.GetCurrentUserId();

            var expenseList = await GetExpenseListAsync(userId.ToString());

            if (expenseList != null)
            {
                // ApprovalStatus : Pending
                expenseList = expenseList.Where(a => a.ApprovalStatus == Enums.ApprovalStatus.Pending).ToList();
                return View(expenseList);
            }
            else
            {
                TempData["ExpenseMessage"] = "Henüz onay bekleyen bir harcama talebiniz yok, buradan yeni talep oluşturabilirsiniz.";
                return RedirectToAction(nameof(Create));
            }


        }


        public async Task<IActionResult> ApprovalStatusApprovedList()
        {
            var userId = _authenticatedUser.GetCurrentUserId();

            var expenseList = await GetExpenseListAsync(userId.ToString());

            if (expenseList != null)
            {
                // ApprovalStatus : Approved
                expenseList = expenseList.Where(a => a.ApprovalStatus == Enums.ApprovalStatus.Approved).ToList();
                return View(expenseList);
            }

            else
            {
                TempData["ExpenseMessage"] = "Henüz onay verilmiş bir harcama talebiniz yok, buradan yeni talep oluşturabilirsiniz.";
                return RedirectToAction(nameof(Create));
            }
        }


        public async Task<IActionResult> ApprovalStatusRejectedList()
        {
            var userId = _authenticatedUser.GetCurrentUserId();

            var expenseList = await GetExpenseListAsync(userId.ToString());

            if (expenseList != null)
            {
                // ApprovalStatus : Rejected
                expenseList = expenseList.Where(a => a.ApprovalStatus == Enums.ApprovalStatus.Rejected).ToList();
                return View(expenseList);
            }

            TempData["ExpenseMessage"] = "Henüz reddedilmiş bir harcama talebiniz yok, buradan yeni talep oluşturabilirsiniz.";
            return RedirectToAction(nameof(Create));
        }


        public async Task<IActionResult> ApprovalStatusCanceledList()
        {
            var userId = _authenticatedUser.GetCurrentUserId();

            var expenseList = await GetExpenseListAsync(userId.ToString());

            if (expenseList != null)
            {
                // ApprovalStatus : Canceled
                expenseList = expenseList.Where(a => a.ApprovalStatus == Enums.ApprovalStatus.Canceled).ToList();
                return View(expenseList);
            }

            TempData["ExpenseMessage"] = "Henüz iptal edilmiş bir harcama talebiniz yok, buradan yeni talep oluşturabilirsiniz.";
            return RedirectToAction(nameof(Create));
        }


        //---------------------------------------------------


        // GET: /Personnel/Expense/Create
        public IActionResult Create()
        {

            var viewModel = new ExpenseViewModel();
            return View(viewModel);
        }

        // POST: /Personnel/Expense/Create
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpenseViewModel viewModel)
        {
            viewModel.AppUserId = int.Parse(_authenticatedUser.GetCurrentUserId().ToString());

            if (viewModel.ImageFile is not null)
            {
                var array = viewModel.ImageFile.FileName.Split('.');
                var withoutExtension = array[0];
                var extension = array[1];

                Guid guid = Guid.NewGuid();
                viewModel.ImageName = $"{withoutExtension}_{guid}.{extension}";

                var konum = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/expense", viewModel.ImageName);

                using (var akisOrtami = new FileStream(konum, FileMode.Create))
                {
                    viewModel.ImageFile.CopyTo(akisOrtami);
                }
            }
            else
            {
                viewModel.ImageName = null;
            }



            if (ModelState.IsValid)
            {
                var response = await _httpClient.PostAsJsonAsync("https://ikjet-api20240824103050.azurewebsites.net/api/Expense", viewModel);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Message"] = "Harcama talebi başarıyla oluşturuldu!";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, "An error occurred while creating the work off.");
            }
            return View(viewModel);
        }

        // GET: /Personnel/Expense/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"https://ikjet-api20240824103050.azurewebsites.net/api/Expense/{id}");

            if (response.IsSuccessStatusCode)
            {
                var prepayment = await response.Content.ReadFromJsonAsync<ExpenseViewModel>();
                return View(prepayment);
            }

            return NotFound();
        }

        // POST: /Personnel/Expense/Edit/5
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ExpenseViewModel viewModel)
        {

            var responseGet = await _httpClient.GetAsync($"https://ikjet-api20240824103050.azurewebsites.net/api/Expense/{viewModel.Id}");

            var prepayment = await responseGet.Content.ReadAsStringAsync();
            var prepaymentModel = JsonSerializer.Deserialize<ExpenseViewModel>(prepayment, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            viewModel.ImageName = prepaymentModel.ImageName;
            viewModel.AppUserId = int.Parse(_authenticatedUser.GetCurrentUserId().ToString());

            //dosya yükleme
            if (viewModel.ImageFile is not null)
            {
                var konum = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/expense", viewModel.ImageFile.FileName);
                viewModel.ImageName = viewModel.ImageFile.FileName;

                var akisOrtami = new FileStream(konum, FileMode.Create);


                viewModel.ImageFile.CopyTo(akisOrtami);

            }
            var response = await _httpClient.PutAsJsonAsync($"https://ikjet-api20240824103050.azurewebsites.net/api/Expense/{id}", viewModel);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "An error occurred while updating the work off.");
            return View(viewModel);
        }

        // GET: /Personnel/Prepayment/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"https://ikjet-api20240824103050.azurewebsites.net/api/Expense/{id}");

            if (response.IsSuccessStatusCode)
            {

                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }




    }
}
