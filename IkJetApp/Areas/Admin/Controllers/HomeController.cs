using IkJetApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace IkJetApp.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class HomeController : Controller
	{
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticatedUser _authenticatedUser;

        public HomeController(IHttpClientFactory httpClientFactory, AuthenticatedUser authenticatedUser)
        {
            _httpClientFactory = httpClientFactory;
            _authenticatedUser = authenticatedUser;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userViewModel = await _authenticatedUser.GetUserByIdAsync(userId);

            return View(userViewModel);
        }


    }
}
