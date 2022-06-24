using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using PhotoShare.Client.BusinessLogic;
using PhotoShare.Shared.Request;
using System.Net.Http.Json;
using static System.Net.Mime.MediaTypeNames;

namespace PhotoShare.Client.Components.Pictures
{
    public partial class PictureUpload
    {
        [Parameter]
        public Guid GroupId { get; set; }
        [Parameter]
        public EventCallback OnChange { get; set; }
        private string Uploader;
        private Guid UploaderKey;
        private static SemaphoreSlim sema = new SemaphoreSlim(200, 200);


        private async Task LoadFiles(InputFileChangeEventArgs e)
        {
            container.IsLoading = true;
            var uploadRequests = e.GetMultipleFiles(999999);
            var error = false;
            
            var tasks = uploadRequests.Select(async f => {
                await sema.WaitAsync();
            try
            {
                var data = await streamHandler.GetBytesFromBrowserfile(f);
                var request = new PictureUploadRequest(GroupId, UploaderKey, Uploader, f)
                {
                    Data = data
                };
                var response = await http.PostAsJsonAsync("/api/Pictures", request);
                if (!response.IsSuccessStatusCode) error = true;
            } finally
            {
                var openQueueSpots = sema.Release();
                    Console.WriteLine($"There are {openQueueSpots} open spots in the queue");
                }
            }
            );
            await Task.WhenAll(tasks);
            container.IsLoading = false;

            await OnChange.InvokeAsync();
            if (error)
            {
                notification.Notify(Radzen.NotificationSeverity.Error, "Fehler beim Hochladen", "Es gab einen Fehler beim Hochladen, es kann sein, dass nicht alle Bilder hochgeladen wurden.");
            } else
            {
                notification.Notify(Radzen.NotificationSeverity.Success, "Bilder erfoglreich hochgeladen");
            }
            


        }

        protected async override Task OnParametersSetAsync()
        {
            UploaderKey = await GetOrSetUploaderKey();
            await base.OnParametersSetAsync();
        }

        private async Task<Guid> GetOrSetUploaderKey()
        {
            var key = await localStorage.GetLocalStorage<Guid>("UploaderKey");
            if (key != null && key != Guid.Empty)
            {
                return key;
            }
            var guid = Guid.NewGuid();
            await localStorage.SetLocalStorageValue<Guid>("UploaderKey", guid);
            return guid;
        }

    }
}
