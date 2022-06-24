using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using PhotoShare.Server.Database.Configuration;
using PhotoShare.Server.Database.Context;
using PhotoShare.Server.IoC;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.BindServices();


builder.Services.AddDbContext<PhotoShareContext>(options => options.AddCorrectDatabase(builder.Configuration));
if (string.IsNullOrEmpty(builder.Configuration.GetValue<string>("FileSaveLocation"))) {
    var dict = new Dictionary<string, string>()
    {
        {"FileSaveLocation", Environment.CurrentDirectory + "/Photos" }
    };
    builder.Configuration.AddInMemoryCollection(dict);
    
}

DirectoryInfo saveDir = new DirectoryInfo(builder.Configuration.GetValue<string>("FileSaveLocation"));
if (!saveDir.Exists)
{
    saveDir.Create();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
