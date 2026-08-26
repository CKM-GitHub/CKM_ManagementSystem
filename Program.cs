using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.DL;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<BaseDL>();


builder.Services.AddScoped<RoleBL>(provider =>
    new RoleBL(
        provider.GetRequiredService<BaseDL>(),
        connectionString
    )
);

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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();