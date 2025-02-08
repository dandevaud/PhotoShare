
using BlazorDownloadFile;
using Blazored.LocalStorage;
using Ljbc1994.Blazor.IntersectionObserver;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PhotoShare.Client;
using PhotoShare.Client.BusinessLogic;
using PhotoShare.Client.Shared.Models;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<LocalApplicationStorageHandler>();
builder.Services.AddScoped<StreamHandler>();
builder.Services.AddSingleton<StateContainer>();
builder.Services.AddIntersectionObserver();
builder.Services.AddBlazorDownloadFile(ServiceLifetime.Scoped);

var clientHandler = new HttpClientHandler()
{
	AllowAutoRedirect = false,
};

builder.Services.AddScoped(sp => new HttpClient(clientHandler) { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress), });

await builder.Build().RunAsync();
