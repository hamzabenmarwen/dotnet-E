using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TP2.Components;
using TP2.Models;
using TP2.Models.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContextPool<AppDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("ProductDBConnection"
)));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategorieRepository, CategoryRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<CartViewComponent>();
// Register wishlist services
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
builder.Services.AddScoped<WishlistViewComponent>();
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();
builder.Services.Configure<IdentityOptions>(options =>

{
    // Default Password settings.
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
