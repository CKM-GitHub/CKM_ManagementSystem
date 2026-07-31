using CKM_ManagementSystem.Data;
using CKM_ManagementSystem.Services;
using CKM_ManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using CKM_ManagementSystem.DL;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 1. DbContext Register 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. IDepartmentService 
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<BaseDL>();
builder.Services.AddScoped<DepartmentDL>();

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