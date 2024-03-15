using Blazored.Toast;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ParkingHUB.Data;
using ParkingHUB.Interface;
using ParkingHUB.Models;
using ParkingHUB.Repository;
using ParkingHUB.ViewModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IParking, ParkingRepository>();
builder.Services.AddScoped<IPagination<ParkingListViewModel>, PaginationRepository<ParkingListViewModel>>();
builder.Services.AddScoped(typeof(PaginationRepository<>));
builder.Services.AddTransient<Seed>();
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();
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
app.UseSession();

app.Run();
