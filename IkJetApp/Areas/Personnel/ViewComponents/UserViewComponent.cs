
using IkJetApp.Helpers;
using IkJetApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace IkJetApp.Areas.Personnel.ViewComponents
{
    public class UserViewComponent : ViewComponent
    {
        private readonly AuthenticatedUser _authenticatedUser;

        public UserViewComponent(AuthenticatedUser authenticatedUser)
        {
            _authenticatedUser = authenticatedUser;
        }

        public async Task<IViewComponentResult> InvokeAsync() 
        {
            var userId = _authenticatedUser.GetCurrentUserId();
            var userViewModel = await _authenticatedUser.GetUserByIdAsync(userId.ToString());

            return View(userViewModel);
        }
    }
}
