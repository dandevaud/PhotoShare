using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoShare.Server.Contracts;
using PhotoShare.Shared.Request;
using System.Security.Claims;

namespace PhotoShare.Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LoginController : ControllerBase
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

				return Ok();
			}
			return Unauthorized();
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return Redirect("/");
		}

		[HttpPost("SetPassword")]
		[AllowAnonymous]
		public async Task<IActionResult> SetPassword(LoginModelRequest model)
		{
			if (await _identityService.SetPasswordForGroup(model.GroupId, model.Password))
			{
				await Login(model);
				return Ok();
			}
			return Unauthorized();
		}
	}
}
