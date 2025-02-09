using Microsoft.AspNetCore.Components;

namespace PhotoShare.Client.Components.Groups
{
	public partial class GroupSearch
	{
		private string GroupGuid;

		[Parameter]
		public bool isVertical { get; set; }
		[Parameter]
		public bool isHeader { get; set; }


		private async Task OnClick()
		{
			container.IsLoading = true;
			try
			{
				if (Guid.TryParse(GroupGuid, out var result))
				{
					var response = await Http.GetAsync($"api/Groups/{result.ToString()}", HttpCompletionOption.ResponseHeadersRead);
					if (response.IsSuccessStatusCode)
					{
						navManager.NavigateTo($"group/{GroupGuid}");
					}
					else
					{
						if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
						{
							notification.Notify(new Radzen.NotificationMessage()
							{
								Severity = Radzen.NotificationSeverity.Error,
								Detail = $"Die angegebene Guid {GroupGuid} wurde nicht gefunden",
								Duration = 5000,
								Summary = "Nicht gefunden"
							});
							return;
						}
						else if (response.ReasonPhrase == "opaqueredirect")
						{

							navManager.NavigateTo($"/Login/{GroupGuid}?RedirectUrl=/group/{GroupGuid}");
							return;
						}

						notification.Notify(new Radzen.NotificationMessage()
						{
							Severity = Radzen.NotificationSeverity.Error,
							Detail = "Unbekanter Fehler",
							Duration = 5000
						});
					}
				}
				else
				{
					notification.Notify(new Radzen.NotificationMessage()
					{
						Severity = Radzen.NotificationSeverity.Error,
						Detail = $"Die angegebene Guid {GroupGuid} ist invalid",
						Duration = 5000,
						Summary = "Invalide Guid"
					});
				}
			}
			finally
			{
				container.IsLoading = false;
			}
		}

	}
}
