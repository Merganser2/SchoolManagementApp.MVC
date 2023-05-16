using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using Auth0.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using SchoolManagementApp.MVC.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the IoC container.
var conn = builder.Configuration.GetConnectionString("SchoolManagementLocalConnection");
// Options that will get passed down to our DbContext in the options
builder.Services.AddDbContext<SchoolMgmtContext>(q => q.UseSqlServer(conn));

// Auth0. Will look in Configuration file, find the Auth0 section, and find the 
//  relevant keys
builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0:Domain"];
    options.ClientId = builder.Configuration["Auth0:ClientId"];
});

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

app.UseAuthentication(); // Auth0 service
app.UseAuthorization();
app.UseNotyf(); // Middleware for toast notifications

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
