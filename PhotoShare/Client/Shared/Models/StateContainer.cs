namespace PhotoShare.Client.Shared.Models
{
    public class StateContainer
    {
        //https://docs.microsoft.com/en-us/aspnet/core/blazor/state-management?view=aspnetcore-3.1&pivots=server

        #region Loading

        private bool isLoading;

        public bool IsLoading
        {
            get => isLoading;
            set
            {
                isLoading = value;
                NotifyLoadingStateChanged();
              
            }
        }

       

        public event Action? OnLoadChange;

        private void NotifyLoadingStateChanged() => OnLoadChange?.Invoke();
        #endregion


    }
}
