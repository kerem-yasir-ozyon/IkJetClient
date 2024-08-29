using IkJetApp.Areas.Personnel.Models.WorkOff;
using IkJetApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;
using IkJetApp.Areas.Personnel.Models.Prepayment;

using System.Net.Http;
using IkJetApp.Models;
using Microsoft.AspNetCore.Authorization;
using IkJetApp.Helpers;

namespace IkJetApp.Areas.Personnel.Controllers
{
    [Area("Personnel")]
    [Authorize(Roles = "Personnel")]

    public class PrepaymentController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticatedUser _authenticatedUser;

        public PrepaymentController(HttpClient httpClient, AuthenticatedUser authenticatedUser)
        {
            _httpClient = httpClient;
            _authenticatedUser = authenticatedUser;
        }



        private async Task<List<PrepaymentViewModel>> GetPrepaymentListAsync(string userId)
        {
            var response = await _httpClient.GetAsync($"https://ikjet-api20240824103050.azurewebsites.net/api/Prepayment/user/{userId}");

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

                    var prepaymentRoot = System.Text.Json.JsonSerializer.Deserialize<PrepaymentRoot>(jsonString, options);
                    return prepaymentRoot?.Values ?? new List<PrepaymentViewModel>();
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
            var prepaymentList = await GetPrepaymentListAsync(userId.ToString());
            if (prepaymentList != null)
            {
                return View(prepaymentList);
            }

            TempData["PrepaymentMessage"] = "Henüz bir avans talebiniz yok, buradan yeni talep oluşturabilirsiniz.";
            return RedirectToAction(nameof(Create));
        }

        public async Task<IActionResult> ApprovalStatusPendingList()
        {
            var userId = _authenticatedUser.GetCurrentUserId();
            var prepaymentList = await GetPrepaymentListAsync(userId.ToString());

            if (prepaymentList != null)
            {
                // ApprovalStatus : Pending
                prepaymentList = prepaymentList.Where(a => a.ApprovalStatus == Enums.ApprovalStatus.Pending).ToList();
                return View(prepaymentList);
            }

			TempData["PrepaymentMessage"] = "Henüz onay bekleyen bir avans talebiniz yok, buradan yeni talep oluşturabilirsiniz.";
			return RedirectToAction(nameof(Create));
		}


        public async Task<IActionResult> ApprovalStatusApprovedList()
        {
            var userId = _authenticatedUser.GetCurrentUserId();
            var prepaymentList = await GetPrepaymentListAsync(userId.ToString());

            if (prepaymentList != null)
            {
                // ApprovalStatus : Approved
                prepaymentList = prepaymentList.Where(a => a.ApprovalStatus == Enums.ApprovalStatus.Approved).ToList();
                return View(prepaymentList);
            }

			TempData["PrepaymentMessage"] = "Henüz onaylanmış bir avans talebiniz yok, buradan yeni talep oluşturabilirsiniz.";
			return RedirectToAction(nameof(Create));
		}


        public async Task<IActionResult> ApprovalStatusRejectedList()
        {
            var userId = _authenticatedUser.GetCurrentUserId();
            var prepaymentList = await GetPrepaymentListAsync(userId.ToString());

            if (prepaymentList != null)
            {
                // ApprovalStatus : Rejected
                prepaymentList = prepaymentList.Where(a => a.ApprovalStatus == Enums.ApprovalStatus.Rejected).ToList();
                return View(prepaymentList);
            }

			TempData["PrepaymentMessage"] = "Henüz reddedilmiş bir avans talebiniz yok, buradan yeni talep oluşturabilirsiniz.";
			return RedirectToAction(nameof(Create));
		}


        public async Task<IActionResult> ApprovalStatusCanceledList()
        {
            var userId = _authenticatedUser.GetCurrentUserId();
            var prepaymentList = await GetPrepaymentListAsync(userId.ToString());

            if (prepaymentList != null)
            {
                // ApprovalStatus : Canceled
                prepaymentList = prepaymentList.Where(a => a.ApprovalStatus == Enums.ApprovalStatus.Canceled).ToList();
                return View(prepaymentList);
            }

			TempData["PrepaymentMessage"] = "Henüz iptal edilmiş bir avans talebiniz yok, buradan yeni talep oluşturabilirsiniz.";
			return RedirectToAction(nameof(Create));
		}


        //---------------------------------------------------


        // GET: /Personnel/Prepayment/Create
        public IActionResult Create()
        {
            var viewModel = new PrepaymentViewModel();
            return View(viewModel);
        }

        // POST: /Personnel/Prepayment/Create
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PrepaymentViewModel viewModel)
        {
            viewModel.AppUserId = int.Parse(_authenticatedUser.GetCurrentUserId().ToString());

            //islemler

            var userViewModel = await _authenticatedUser.GetUserByIdAsync(viewModel.AppUserId.ToString());


            if ((viewModel.Amount) > userViewModel.Salary * 3)
            {
                ViewBag.ErrorMessage = "Talep edeceğiniz avans miktari maaşınızın en fazla 3 katı olabilir.";
                return View();
            }


            //islemler

            if (ModelState.IsValid)
            {
                var response = await _httpClient.PostAsJsonAsync("https://ikjet-api20240824103050.azurewebsites.net/api/Prepayment", viewModel);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Message"] = "Harcama talebi başarıyla oluşturuldu!";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, "An error occurred while creating the work off.");
            }
            return View(viewModel);
        }

        // GET: /Personnel/Prepayment/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"https://ikjet-api20240824103050.azurewebsites.net/api/Prepayment/{id}");

            if (response.IsSuccessStatusCode)
            {
                var prepayment = await response.Content.ReadFromJsonAsync<PrepaymentViewModel>();
                return View(prepayment);
            }

            return NotFound();
        }

        // POST: /Personnel/Prepayment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PrepaymentViewModel viewModel)
        {
            viewModel.AppUserId = int.Parse(_authenticatedUser.GetCurrentUserId().ToString());
            if (id != viewModel.Id || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var response = await _httpClient.PutAsJsonAsync($"https://ikjet-api20240824103050.azurewebsites.net/api/Prepayment/{id}", viewModel);

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
            var response = await _httpClient.DeleteAsync($"https://ikjet-api20240824103050.azurewebsites.net/api/Prepayment/{id}");

            if (response.IsSuccessStatusCode)
            {

                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }

        // POST: /Personnel/WorkOff/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Delete(int id, IFormCollection form)
        //{
        //    var response = await _httpClient.DeleteAsync($"https://ikjet-api20240824103050.azurewebsites.net/api/WorkOff/{id}");

        //    if (response.IsSuccessStatusCode)
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }

        //    return BadRequest();
        //}


        
    }
}
