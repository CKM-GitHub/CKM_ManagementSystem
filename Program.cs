using CKM_ManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.DL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<BaseDL>();
builder.Services.AddScoped<DepartmentBL>();
builder.Services.AddScoped<ProjectBL>();
var app = builder.Build();

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