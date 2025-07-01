using Antohny.Components;
using Antohny.Components.Account;
using Antohny.Data;
using Antohny.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// ----------------------
// Servicios Blazor
// ----------------------
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ----------------------
// Autenticación
// ----------------------
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

// ----------------------
// Servicios propios
// ----------------------
builder.Services.AddSingleton<FirebaseLoginService>();

// HttpClient configurado para usar la API en Render
builder.Services.AddHttpClient<ChatGptService>(client =>
{
    client.BaseAddress = new Uri("https://rendimasapp.onrender.com/"); // ← URL de tu backend en Render
});

// ----------------------
// Controladores y API
// ----------------------
builder.Services.AddControllers();

// CORS: si consumes desde otro dominio (opcional)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://rendimasapp.onrender.com") // ← o cambia a tu frontend si lo separas
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ----------------------
// Identity y DB
// ----------------------
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
.AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// ----------------------
// Construcción de la app
// ----------------------
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Activar CORS
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

// Mapeo de controladores para la API
app.MapControllers();

// Mapeo de componentes Blazor
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Mapeo de endpoints de identidad
app.MapAdditionalIdentityEndpoints();

app.Run();
