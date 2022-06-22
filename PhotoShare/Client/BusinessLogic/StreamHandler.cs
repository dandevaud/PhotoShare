using Microsoft.AspNetCore.Components.Forms;

namespace PhotoShare.Client.BusinessLogic
{
    public class StreamHandler
    {

        public async Task<byte[]> GetBytesFromBrowserfile(IBrowserFile file)
        {
            using var stream = file.OpenReadStream(file.Size+1);
            using var ms = new MemoryStream();

            await stream.CopyToAsync(ms);
            return ms.ToArray();

        }
    }
}
