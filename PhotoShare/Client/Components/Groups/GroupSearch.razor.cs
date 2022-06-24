using Microsoft.AspNetCore.Components;
using PhotoShare.Shared;

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
            if (Guid.TryParse(GroupGuid, out var result))
            {
                
                var response = await Http.GetAsync($"api/Groups/{result.ToString()}");
                if (response.IsSuccessStatusCode)
                {
                    var feedback = await response.Content.ReadAsStringAsync();
                    navManager.NavigateTo($"group/{GroupGuid}");
                } else
                {
                   if( response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        notification.Notify(new Radzen.NotificationMessage()
                        {
                            Severity = Radzen.NotificationSeverity.Error,
                            Detail = $"Die angegebene Guid {GroupGuid} wurde nicht gefunden",
                            Duration = 5000,
                            Summary = "Nicht gefunden"
                        });
                    }
                }
            } else
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
    }
}
