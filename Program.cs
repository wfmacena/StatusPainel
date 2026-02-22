using StatusPainel.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StatusPainel.Data;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("StatusPainelContextConnection") ?? throw new InvalidOperationException("Connection string 'StatusPainelContextConnection' not found.");

// ALTERADO: StatusPainelContext → ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

// ALTERADO: StatusPainelContext → ApplicationDbContext
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // ADICIONADO para suporte a roles
    .AddEntityFrameworkStores<ApplicationDbContext>();

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

app.UseAuthentication();  // ADICIONADO - estava faltando!
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();