using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Resolved.Data;
using Resolved.Data.Models;
using Resolved.Models.ApplicationUser;

namespace Resolved.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationUser _userService;
        private readonly IUpload _uploadServie;

        public ProfileController(
            UserManager<ApplicationUser> userManager, 
            IApplicationUser userService,
            IUpload uploadServie)
        {
            _userManager = userManager;
            _userService = userService;
            _uploadServie = uploadServie;
        }

        public IActionResult Detail(string id)
        {
            var user = _userService.GetById(id);
            var userRoles = _userManager.GetRolesAsync(user).Result;

            var model = new ProfileModel()
            {
                UserId = user.Id,
                UserName = user.UserName,
                UserRating = user.Rating.ToString(),
                Email = user.Email,
                ProfileImageUrl = user.ProfileImageUrl,
                MemberSince = user.MemberSince,
                IsAdmin = userRoles.Contains("Admin"),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult>UploadProfileImage(IFormFile file)
        {
            var userId = _userManager.GetUserId(User);
            return RedirectToAction("Detail", "Profile", new { id = userId });
        }
    }
}