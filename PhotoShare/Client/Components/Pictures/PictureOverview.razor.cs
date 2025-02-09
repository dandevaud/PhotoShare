using BlazorDownloadFile;
using Microsoft.AspNetCore.Components;
using PhotoShare.Client.Shared.Models;
using PhotoShare.Shared.Response;
using System.Net.Http.Json;

namespace PhotoShare.Client.Components.Pictures
{
	public partial class PictureOverview
	{
		[Parameter]
		public Guid GroupId { get; set; }
		[Parameter]
		public Guid? AdminKey { get; set; }

		private List<PictureUIDto> _picture;
		[Inject] IBlazorDownloadFileService downloadFileService { get; set; }


		protected async override Task OnParametersSetAsync()
		{
			await SetPictures();
			await base.OnParametersSetAsync();
		}

		private async Task SetPictures()
		{
			container.IsLoading = true;
			try
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
				}
				else
				{
					notification.Notify(Radzen.NotificationSeverity.Error, "Fehler aufgetreten", "Beim Laden der Gruppen Bilder ist ein Fehler aufgetreten");
				}
			}
			finally
			{
				container.IsLoading = false;
			}
			StateHasChanged();

		}

		private async Task OnChange()
		{
			await SetPictures();
			StateHasChanged();
		}

		private void SelectAll()
		{
			container.IsLoading = true;
			try
			{
				_picture.ForEach(p => p.isSelected = true);
			}
			finally
			{
				container.IsLoading = false;
			}
		}

		private void DeSelectAll()
		{
			_picture.ForEach(p => p.isSelected = false);
		}

		private async void DownloadSelection()
		{
			container.IsLoading = true;
			try
			{
				var toDownload = _picture.Where(p => p.isSelected).Select(p => p.picture.Id).ToList();
				if (NoPictureSelected(toDownload, "Nichts zum Herunterladen"))
				{
					return;
				}
				var response = await client.PostAsJsonAsync($"/api/Pictures/Load/{GroupId}", toDownload);
				if (!response.IsSuccessStatusCode)
				{
					notification.Notify(Radzen.NotificationSeverity.Error, "Fehler beim Herunterladen", "Ein Fehler beim Herunterladen der Bilder ist aufgetreten, bitte versuchen Sie es erneut.");
				}
				else
				{

					await downloadFileService.DownloadFile($"{GroupId}.zip", response.Content.ReadAsStream(), contentType: response.Content.Headers.ContentType.MediaType);
				}


			}
			finally
			{
				container.IsLoading = false;
			}

		}

		private bool NoPictureSelected<TItem>(IEnumerable<TItem> pictureSleection, string errorTitle)
		{
			if (!pictureSleection.Any())
			{
				notification.Notify(Radzen.NotificationSeverity.Warning, errorTitle, "Es wurden keine Bilder ausgewählt");
				return true;
			}
			return false;
		}

		private async Task DeleteSelection()
		{

			var delete = await ShowDeletionDialog();
			if (delete)
			{
				var toDelete = _picture.Where(p => p.isSelected);
				if (NoPictureSelected(toDelete, "Nichts zum Löschen"))
				{
					return;
				}
				SemaphoreSlim sema = new SemaphoreSlim(100, 100);

				container.IsLoading = true;
				try
				{
					foreach (var picture in toDelete)
					{
						await sema.WaitAsync();
						try
						{
							HttpResponseMessage? response = null;
							if (AdminKey != null && AdminKey != Guid.Empty) { response = await client.DeleteAsync($"api/pictures/{picture.picture.GroupId}/{picture.picture.Id}/{AdminKey}"); }
							if (!(response?.IsSuccessStatusCode ?? false))
							{
								var guid = await store.GetLocalStorage<Guid>("UploaderKey");
								if (guid != null && guid != Guid.Empty)
								{
									await client.DeleteAsync($"api/pictures/{picture.picture.GroupId}/{picture.picture.Id}/{guid}");
								}
							}
						}
						finally
						{
							sema.Release();
						}

					}
				}
				finally
				{
					container.IsLoading = false;
				}
				await SetPictures();
				StateHasChanged();
			}

		}
	}
}
