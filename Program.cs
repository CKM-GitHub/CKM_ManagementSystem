using CKM_ManagementSystem.Authorization;
using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.Data;
using CKM_ManagementSystem.DL;
using CKM_ManagementSystem.MenuBL;
using CKM_ManagementSystem.Permissions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<BaseDL>();
builder.Services.AddScoped<DepartmentBL>();
builder.Services.AddScoped<Menu_BL>();
builder.Services.AddScoped<MainMenuBL>();
builder.Services.AddScoped<RoleBL>();
builder.Services.AddScoped<ProjectBL>();
builder.Services.AddScoped<LoginUserBL>();
builder.Services.AddScoped<ChangePasswordBL>();
builder.Services.AddScoped<UserPermissionBL>();
builder.Services.AddScoped<TaskStatusesBL>();
builder.Services.AddScoped<UserEntryBL>();
builder.Services.AddScoped<UserListBL>();
builder.Services.AddScoped<PasswordService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CurrentUserPermission>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

// ---------- Authentication ----------
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.Cookie.Name = "CKM_AuthCookie";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

        options.LoginPath = "/LoginUsers/Login";
        options.AccessDeniedPath = "/Error/StatusCode/403";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;

        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = ctx =>
            {
                if (ctx.Request.Path.StartsWithSegments("/LoginUsers") ||
                    ctx.Request.Path.StartsWithSegments("/Error"))
                {
                    ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                }
                ctx.Response.Redirect(ctx.RedirectUri);
                return Task.CompletedTask;
            },
            OnRedirectToAccessDenied = ctx =>
            {
                if (ctx.Request.Path.StartsWithSegments("/LoginUsers") ||
                    ctx.Request.Path.StartsWithSegments("/Error"))
                {
                    ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                }
                ctx.Response.Redirect(ctx.RedirectUri);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/StatusCode/500");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.UseStatusCodePagesWithReExecute("/Error/StatusCode/{0}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=LoginUsers}/{action=Login}/{id?}");

app.Run();