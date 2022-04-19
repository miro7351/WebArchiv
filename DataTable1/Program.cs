/*
 * MH: 16.04.2022
zdroj: https://codewithmukesh.com/blog/jquery-datatable-in-aspnet-core/
 generovanie poloziek to db tab. https://www.mockaroo.com/
 
 * aplikacia pouziva DB: CustomerDB  na MH: Dell
 * "DefaultConnection": "Server=HRABCAK;Database=CustomerDB;Trusted_Connection=True;MultipleActiveResultSets=true"
 * ----------------------------

 */


using DataTable1.DB.Model;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
//WebApplication.CreateBuilder initializes a new instance of the WebApplicationBuilder class with preconfigured defaults. 

#region --- Adding services in container ----

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

//--toto sa robilo v Startup.cs--
//The name of the connection string is passed in to the context by calling a method on a DbContextOptionsBuilder object
//For local development, the ASP.NET Core configuration system reads the connection string from the appsettings.json file.
var conString = builder.Configuration.GetConnectionString("DatabaseConnection");



builder.Services.AddDbContext<CustomerContext>(options => options.UseSqlServer(conString));

//builder.Services.AddDatabaseDeveloperPageExceptionFilter();



//The AddDatabaseDeveloperPageExceptionFilter provides helpful error information in the development environment.
/*z NUgetu: Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
 * ASP.NET Core middleware for Entity Framework Core error pages. 
 * Use this middleware to detect and diagnose errors with Entity Framework Core migrations.
 */


//-------------------------------

#endregion --- Adding services in container ----




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection(); //forces HTTP calls to automatically redirect to equivalent HTTPS addresses.
app.UseStaticFiles();      //pre pouzitie static files, napr. HTML, JavaScript, CSS, graficke subory

app.UseRouting();

//--MH pridane:
//app.UseSession();
// HttpContext.Session is available after session state is configured.
//Call UseSession after UseRouting and before UseEndpoints.
//---

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
