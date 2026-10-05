using Baikuredo.Web;
using Baikuredo.Web.Services.Abstractions;
using Baikuredo.Web.Services.Implementations;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.AddCustomConfiguration();
builder.Services.AddScoped<ICategoriasService, CategoriasService>();
WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.AddCustomWebApplicationConfiguration();

app.Run();
