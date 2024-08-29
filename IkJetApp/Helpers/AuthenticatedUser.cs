using IkJetApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;

namespace IkJetApp.Helpers
{
    public class AuthenticatedUser
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthenticatedUser(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UserViewModel> GetUserByIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;

            var httpClient = _httpClientFactory.CreateClient("MyHttpClient");
            var response = await httpClient.GetAsync($"https://ikjet-api20240824103050.azurewebsites.net/api/AppUser/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var userJson = await response.Content.ReadAsStringAsync();
            var userViewModel = JsonSerializer.Deserialize<UserViewModel>(userJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return userViewModel;
        }

        public int? GetCurrentUserId()
        {
            var userIdString = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdString, out int userId))
            {
                return userId;
            }
            return null;
        }
    }
}
