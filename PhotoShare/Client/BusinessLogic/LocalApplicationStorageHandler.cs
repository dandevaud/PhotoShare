using Blazored.LocalStorage;

namespace PhotoShare.Client.BusinessLogic
{
    public class LocalApplicationStorageHandler
    {
        private ILocalStorageService storageService;

        public LocalApplicationStorageHandler(ILocalStorageService storageService)
        {
            this.storageService = storageService;
        }   


        public async Task<string> GetLocalStorage(string key,CancellationToken ct = default)
        {
           return await storageService.GetItemAsStringAsync(key, ct);
        }
    }
}
