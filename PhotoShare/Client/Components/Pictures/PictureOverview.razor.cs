using Microsoft.AspNetCore.Components;
using PhotoShare.Client.Shared.Models;
using PhotoShare.Shared.Response;
using System.Net.Http.Json;

namespace PhotoShare.Client.Components.Pictures
{
    public partial class PictureOverview
    {
        [Parameter]
        public Guid GroupId { get; set;}
        [Parameter]
        public Guid? AdminKey { get; set; }

        private List<PictureUIDto> _picture;

        protected async override Task OnParametersSetAsync()
        {
            var response = await client.GetAsync($"/api/Pictures/ByGroup/{GroupId}");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadFromJsonAsync<List<PictureDto>>();
                _picture = responseContent?.Select(p => new PictureUIDto()
                {
                    picture = p,
                    isSelected = false
                }).ToList() ?? new();
            } else
            {
                notification.Notify(Radzen.NotificationSeverity.Error, "Fehler aufgetreten", "Beim Laden der Gruppen Bilder ist ein Fehler aufgetreten");
            }
            await base.OnParametersSetAsync();
        }
    }
}
