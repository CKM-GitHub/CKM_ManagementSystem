using CKM_ManagementSystem.Data;
using CKM_ManagementSystem.Services;
using CKM_ManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.DL;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//?Session
builder.Services.AddSession();


// 1. DbContext Register 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. IDepartmentService 
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<BaseDL>();
builder.Services.AddScoped<DepartmentBL>();
builder.Services.AddScoped<MainMenuBL>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();       

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();