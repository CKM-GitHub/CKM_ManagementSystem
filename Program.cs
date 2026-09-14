using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.Data;
using CKM_ManagementSystem.MenuBL;
using CKM_ManagementSystem.DL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 2. IDepartmentService 
builder.Services.AddScoped<BaseDL>();
builder.Services.AddScoped<DepartmentBL>();
builder.Services.AddScoped<Menu_BL>();
builder.Services.AddScoped<MainMenuBL>();
builder.Services.AddScoped<RoleBL>();
// builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<LoginUserBL>();
builder.Services.AddScoped<ChangePasswordBL>();
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.Cookie.Name = "CKM_AuthCookie";
        options.LoginPath = "/LoginUsers/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddScoped<UserEntryBL>();
builder.Services.AddScoped<UserListBL>();
builder.Services.AddScoped<PasswordService>();
var app = builder.Build();

app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("Error/500");
    app.UseHsts();
}



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