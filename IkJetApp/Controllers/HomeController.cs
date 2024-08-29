
using IkJetApp.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Reflection;
using IkJetApp.Helpers;

namespace IkJetApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticatedUser _authenticatedUser;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, HttpClient httpClient, IHttpClientFactory httpClientFactory, AuthenticatedUser authenticatedUser)
        {
            _logger = logger;
            _httpClient = httpClient;
            _httpClientFactory = httpClientFactory;
            _authenticatedUser = authenticatedUser;
        }



        public IActionResult Index()
        {
            return View();
        }
   
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userViewModel =await _authenticatedUser.GetUserByIdAsync(userId);
           
            return View(userViewModel);
        }



        public async Task<IActionResult> Detail()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userViewModel = await _authenticatedUser.GetUserByIdAsync(userId);
            return View(userViewModel);
        }


        public async Task<IActionResult> EmailContact(string name, string email, string subject, string message)
        {
            var url = $"https://ikjet-api20240824103050.azurewebsites.net/api/AppUser/contact-email?name={name}&email={email}&subject={subject}&message={message}";


            var httpClientSendEmail = _httpClientFactory.CreateClient("MyHttpClient");

            var sendResponse = await httpClientSendEmail.GetAsync(url);


            if (sendResponse.IsSuccessStatusCode)
            {
				TempData["Message"] = "Mesajınız iletildi. En kısa sürede tarafınıza ulaşım sağlayacağız!";
                return RedirectToAction(nameof(Index));
            }

            


            return Ok();

        }






        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
