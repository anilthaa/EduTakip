using System.Text;
using EduTakip.Components;
using EduTakip.Models;
using EduTakip.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

string connectionString;

connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<EduTakip.Models.EduTakipDBContext>(options =>
    options.UseSqlServer(connectionString));
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        options.DetailedErrors = true;
    });

var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection.GetValue<string>("Key");
if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("Jwt:Key configuration is missing.");
}

builder.Services.AddRadzenComponents();
builder.Services.Configure<JwtOptions>(jwtSection);
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(1),
        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],
        IssuerSigningKey = signingKey
    };
});

builder.Services.AddAuthorization();

// HttpClient for API calls - BaseAddress will be set in AuthService
builder.Services.AddHttpClient<IAuthService, AuthService>();

// Auth services
builder.Services.AddScoped<AuthStateService>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<AuthStateService>());

builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<ContextMenuService>();
builder.Services.AddScoped<TooltipService>();

// Toast Notification Service
builder.Services.AddSingleton<IToastService, ToastService>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

var authGroup = app.MapGroup("/api/auth");

authGroup.MapPost("/register", async (RegisterRequest request, EduTakipDBContext db, IPasswordHasher hasher) =>
{
    var email = request.EPosta.Trim().ToLowerInvariant();

    // Kullanıcı tipi belirtilmemişse veya geçersizse, ilk aktif kullanıcı tipini kullan
    var userType = await db.KullaniciTipleri.FindAsync(request.KullaniciTip_ID);
    if (userType is null || !userType.Aktif)
    {
        userType = await db.KullaniciTipleri.FirstOrDefaultAsync(x => x.Aktif);
        if (userType is null)
        {
            return Results.BadRequest("Sistemde aktif kullanıcı tipi bulunamadı.");
        }
    }

    var exists = await db.Kullanicilar.AnyAsync(x => x.EPosta == email);
    if (exists)
    {
        return Results.Conflict("E-posta zaten kayıtlı.");
    }

    var user = new ItemKullanici
    {
        Adi = request.Adi.Trim(),
        Soyadi = request.Soyadi.Trim(),
        EPosta = email,
        KullaniciTip_ID = request.KullaniciTip_ID,
        KullaniciTip = userType,
        Sifre = hasher.Hash(request.Sifre),
        Aktif = true,
        CreUser = 0,
        CreDate = DateTime.UtcNow
    };

    db.Kullanicilar.Add(user);
    await db.SaveChangesAsync();

    var dto = new AuthUserDto(user.ID, user.Adi, user.Soyadi, user.EPosta, user.KullaniciTip.Adi);
    return Results.Created($"/api/auth/users/{user.ID}", dto);
});

authGroup.MapPost("/login", async (LoginRequest request, EduTakipDBContext db, IPasswordHasher hasher, IJwtService jwtService) =>
{
    var email = request.EPosta.Trim().ToLowerInvariant();
    var user = await db.Kullanicilar
        .Include(x => x.KullaniciTip)
        .FirstOrDefaultAsync(x => x.EPosta == email && x.Aktif);

    if (user is null || !hasher.Verify(user.Sifre, request.Sifre))
    {
        return Results.BadRequest("E-posta veya şifre hatalı.");
    }

    var token = jwtService.CreateToken(user);
    var dto = new AuthUserDto(user.ID, user.Adi, user.Soyadi, user.EPosta, user.KullaniciTip?.Adi ?? string.Empty);

    return Results.Ok(new AuthResponse(token, dto));
});

// Kullanıcı tiplerini getir
authGroup.MapGet("/user-types", async (EduTakipDBContext db) =>
{
    var types = await db.KullaniciTipleri
        .Where(x => x.Aktif)
        .Select(x => new { x.ID, x.Adi })
        .ToListAsync();
    return Results.Ok(types);
});

app.Run();
