using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using PhotoShare.Shared;
using Radzen;
using System.Net.Http.Json;
using Group = PhotoShare.Shared.Group;

namespace PhotoShare.Client.Components.Groups
{
    public partial class GroupOverview : ComponentBase
    {
        [Parameter]
        public string GroupId { get; set; }

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
            var getGroup = http.GetAsync($"/api/groups/{GroupId}");
            var getIsAdmin = http.GetAsync($"/api/groups/hasAccess/{GroupId}?adminkey={adminKey}");
            var groupdResponse = await getGroup;
            var isAdminResponse = await getIsAdmin;
            if (groupdResponse.IsSuccessStatusCode)
            {
                Group = await groupdResponse.Content.ReadFromJsonAsync<Group>() ?? new Group();
            } else
            {
                await ShowErrorLoadingGroup();
                return ;
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
            } else
            {
                notification.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Detail = "Fehler beim Aktualisieren der Gruppe ist aufgetreten",
                    Summary = "Fehler"
                });
            }
        }
    }
}
