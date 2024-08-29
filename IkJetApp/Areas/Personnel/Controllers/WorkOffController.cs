using IkJetApp.Areas.Personnel.Models.WorkOff;
using IkJetApp.Helpers;
using IkJetApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IkJetApp.Areas.Personnel.Controllers
{
    [Area("Personnel")]
    [Authorize(Roles ="Personnel")]
    public class WorkOffController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticatedUser _authenticatedUser;

        public WorkOffController(HttpClient httpClient, AuthenticatedUser authenticatedUser)
        {
            _httpClient = httpClient;
            _authenticatedUser = authenticatedUser;
        }
        private async Task<List<WorkOffViewModel>> GetWorkOffListAsync(string userId)
        {
            var response = await _httpClient.GetAsync($"https://ikjet-api20240824103050.azurewebsites.net/api/WorkOff/user/{userId}");

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

                    var workOffRoot = System.Text.Json.JsonSerializer.Deserialize<WorkOffRoot>(jsonString, options);
                    return workOffRoot?.Values ?? new List<WorkOffViewModel>();
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
            var workOffList = await GetWorkOffListAsync(userId.ToString());

            if (workOffList != null)
            {
                return View(workOffList);
            }

			TempData["WorkOffMessage"] = "Henüz bir izin talebiniz yok, buradan yeni talep oluşturabilirsiniz.";
			return RedirectToAction(nameof(Create));
		}

        public async Task<IActionResult> ApprovalStatusPendingList()
        {
            var userId = _authenticatedUser.GetCurrentUserId();
            var workOffList = await GetWorkOffListAsync(userId.ToString());

            if (workOffList != null)
            {
                // ApprovalStatus : Pending
                workOffList = workOffList.Where(a => a.ApprovalStatus == Enums.ApprovalStatus.Pending).ToList();
                return View(workOffList);
            }

			TempData["WorkOffMessage"] = "Henüz onay bekleyen bir izin talebiniz yok, buradan yeni talep oluşturabilirsiniz.";
			return RedirectToAction(nameof(Create));
		}


        public async Task<IActionResult> ApprovalStatusApprovedList()
        {
            var userId = _authenticatedUser.GetCurrentUserId();
            var workOffList = await GetWorkOffListAsync(userId.ToString());

            if (workOffList != null)
            {
                // ApprovalStatus : Approved
                workOffList = workOffList.Where(a => a.ApprovalStatus == Enums.ApprovalStatus.Approved).ToList(); 
                return View(workOffList);
            }

			TempData["WorkOffMessage"] = "Henüz onaylanmış bir izin talebiniz yok, buradan yeni talep oluşturabilirsiniz.";
			return RedirectToAction(nameof(Create));
		}


        public async Task<IActionResult> ApprovalStatusRejectedList()
        {
            var userId = _authenticatedUser.GetCurrentUserId();
            var workOffList = await GetWorkOffListAsync(userId.ToString());

            if (workOffList != null)
            {
                // ApprovalStatus : Rejected
                workOffList = workOffList.Where(a => a.ApprovalStatus == Enums.ApprovalStatus.Rejected).ToList();
                return View(workOffList);
            }

			TempData["WorkOffMessage"] = "Henüz reddedilmiş bir izin talebiniz yok, buradan yeni talep oluşturabilirsiniz.";
			return RedirectToAction(nameof(Create));
		}


        public async Task<IActionResult> ApprovalStatusCanceledList()
        {
            var userId = _authenticatedUser.GetCurrentUserId();
            var workOffList = await GetWorkOffListAsync(userId.ToString());

            if (workOffList != null)
            {
                // ApprovalStatus : Canceled
                workOffList = workOffList.Where(a => a.ApprovalStatus == Enums.ApprovalStatus.Canceled).ToList();
                return View(workOffList);
            }

			TempData["WorkOffMessage"] = "Henüz iptal edilmiş bir izin talebiniz yok, buradan yeni talep oluşturabilirsiniz.";
			return RedirectToAction(nameof(Create));
		}




        // GET: /Personnel/WorkOff/Create
        public IActionResult Create()
        {
            var viewModel = new WorkOffViewModel();
            return View(viewModel);
        }

        // POST: /Personnel/WorkOff/Create
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkOffViewModel viewModel)
        {
            viewModel.AppUserId = int.Parse(_authenticatedUser.GetCurrentUserId().ToString());

            if (ModelState.IsValid)
            {
                var response = await _httpClient.PostAsJsonAsync("https://ikjet-api20240824103050.azurewebsites.net/api/WorkOff", viewModel);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Message"] = "Harcama talebi başarıyla oluşturuldu!";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, "An error occurred while creating the work off.");
            }
            return View(viewModel);
        }

        // GET: /Personnel/WorkOff/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"https://ikjet-api20240824103050.azurewebsites.net/api/WorkOff/{id}");

            if (response.IsSuccessStatusCode)
            {
                var workOff = await response.Content.ReadFromJsonAsync<WorkOffViewModel>();
                return View(workOff);
            }

            return NotFound();
        }

        // POST: /Personnel/WorkOff/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WorkOffViewModel viewModel)
        {
            viewModel.AppUserId = int.Parse(_authenticatedUser.GetCurrentUserId().ToString());
            if (id != viewModel.Id || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var response = await _httpClient.PutAsJsonAsync($"https://ikjet-api20240824103050.azurewebsites.net/api/WorkOff/{id}", viewModel);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "An error occurred while updating the work off.");
            return View(viewModel);
        }

        // GET: /Personnel/WorkOff/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"https://ikjet-api20240824103050.azurewebsites.net/api/WorkOff/{id}");

            if (response.IsSuccessStatusCode)
            {
                
                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }
    }
}
