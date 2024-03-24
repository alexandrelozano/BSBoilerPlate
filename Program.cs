using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using BSBoilerPlate.Data;
using MudBlazor.Services;
using BSBoilerPlate.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using BSBoilerPlate.Services;
using Blazored.LocalStorage;
using Blazored.SessionStorage;

var builder = WebApplication.CreateBuilder(args);

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
// Begin I18N configuration
builder.Services.AddLocalization(options => options.ResourcesPath = "Localization");
// End I18N configuration
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddScoped<AppService>();
builder.Services.AddScoped<QuestPDFService>();
builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddDbContext<BSBoilerPlateContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    );
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddEventAggregator();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<BSBoilerPlateContext>();

    DbInitializer.Initialize(context);
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// Begin I18N configuration
var supportedCultures = new[] { "en-EN", "es-ES", "ca-ES" };

var localizationOptions = new RequestLocalizationOptions()
  .SetDefaultCulture(supportedCultures[2])
  .AddSupportedCultures(supportedCultures)
  .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);
// End I18N configuration

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();