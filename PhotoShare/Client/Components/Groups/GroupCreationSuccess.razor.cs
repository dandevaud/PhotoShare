using Microsoft.AspNetCore.Components;
using PhotoShare.Shared.Request;
using Radzen;
using System.Net;
using System.Net.Http.Json;

namespace PhotoShare.Client.Components.Groups
{
	public partial class GroupCreationSuccess
	{
		[Parameter]
		public string groupId { get; set; }

		[Parameter]
		public string adminkey { get; set; }

		private bool? hasPassword;

		private async Task setPassword(LoginModelRequest loginModelRequest)
		{
			loginModelRequest.GroupId = Guid.Parse(groupId);

			var response = await Http.PostAsJsonAsync("api/Login/SetPassword", loginModelRequest);
			if (response.StatusCode == HttpStatusCode.OK)
			{
				hasPassword = true;

				notification.Notify(new NotificationMessage()
				{
					Severity = NotificationSeverity.Success,
					Detail = "Passwort wurde erfolgreich gesetzt",
					Summary = "Gruppe aktualisiert"
				});
			}
		}

	}
}
