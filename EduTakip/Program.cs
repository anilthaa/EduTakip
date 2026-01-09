using System.Security.Claims;
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
        IssuerSigningKey = signingKey,
        RoleClaimType = ClaimTypes.Role,
        NameClaimType = ClaimTypes.Name
    };
    
    // JWT token'dan role claim'ini map et
    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var roleClaim = context.Principal?.FindFirst(ClaimTypes.Role);
            if (roleClaim != null && context.Principal?.Identity is ClaimsIdentity identity)
            {
                if (!identity.HasClaim(ClaimTypes.Role, roleClaim.Value))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
                }
            }
            return Task.CompletedTask;
        }
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

// Büyük dosya yüklemeleri için limitleri artır
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
	options.MultipartBodyLengthLimit = 50 * 1024 * 1024; // 50MB
	options.ValueLengthLimit = 50 * 1024 * 1024; // 50MB
	options.MultipartHeadersLengthLimit = 50 * 1024 * 1024; // 50MB
});

// Kestrel için request body size limitini artır
builder.WebHost.ConfigureKestrel(options =>
{
	options.Limits.MaxRequestBodySize = 50 * 1024 * 1024; // 50MB
});

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

    // Kullanıcı tipi validasyonu
    if (!request.KullaniciTip_ID.HasValue || request.KullaniciTip_ID.Value <= 0)
    {
        return Results.BadRequest("Kullanıcı tipi seçilmelidir.");
    }

    // Kullanıcı tipi belirtilmemişse veya geçersizse, ilk aktif kullanıcı tipini kullan
    var userType = await db.KullaniciTipleri.FindAsync(request.KullaniciTip_ID.Value);
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
        KullaniciTip_ID = request.KullaniciTip_ID.Value,
        KullaniciTip = userType,
        Sifre = hasher.Hash(request.Sifre),
        Aktif = true,
        CreUser = 0,
        CreDate = DateTime.UtcNow
    };

    db.Kullanicilar.Add(user);
    await db.SaveChangesAsync(); // Önce kullanıcıyı kaydet ki ID'si oluşsun

    // Kullanıcı tipine göre öğrenci veya öğretmen kaydı oluştur
    var userTypeName = userType.Adi.ToLower();
    
    if (userTypeName.Contains("öğrenci") || userTypeName.Contains("ogrenci") || userTypeName.Contains("student"))
    {
        if (string.IsNullOrWhiteSpace(request.OgrenciNo))
        {
            // Kullanıcıyı geri sil
            db.Kullanicilar.Remove(user);
            await db.SaveChangesAsync();
            return Results.BadRequest("Öğrenci için öğrenci numarası gereklidir.");
        }

        // Öğrenci numarası kontrolü
        var ogrenciNoExists = await db.Ogrenciler.AnyAsync(x => x.OgrenciNo == request.OgrenciNo.Trim());
        if (ogrenciNoExists)
        {
            db.Kullanicilar.Remove(user);
            await db.SaveChangesAsync();
            return Results.Conflict("Bu öğrenci numarası zaten kayıtlı.");
        }

        // Sınıf seçilmemişse null bırak (öğrenci kayıt olduktan sonra sınıfa katılabilir)
        int? sinifId = null;
        if (request.Sinif_ID.HasValue && request.Sinif_ID.Value > 0)
        {
            sinifId = request.Sinif_ID.Value;
        }

        var ogrenci = new ItemOgrenci
        {
            Kullanici_ID = user.ID,
            OgrenciNo = request.OgrenciNo.Trim(),
            Sinif_ID = sinifId,
            CreUser = 0,
            CreDate = DateTime.UtcNow
        };

        db.Ogrenciler.Add(ogrenci);
    }
    else if (userTypeName.Contains("öğretmen") || userTypeName.Contains("ogretmen") || userTypeName.Contains("teacher"))
    {
        if (!request.Birim_ID.HasValue)
        {
            // Kullanıcıyı geri sil
            db.Kullanicilar.Remove(user);
            await db.SaveChangesAsync();
            return Results.BadRequest("Öğretmen için birim gereklidir.");
        }

        var ogretmen = new ItemOgretmen
        {
            Kullanici_ID = user.ID,
            Birim_ID = request.Birim_ID.Value,
            CreUser = 0,
            CreDate = DateTime.UtcNow
        };

        db.Ogretmenler.Add(ogretmen);
    }

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

// Dosya yükleme endpoint'i
var uploadGroup = app.MapGroup("/api/upload");
uploadGroup.MapPost("/", async (HttpRequest request) =>
{
    try
    {
        if (!request.HasFormContentType)
        {
            return Results.BadRequest("Form content type bekleniyor.");
        }

        // Request body size limitini kontrol et
        const long maxFileSize = 50 * 1024 * 1024; // 50MB
        
        var form = await request.ReadFormAsync();
        var file = form.Files.FirstOrDefault();

        if (file == null || file.Length == 0)
        {
            return Results.BadRequest("Dosya bulunamadı.");
        }

        // Dosya boyutu kontrolü
        if (file.Length > maxFileSize)
        {
            return Results.BadRequest($"Dosya boyutu çok büyük. Maksimum {maxFileSize / (1024 * 1024)}MB olmalıdır.");
        }

        // Uploads klasörünü oluştur
        var uploadsFolder = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "uploads");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        // Dosya adını benzersiz yap
        var fileName = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{file.FileName}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        // Dosyayı kaydet
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return Results.Ok(new { fileName = $"/uploads/{fileName}" });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Dosya yüklenirken hata oluştu: {ex.Message}");
    }
}).DisableAntiforgery();

// Seed Data - Admin kullanıcı tipi ve admin kullanıcısı
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EduTakipDBContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    
    // Admin kullanıcı tipini kontrol et ve ekle
    var adminTip = await db.KullaniciTipleri.FirstOrDefaultAsync(x => x.Adi.ToLower() == "admin");
    if (adminTip == null)
    {
        adminTip = new ItemKullaniciTip
        {
            Adi = "Admin",
            Aktif = true,
            CreUser = 0,
            CreDate = DateTime.UtcNow
        };
        db.KullaniciTipleri.Add(adminTip);
        await db.SaveChangesAsync();
    }
    
    // Admin kullanıcısını kontrol et ve ekle
    var adminUser = await db.Kullanicilar.FirstOrDefaultAsync(x => x.EPosta.ToLower() == "admin@edutakip.com");
    if (adminUser == null)
    {
        adminUser = new ItemKullanici
        {
            Adi = "Admin",
            Soyadi = "User",
            EPosta = "admin@edutakip.com",
            KullaniciTip_ID = adminTip.ID,
            KullaniciTip = adminTip,
            Sifre = hasher.Hash("Admin123!"), // Varsayılan şifre: Admin123!
            Aktif = true,
            CreUser = 0,
            CreDate = DateTime.UtcNow
        };
        db.Kullanicilar.Add(adminUser);
        await db.SaveChangesAsync();
    }
}

app.Run();
