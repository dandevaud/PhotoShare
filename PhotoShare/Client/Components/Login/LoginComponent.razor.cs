using Microsoft.AspNetCore.Components;
using PhotoShare.Shared.Request;

namespace PhotoShare.Client.Components.Login
{
	public partial class LoginComponent
	{
		[Parameter]
		public EventCallback<LoginModelRequest> OnLoginCallback { get; set; }

		[Parameter]
		public bool IsPasswordSetup { get; set; }

		private LoginModelRequest loginModelRequest = new LoginModelRequest();

		private async Task OnLogin()
		{
			await OnLoginCallback.InvokeAsync(loginModelRequest);
		}
	}
}
