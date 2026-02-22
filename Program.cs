using StatusPainel.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StatusPainel.Data;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("StatusPainelContextConnection") ?? throw new InvalidOperationException("Connection string 'StatusPainelContextConnection' not found.");

builder.Services.AddDbContext<StatusPainelContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<StatusPainelContext>();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();

// Registra o HttpClient e o GameStatusService
builder.Services.AddHttpClient();
builder.Services.AddSingleton<GameStatusService>();

// Adiciona servi�o em background para atualiza��es autom�ticas
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

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();