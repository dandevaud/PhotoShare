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

        private string path = "";
        private bool isEdit = false;

        private void SetPath()
        {
            path = "/api/Pictures/Load/"+pictureUI.picture.GroupId + pictureUI.picture.Id;
        }

        protected async override Task OnParametersSetAsync()
        {
            await CheckEditRight();
            SetPath();
            await base.OnParametersSetAsync();
        }

        private async Task CheckEditRight()
        {
            var hasEditRights = false;
            if (adminKey != null)
            {
               var response = await http.GetAsync($"api/pictures/HasAdminRights/{pictureUI.picture.GroupId}/{pictureUI.picture.Id}?adminKey={adminKey}");
                if (response.IsSuccessStatusCode)
                {
                    hasEditRights = await response.Content.ReadFromJsonAsync<bool>();

                }
            } 
            if (!hasEditRights)
            {
                var uploaderKey = store.GetLocalStorage<Guid>("UploaderKey");
                var response = await http.GetAsync($"api/pictures/HasAdminRights/{pictureUI.picture.GroupId}/{pictureUI.picture.Id}?adminKey={uploaderKey}");
                if (response.IsSuccessStatusCode)
                {
                    hasEditRights = await response.Content.ReadFromJsonAsync<bool>();

                }
            }
        }
    }
}
