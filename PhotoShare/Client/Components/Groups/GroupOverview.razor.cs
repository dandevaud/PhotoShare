using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Radzen;
using System.Net.Http.Json;
using Group = PhotoShare.Shared.Group;

namespace PhotoShare.Client.Components.Groups
{
	public partial class GroupOverview : ComponentBase
	{
		[Parameter]
		public string GroupIdString
		{
			get => GroupId.ToString();
			set
			{
				GroupId = Guid.Parse(value);
			}
		}

		public Guid GroupId { get; set; }

		private Guid adminKey { get; set; }
		private Group Group { get; set; }
		private bool isAdmin;
		private bool editName;
		private bool editDescription;

		protected async override Task OnParametersSetAsync()
		{
			SetAdminKey();
			await GetGroup();

			await base.OnParametersSetAsync();
		}

		private void SetAdminKey()
		{
			var query = new Uri(nav.Uri).Query;
			var parsed = QueryHelpers.ParseNullableQuery(query);
			adminKey = (parsed?.TryGetValue("adminkey", out var value) ?? false) ? Guid.TryParse(value, out var result) ? result : Guid.Empty : Guid.Empty;

		}

		private async Task GetGroup()
		{
			container.IsLoading = true;
			try
			{
				var getGroup = http.GetAsync($"/api/groups/{GroupId}");
				var getIsAdmin = http.GetAsync($"/api/groups/hasAccess/{GroupId}?adminkey={adminKey}");
				var groupdResponse = await getGroup;
				var isAdminResponse = await getIsAdmin;
				if (groupdResponse.IsSuccessStatusCode)
				{
					Group = await groupdResponse.Content.ReadFromJsonAsync<Group>() ?? new Group();
				}
				else if (groupdResponse.ReasonPhrase == "opaqueredirct")
				{

					nav.NavigateTo($"/Login/{GroupId}?redirectUrl={nav.Uri.ToString()}");

				}
				else
				{
					await ShowErrorLoadingGroup();
					return;
				}
				if (isAdminResponse.IsSuccessStatusCode)
				{
					isAdmin = (await isAdminResponse.Content.ReadAsStringAsync()).Equals("true", StringComparison.InvariantCultureIgnoreCase);
				}
			}
			finally
			{
				container.IsLoading = false;
			}
		}

		private async Task UpdatedGroupAsync()
		{
			var response = await http.PutAsJsonAsync($"/api/groups/{GroupId}?accesskey={adminKey}", Group);
			if (response.IsSuccessStatusCode)
			{
				notification.Notify(new NotificationMessage()
				{
					Severity = NotificationSeverity.Success,
					Detail = "Gruppe wurde erfolgreich aktualisert",
					Summary = "Gruppe aktualisiert"
				});
			}
			else
			{
				notification.Notify(new NotificationMessage()
				{
					Severity = NotificationSeverity.Error,
					Detail = "Fehler beim Aktualisieren der Gruppe ist aufgetreten",
					Summary = "Fehler"
				});
			}
		}

		private async Task DeleteGroup()
		{
			var response = await ShowDeletionDialog();
			if (response)
			{
				var deleteResponse = await http.DeleteAsync($"/api/groups/{GroupId}?accesskey={adminKey}");
				if (deleteResponse.IsSuccessStatusCode)
				{
					notification.Notify(NotificationSeverity.Success, "Gruppe wurde gelöscht");
					nav.NavigateTo(nav.BaseUri);
				}
				else
				{
					notification.Notify(NotificationSeverity.Error, "Fehler beim Löschen der Gruppe", "Ein Fehler beim Löschen der Gruppe ist aufgetreten, bitte versuchen Sie es erneut");
				}

			}
		}
	}
}
