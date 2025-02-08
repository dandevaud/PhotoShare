using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoShare.Server.Contracts;
using PhotoShare.Shared.Request;
using System.Security.Claims;

namespace PhotoShare.Server.Controllers
{
	public class LoginController : Controller
	{
		public LoginController(IIdentityService identityService)
		{
			_identityService = identityService;
		}
		private IIdentityService _identityService;


		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Login(LoginModelRequest model)
		{
			if (await _identityService.HasAccess(model.GroupId, model.Password))
			{
				var claims = new List<Claim>
				{
					new Claim(ClaimTypes.Name, model.GroupId.ToString())
				};

				var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
				await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

				return Redirect(model.ReturnUrl.ToString());
			}
			return Unauthorized();
		}
	}
}
