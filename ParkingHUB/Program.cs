using Blazored.Toast;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using ParkingHUB.Data;
using ParkingHUB.Interface;
using ParkingHUB.Models;
using ParkingHUB.Repository;
using ParkingHUB.ViewModel;
using PdfSharp.Charting;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
});
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IParking, ParkingRepository>();
builder.Services.AddScoped<IEmail, EmailService>();
builder.Services.AddScoped<IPagination<ParkingListViewModel>, PaginationRepository<ParkingListViewModel>>();
builder.Services.AddScoped(typeof(PaginationRepository<>));
builder.Services.AddTransient<Seed>();
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(1);
});
builder.Services.AddBlazoredToast();
builder.Services.AddSession(options =>
{
    options.Cookie.IsEssential = true; 
});
var app = builder.Build();

if (args.Length == 1 && args[0].ToLower() == "seeddata")
{
    await Seed.SeedUsersAndRolesAsync(app);
    Seed.SeedData(app);
}

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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");
app.MapControllerRoute(
        name: "logout",
        pattern: "Home/Logout",
        defaults: new { controller = "Home", action = "Login" });

app.MapControllerRoute(
    name: "resetpassword",
    pattern: "Home/ResetPassword/{token?}", 
    defaults: new { controller = "Home", action = "ResetPassword" });


app.UseSession();

app.Run();
