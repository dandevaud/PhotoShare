using Microsoft.AspNetCore.Components;
using PhotoShare.Shared;
using PhotoShare.Shared.Response;
using System.Net.Http.Json;

namespace PhotoShare.Client.Components.Groups
{
    public partial class GroupCreation
    {
        [Parameter]
        public bool isStandalone { get; set; } = true;

        private Group Group { get; set; } = new Group();

        private async Task Create() {
            if (string.IsNullOrEmpty(Group.Name))
            {
                notification.Notify(new Radzen.NotificationMessage()
                {
                    Severity = Radzen.NotificationSeverity.Error,
                    Detail = "Bitte wählen Sie einen Namen für die Gruppe aus",
                    Duration = 5000,
                    Summary = "Invalider Name"
                });
                return;
            }
            var response = await client.PostAsJsonAsync<Group>("/api/Groups",Group);
            if (!response.IsSuccessStatusCode)
            {
                notification.Notify(new Radzen.NotificationMessage()
                {
                    Severity = Radzen.NotificationSeverity.Error,
                    Detail = "Ein Fehler ist beim abspeichern aufgetreten, versuchen Sie es nochmals",
                    Summary = "Fehler"
                });
                return;
            }
            var resp = await response.Content.ReadFromJsonAsync<GroupCreationResponse>();
            nav.NavigateTo($"/group/success/{resp.Group.Id}/{resp.AdministrationKey}");
        }
    }
}
