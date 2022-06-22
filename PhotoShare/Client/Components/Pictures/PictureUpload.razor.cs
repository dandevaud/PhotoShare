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


        private async Task LoadFiles(InputFileChangeEventArgs e)
        {
            var uploadRequests = e.GetMultipleFiles().Select(async f => {
                var data = await streamHandler.GetBytesFromBrowserfile(f);
                var request = new PictureUploadRequest(GroupId, UploaderKey, Uploader, f)
                {
                    Data = data
                };
                await http.PostAsJsonAsync("/api/Pictures", request);
            }
            );
            await Task.WhenAll(uploadRequests);
            
            await OnChange.InvokeAsync();
            StateHasChanged();


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
