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


        public async Task<T?> GetLocalStorage<T>(string key,CancellationToken ct = default)
        {
           return await storageService.GetItemAsync<T?>(key, ct);
        }

        public async Task SetLocalStorageValue<T>(string key, T value, CancellationToken ct = default)
        {
            await storageService.SetItemAsync<T>(key, value, ct);
        }
    }
}
