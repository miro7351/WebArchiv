


/*
 * MH: 05.04.2022

 * aplikacia pouziva DB: TOYOTA_T1  na MH: Dell
 * "DefaultConnection": "Server=HRABCAK;Database=TOYOTA_T1;Trusted_Connection=True;MultipleActiveResultSets=true"
 * ----------------------------
 * 
 * https://www.sharepointcafe.net/2022/01/create-first-mvc-application-using-net-6-in-visual-studio-2022.html
 * there is no more startup.cs file in .NET 6. 
 * It can be thought of as a major change in .NET 6 framework.
   .NET 6 startup.cs file code is merged in Program.cs file.
 */


using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;
using PA.TOYOTA.DB;
using ToyotaArchiv.Infrastructure;
using ToyotaArchiv.Services;

var builder = WebApplication.CreateBuilder(args);
//WebApplication.CreateBuilder initializes a new instance of the WebApplicationBuilder class with preconfigured defaults. 

#region --- Adding services in container ----

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

//--toto sa robilo v Startup.cs--
//The name of the connection string is passed in to the context by calling a method on a DbContextOptionsBuilder object
//For local development, the ASP.NET Core configuration system reads the connection string from the appsettings.json file.
var conString = builder.Configuration.GetConnectionString("DefaultConnection");

//nacitanie nastavenia z konfig suboru
var openFileString = builder.Configuration["AutoOpenFile"];
Boolean.TryParse(openFileString, out bool openFile);   
ToyotaArchiv.Global.AppData.AutoOpenFile = openFile;    

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true; //???
    options.Cookie.IsEssential = true;//???
});

builder.Services.AddDbContext<ToyotaContext>(options => options.UseSqlServer(conString));

//pre login
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Unauthorized/";
        options.AccessDeniedPath = "/Account/Forbidden/";
    });


builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//MH: registracia IZakazkaTransformService Funkcie pre transformaciu Instacie typu ZakazkaZO <-> Zakazka
builder.Services.AddSingleton<IZakazkaTransformService, ZakazkaServiceWeb>();



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
app.UseSession();
// HttpContext.Session is available after session state is configured.
//Call UseSession after UseRouting and before UseEndpoints.
//---

// pre login
app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.None,
    HttpOnly = HttpOnlyPolicy.Always

});
app.UseAuthentication();

app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.Run();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Accounts}/{action=Login}/{id?}");

app.Run();
