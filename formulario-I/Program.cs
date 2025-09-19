using FormularioL.Services;
using formulario_I.Components; // Ajusta el namespace del App si difiere

var builder = WebApplication.CreateBuilder(args);

// 1) Razor Components + modo interactivo servidor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 2) Antiforgery: registra el servicio
builder.Services.AddAntiforgery();

// 3) DI del servicio de correo
builder.Services.Configure<FormularioL.Services.EmailOptions>(
    builder.Configuration.GetSection("Email"));

builder.Services.AddSingleton<FormularioL.Services.IEmailService,
                               FormularioL.Services.EmailService>();

// (Opcional) User Secrets en dev
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// 4) Pipeline HTTP
app.UseHttpsRedirection();

// Estáticos (usa MapStaticAssets en proyectos Razor Components)
app.MapStaticAssets();

// 5) **ANTIFORGERY MIDDLEWARE**
app.UseAntiforgery();

// 6) Endpoints de Razor Components
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

await app.RunAsync();
