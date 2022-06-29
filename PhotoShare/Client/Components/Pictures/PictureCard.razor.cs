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

        private ElementReference Card { get; set; }

        private string path = "";
        private bool isEdit = false;
        private Guid? isEditKey;

        private void SetPath()
        {
            path = "/api/Pictures/Load/"+pictureUI.picture.GroupId +"/"+ pictureUI.picture.Id;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await SetUpObserver();
            }
            await base.OnAfterRenderAsync(firstRender);
        }


        public async Task SetUpObserver()
        {
            await observer.Observe(Card, async (entries) =>
            {
                var entry = entries.FirstOrDefault();
                if (entry.IsIntersecting)
                {
                    SetPath();
                    await CheckEditRight();
                    StateHasChanged();
                }
            });
        }

      

        private async Task CheckEditRight()
        {
            var hasEditRights = false;
            if (adminKey != null && adminKey != Guid.Empty)
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


        private async Task<bool> ShowDeletionDialog()
        {
            var result = await dialog.Confirm("Löschung bestätigen!", $"Sind Sie sicher dass Sie das Bild löschen möchten? Diese Aktion kann nicht mehr Rückgängig gemacht werden", new Radzen.ConfirmOptions() { OkButtonText = "Ja", CancelButtonText = "Nein" });
            return result ?? false;
        }

        private async Task DeletePicture()
        {
            if (!(await ShowDeletionDialog())) return;
            container.IsLoading = true;
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
            container.IsLoading = false;
        }

        private string ShortenName(string name)
        {
            if (name.Length < 33) return name;
            return name.Substring(0, 30) + "...";
        }
    }
}
