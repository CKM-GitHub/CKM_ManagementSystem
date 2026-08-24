using CKM_ManagementSystem.Data;
using CKM_ManagementSystem.Services;
using CKM_ManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.BL.Interface;
using CKM_ManagementSystem.DL;
using CKM_ManagementSystem.DL.Interface;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 1. DbContext Register 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. IDepartmentService 
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<BaseDL>();
builder.Services.AddScoped<DepartmentBL>();
builder.Services.AddScoped<IChangePasswordBL, ChangePasswordBL>();
builder.Services.AddScoped<IChangePasswordDL, ChangePasswordDL>();
builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options =>
{
    options.Cookie.Name = "CKM_AuthCookie";
    options.LoginPath = "/LoginUsers/Login";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
});

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

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Departments}/{action=Entry}/{id?}");

app.Run();