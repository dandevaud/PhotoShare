using Microsoft.AspNetCore.Components;
using PhotoShare.Client.Shared.Models;
using PhotoShare.Shared.Response;
using System.Net.Http.Json;

namespace PhotoShare.Client.Components.Pictures
{
    public partial class PictureCard
    {
        [Parameter]
        public PictureUIDto pictureUI { get; set; }

        [Parameter]
        public Guid? adminKey { get; set; }

        [Parameter]
        public EventCallback OnChange { get; set; }

        private string path = "";
        private bool isEdit = false;
        private Guid? isEditKey;

        private void SetPath()
        {
            path = "/api/Pictures/Load/"+pictureUI.picture.GroupId +"/"+ pictureUI.picture.Id;
        }

        protected async override Task OnParametersSetAsync()
        {
            await CheckEditRight();
            SetPath();
            await base.OnParametersSetAsync();
            StateHasChanged();
        }

        private async Task CheckEditRight()
        {
            var hasEditRights = false;
            if (adminKey != null)
            {
               var response = await http.GetAsync($"api/pictures/HasAdminRights/{pictureUI.picture.GroupId}/{pictureUI.picture.Id}?adminKey={adminKey}");
                if (response.IsSuccessStatusCode)
                {
                    hasEditRights = (await response.Content.ReadAsStringAsync()).Equals("true",StringComparison.InvariantCultureIgnoreCase);
                    if (hasEditRights) isEditKey = adminKey;
                }
            } 
            if (!hasEditRights)
            {
                var uploaderKey = await store.GetLocalStorage<Guid>("UploaderKey");
                var response = await http.GetAsync($"api/pictures/HasAdminRights/{pictureUI.picture.GroupId}/{pictureUI.picture.Id}?adminKey={uploaderKey}");
                if (response.IsSuccessStatusCode)
                {
                    hasEditRights = (await response.Content.ReadAsStringAsync()).Equals("true", StringComparison.InvariantCultureIgnoreCase);
                    if (hasEditRights) isEditKey = uploaderKey;
                }
            }
            isEdit = hasEditRights;
            StateHasChanged();
        }

        private async Task DeletePicture()
        {
           
            var response = await http.DeleteAsync($"api/pictures/{pictureUI.picture.GroupId}/{pictureUI.picture.Id}/{isEditKey}");
            if (!response.IsSuccessStatusCode)
            {
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.Forbidden:
                        notification.Notify(Radzen.NotificationSeverity.Error, "Zu wenig Rechte", "Das Bild konnte nicht gelöscht werden da die entsprechenden Rechte fehlen");
                        break;
                    default:
                        notification.Notify(Radzen.NotificationSeverity.Error, "Fehler", "Fehler beim Löschen des Bildes bitte versuchen Sie es noch einmal");
                        break;
                }
            } else
            {
                notification.Notify(Radzen.NotificationSeverity.Success, "Erfolgreich gelöscht", "Bild wurde erfolgreich gelöscht");
            }
            await OnChange.InvokeAsync();
        }
    }
}
