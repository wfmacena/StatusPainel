using StatusPainel.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StatusPainel.Data;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("StatusPainelContextConnection") ?? throw new InvalidOperationException("Connection string 'StatusPainelContextConnection' not found.");

builder.Services.AddDbContext<StatusPainelContext>(options => options.UseSqlServer(connectionString));

// Registrar identidade com suporte a roles
builder.Services
    .AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<StatusPainelContext>();

// Configurar cookie de autenticação
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();

// Registra o HttpClient e o GameStatusService
builder.Services.AddHttpClient();
builder.Services.AddSingleton<GameStatusService>();

// Adiciona serviço em background para atualizações automáticas
builder.Services.AddHostedService<GameStatusBackgroundService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ORDEM CORRETA - Authentication antes de Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // 1. Cria a role "Admin" se ela não existir
    string roleName = "Admin";
    if (!await roleManager.RoleExistsAsync(roleName))
    {
        await roleManager.CreateAsync(new IdentityRole(roleName));
    }

    // 2. Cria o usuário admin se ele não existir
    string adminEmail = "admin@statuspainel.com";
    string adminPassword = "Admin@123456";

    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, roleName);
        }
    }
}

app.Run();