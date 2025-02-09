using Microsoft.AspNetCore.Components;
using PhotoShare.Shared.Request;
using System.Net.Http.Json;

namespace PhotoShare.Client.Pages
{
	public partial class Login
	{
		[Parameter]
		public string GroupId { get; set; }

		[Parameter]
		[SupplyParameterFromQuery]
		public string RedirectUrl { get; set; } = "/";



		private async Task HandleSubmit(LoginModelRequest loginModelRequest)
		{
			loginModelRequest.GroupId = Guid.Parse(GroupId);
			var response = await client.PostAsJsonAsync("/api/Login", loginModelRequest);
			if (response.IsSuccessStatusCode)
			{
				nav.NavigateTo(RedirectUrl);
			}
			else
			{
				notification.Notify(Radzen.NotificationSeverity.Error, "Nicht authorisiert", "Login ist fehlgeschlagen");
			}

		}

	}
}
