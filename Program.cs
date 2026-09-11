using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.Data;
using CKM_ManagementSystem.MenuBL;
using CKM_ManagementSystem.DL;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<BaseDL>();
builder.Services.AddScoped<DepartmentBL>();
builder.Services.AddScoped<Menu_BL>();
builder.Services.AddScoped<MainMenuBL>();

// builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<LoginUserBL>();
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.Cookie.Name = "CKM_AuthCookie";
        options.LoginPath = "/LoginUsers/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });


builder.Services.AddScoped<BaseDL>();
builder.Services.AddScoped<UserEntryBL>();
builder.Services.AddScoped<UserListBL>();
builder.Services.AddScoped<PasswordService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("Error/500");
    app.UseHsts();
}

app.UseStatusCodePages(async context =>
{
    if (context.HttpContext.Response.StatusCode == StatusCodes.Status404NotFound)
    {
        context.HttpContext.Response.Redirect("https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQtu8SLcS1IQcINzO_ilRCY1APLalIhxU5Oi3eU8YUncg&s=10");
    }

    await Task.CompletedTask;
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseSession();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=DepartmentEntry}/{action=Entry}/{id?}");

app.Run();