using StatusPainel.Services;

var builder = WebApplication.CreateBuilder(args);

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

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();