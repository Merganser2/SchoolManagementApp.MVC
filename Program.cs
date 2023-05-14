using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using Microsoft.EntityFrameworkCore;
using SchoolManagementApp.MVC.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the IoC container.
var conn = builder.Configuration.GetConnectionString("SchoolManagementLocalConnection");
// Options that will get passed down to our DbContext in the options
builder.Services.AddDbContext<SchoolMgmtContext>(q => q.UseSqlServer(conn));
builder.Services.AddControllersWithViews();
// Add Notification service and set up its options
builder.Services.AddNotyf(c =>
{
    c.DurationInSeconds = 5;
    c.IsDismissable = true;
    c.Position = NotyfPosition.TopRight;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// app.UseAuthentication; TODO: add this later for Auth0
app.UseAuthorization();
app.UseNotyf(); // Middleware for toast notifications

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
