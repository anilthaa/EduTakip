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
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHttpContextAccessor();

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

// Token hashleme helper metodu (local function)
string HashToken(string token)
{
    using var sha256 = System.Security.Cryptography.SHA256.Create();
    var tokenBytes = System.Text.Encoding.UTF8.GetBytes(token);
    var hashedTokenBytes = sha256.ComputeHash(tokenBytes);
    return Convert.ToHexString(hashedTokenBytes);
}

authGroup.MapPost("/register", async (RegisterRequest request, EduTakipDBContext db, IPasswordHasher hasher, IEmailService emailService, IHttpContextAccessor httpContextAccessor) =>
{
    // Temel alan kontrolleri
    if (string.IsNullOrWhiteSpace(request.Adi))
    {
        return Results.BadRequest("Ad gereklidir.");
    }

    if (string.IsNullOrWhiteSpace(request.Soyadi))
    {
        return Results.BadRequest("Soyad gereklidir.");
    }

    if (string.IsNullOrWhiteSpace(request.EPosta))
    {
        return Results.BadRequest("E-posta gereklidir.");
    }

    if (string.IsNullOrWhiteSpace(request.Sifre))
    {
        return Results.BadRequest("Şifre gereklidir.");
    }

    // Ad ve soyad validasyonu (en az 2 karakter, sadece harf ve boşluk)
    var nameRegex = new System.Text.RegularExpressions.Regex(@"^[a-zA-ZçğıöşüÇĞIİÖŞÜ\s\-\.]{2,50}$");
    if (!nameRegex.IsMatch(request.Adi.Trim()))
    {
        return Results.BadRequest("Ad en az 2 karakter olmalı ve sadece harf içermelidir.");
    }

    if (!nameRegex.IsMatch(request.Soyadi.Trim()))
    {
        return Results.BadRequest("Soyad en az 2 karakter olmalı ve sadece harf içermelidir.");
    }

    // E-posta format kontrolü
    var emailRegex = new System.Text.RegularExpressions.Regex(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    
    var email = request.EPosta.Trim().ToLowerInvariant();
    if (!emailRegex.IsMatch(email))
    {
        return Results.BadRequest("Geçerli bir e-posta adresi giriniz.");
    }

    // E-posta uzunluk kontrolü
    if (email.Length > 100)
    {
        return Results.BadRequest("E-posta adresi çok uzun.");
    }

    // Şifre validasyonu
    if (request.Sifre.Length < 6)
    {
        return Results.BadRequest("Şifre en az 6 karakter olmalıdır.");
    }

    if (request.Sifre.Length > 100)
    {
        return Results.BadRequest("Şifre en fazla 100 karakter olabilir.");
    }

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

    // E-posta tekrar kontrolü
    var exists = await db.Kullanicilar.AnyAsync(x => x.EPosta == email);
    if (exists)
    {
        return Results.Conflict("E-posta zaten kayıtlı.");
    }

    // E-posta onay token'ı oluştur
    var confirmationToken = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32))
        .Replace("+", "-")
        .Replace("/", "_")
        .Replace("=", "");

    // Token'ı hashle (SHA256) - veritabanında hash'lenmiş olarak sakla
    var hashedConfirmationToken = HashToken(confirmationToken);
    
    // E-postaya gönderilecek token (hash'lenmemiş, URL-safe)
    var urlSafeToken = confirmationToken;

    var user = new ItemKullanici
    {
        Adi = request.Adi.Trim(),
        Soyadi = request.Soyadi.Trim(),
        EPosta = email,
        KullaniciTip_ID = request.KullaniciTip_ID.Value,
        KullaniciTip = userType,
        Sifre = hasher.Hash(request.Sifre),
        Aktif = false, // Admin onayı bekliyor
        EmailConfirmed = false,
        EmailConfirmationToken = hashedConfirmationToken,
        EmailConfirmationTokenExpiry = DateTime.UtcNow.AddHours(24),
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

        // Öğrenci numarası format kontrolü
        var ogrenciNoRegex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9\-]{3,20}$");
        var ogrenciNoTrimmed = request.OgrenciNo.Trim();
        if (!ogrenciNoRegex.IsMatch(ogrenciNoTrimmed))
        {
            db.Kullanicilar.Remove(user);
            await db.SaveChangesAsync();
            return Results.BadRequest("Öğrenci numarası 3-20 karakter arası olmalı ve sadece harf, rakam ve tire içermelidir.");
        }

        // Öğrenci numarası tekrar kontrolü
        var ogrenciNoExists = await db.Ogrenciler.AnyAsync(x => x.OgrenciNo == ogrenciNoTrimmed);
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

    // E-posta onay mesajı gönder - token query string olarak gönderilecek
    var baseUrl = $"{httpContextAccessor.HttpContext?.Request.Scheme}://{httpContextAccessor.HttpContext?.Request.Host}";
    var confirmationUrl = $"{baseUrl}/confirm-email?token={Uri.EscapeDataString(urlSafeToken)}";
    
    var emailSent = await emailService.SendAccountConfirmationEmailAsync(email, urlSafeToken, confirmationUrl);
    
    if (!emailSent)
    {
        // E-posta gönderilemediyse log'a yaz ama kullanıcıyı silme (kullanıcı admin onayı bekleyebilir)
        // Logger kullanılamıyor, bu yüzden sadece devam ediyoruz
    }

    var dto = new AuthUserDto(user.ID, user.Adi, user.Soyadi, user.EPosta, user.KullaniciTip.Adi);
    return Results.Created($"/api/auth/users/{user.ID}", new { message = "Kayıt başarılı. Lütfen e-postanızı kontrol edin ve hesabınızı onaylayın. Ayrıca hesabınızın admin tarafından onaylanması gerekmektedir.", user = dto });
});

authGroup.MapPost("/login", async (LoginRequest request, EduTakipDBContext db, IPasswordHasher hasher, IJwtService jwtService) =>
{
    var email = request.EPosta.Trim().ToLowerInvariant();
    var user = await db.Kullanicilar
        .Include(x => x.KullaniciTip)
        .FirstOrDefaultAsync(x => x.EPosta == email && x.DelDate == null);

    if (user is null || !hasher.Verify(user.Sifre, request.Sifre))
    {
        return Results.BadRequest("E-posta veya şifre hatalı.");
    }

    // E-posta onayı kontrolü
    if (!user.EmailConfirmed)
    {
        return Results.BadRequest("Hesabınız henüz onaylanmamış. Lütfen e-postanızı kontrol edin ve hesabınızı onaylayın.");
    }

    // Reddedilme kontrolü
    if (user.Reddedildi)
    {
        return Results.BadRequest("Hesabınız reddedilmiştir. Giriş yapamazsınız.");
    }

    // Admin onayı kontrolü - EmailConfirmed = true ama Aktif = false ise admin onayı bekliyor
    if (!user.Aktif)
    {
        return Results.BadRequest("Hesabınız admin onayı bekliyor. Lütfen admin onayından sonra tekrar deneyin.");
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

// Şifremi unuttum
authGroup.MapPost("/forgot-password", async (ForgotPasswordRequest request, EduTakipDBContext db, IEmailService emailService, IHttpContextAccessor httpContextAccessor) =>
{
    // E-posta validasyonu
    if (string.IsNullOrWhiteSpace(request.EPosta))
    {
        return Results.BadRequest("E-posta gereklidir.");
    }

    var emailRegex = new System.Text.RegularExpressions.Regex(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    
    var email = request.EPosta.Trim().ToLowerInvariant();
    if (!emailRegex.IsMatch(email))
    {
        return Results.BadRequest("Geçerli bir e-posta adresi giriniz.");
    }

    // Kullanıcıyı bul
    var user = await db.Kullanicilar
        .FirstOrDefaultAsync(x => x.EPosta == email && x.Aktif && x.DelDate == null);

    // Güvenlik: Kullanıcı bulunmasa bile başarılı mesajı döndür (email enumeration saldırısını önlemek için)
    if (user == null)
    {
        // Gerçek uygulamada burada log tutulabilir
        return Results.Ok(new { message = "Eğer bu e-posta adresi kayıtlıysa, şifre sıfırlama linki gönderildi." });
    }

    // Token oluştur
    var token = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32))
        .Replace("+", "-")
        .Replace("/", "_")
        .Replace("=", "");

    // Token'ı hashle (SHA256) - güvenlik için
    var hashedToken = HashToken(token);
    
    // Hash'lenmiş token'ı URL-safe hale getir (Base64 karakterlerini URL-safe yap)
    var urlSafeHashedToken = hashedToken
        .Replace("+", "-")
        .Replace("/", "_")
        .Replace("=", "");

    // Hash'lenmiş token'ı veritabanına kaydet (1 saat geçerli)
    user.SifreSifirlamaToken = hashedToken;
    user.SifreSifirlamaTokenExpiry = DateTime.UtcNow.AddHours(1);
    db.Kullanicilar.Update(user);
    await db.SaveChangesAsync();

    // E-posta gönder - URL'de hash'lenmiş token kullan
    var baseUrl = $"{httpContextAccessor.HttpContext?.Request.Scheme}://{httpContextAccessor.HttpContext?.Request.Host}";
    var resetUrl = $"{baseUrl}/reset-password/{urlSafeHashedToken}";
    
    var emailSent = await emailService.SendPasswordResetEmailAsync(email, urlSafeHashedToken, resetUrl);

    if (!emailSent)
    {
        // E-posta gönderilemediyse token'ı temizle
        user.SifreSifirlamaToken = null;
        user.SifreSifirlamaTokenExpiry = null;
        db.Kullanicilar.Update(user);
        await db.SaveChangesAsync();
        
        return Results.BadRequest("E-posta gönderilemedi. Lütfen daha sonra tekrar deneyin.");
    }

    return Results.Ok(new { message = "Şifre sıfırlama linki e-posta adresinize gönderildi." });
});

// Şifre sıfırla
authGroup.MapPost("/reset-password", async (ResetPasswordRequest request, EduTakipDBContext db, IPasswordHasher hasher) =>
{
    // Token ve şifre validasyonu
    if (string.IsNullOrWhiteSpace(request.Token))
    {
        return Results.BadRequest("Token gereklidir.");
    }

    if (string.IsNullOrWhiteSpace(request.YeniSifre))
    {
        return Results.BadRequest("Yeni şifre gereklidir.");
    }

    if (request.YeniSifre.Length < 6)
    {
        return Results.BadRequest("Şifre en az 6 karakter olmalıdır.");
    }

    if (request.YeniSifre.Length > 100)
    {
        return Results.BadRequest("Şifre en fazla 100 karakter olabilir.");
    }

    // Gelen token URL-safe formatında olduğu için Base64 formatına geri çevir
    var tokenFromUrl = request.Token
        .Replace("-", "+")
        .Replace("_", "/");
    
    // Padding ekle (Base64 için gerekli)
    switch (tokenFromUrl.Length % 4)
    {
        case 2: tokenFromUrl += "=="; break;
        case 3: tokenFromUrl += "="; break;
    }

    // Token zaten hash'lenmiş olarak geldiği için direkt kullan
    var hashedToken = tokenFromUrl;

    // Kullanıcıyı hash'lenmiş token ile bul
    var user = await db.Kullanicilar
        .FirstOrDefaultAsync(x => x.SifreSifirlamaToken == hashedToken && 
                                  x.SifreSifirlamaTokenExpiry.HasValue &&
                                  x.SifreSifirlamaTokenExpiry.Value > DateTime.UtcNow &&
                                  x.Aktif &&
                                  x.DelDate == null);

    if (user == null)
    {
        return Results.BadRequest("Geçersiz veya süresi dolmuş token.");
    }

    // Şifreyi güncelle
    user.Sifre = hasher.Hash(request.YeniSifre);
    user.SifreSifirlamaToken = null;
    user.SifreSifirlamaTokenExpiry = null;
    user.UpdDate = DateTime.UtcNow;
    user.UpdUser = user.ID;
    
    db.Kullanicilar.Update(user);
    await db.SaveChangesAsync();

    return Results.Ok(new { message = "Şifre başarıyla sıfırlandı." });
});

// E-posta onayla
authGroup.MapGet("/confirm-email", async (string? token, EduTakipDBContext db) =>
{
    if (string.IsNullOrWhiteSpace(token))
    {
        return Results.BadRequest("Token gereklidir.");
    }

    // Gelen token hash'lenmemiş, veritabanında hash'lenmiş olarak saklanıyor
    // Token'ı hash'leyip veritabanındaki hash'lenmiş token ile karşılaştır
    var hashedToken = HashToken(token);
    
    var user = await db.Kullanicilar
        .FirstOrDefaultAsync(x => x.EmailConfirmationToken == hashedToken 
            && x.EmailConfirmationTokenExpiry.HasValue 
            && x.EmailConfirmationTokenExpiry.Value > DateTime.UtcNow);

    if (user == null)
    {
        return Results.BadRequest("Geçersiz veya süresi dolmuş token.");
    }

    // E-posta onayını tamamla
    try
    {
        user.EmailConfirmed = true;
        user.EmailConfirmationToken = null;
        user.EmailConfirmationTokenExpiry = null;
        db.Kullanicilar.Update(user);
        await db.SaveChangesAsync();

        return Results.Redirect("/email-confirmed");
    }
    catch (Exception ex)
    {
        // Veritabanı güncellemesi başarısız olsa bile, eğer EmailConfirmed zaten true ise başarılı say
        if (user.EmailConfirmed)
        {
            return Results.Ok(new { message = "E-posta zaten onaylanmış. Hesabınızın admin tarafından onaylanması gerekmektedir." });
        }
        return Results.BadRequest($"E-posta onaylanırken bir hata oluştu: {ex.Message}");
    }
});

// Admin hesap onayı
authGroup.MapPost("/approve-account/{userId}", async (int userId, EduTakipDBContext db, HttpContext httpContext) =>
{
    // Admin kontrolü
    var user = httpContext.User;
    var isAdmin = user.IsInRole("Admin");
    if (!isAdmin)
    {
        return Results.Forbid();
    }

    var targetUser = await db.Kullanicilar
        .FirstOrDefaultAsync(x => x.ID == userId && x.DelDate == null);

    if (targetUser == null)
    {
        return Results.NotFound("Kullanıcı bulunamadı.");
    }

    // E-posta onayı kontrolü
    if (!targetUser.EmailConfirmed)
    {
        return Results.BadRequest("Kullanıcının önce e-postasını onaylaması gerekmektedir.");
    }

    // Hesap zaten aktifse
    if (targetUser.Aktif)
    {
        return Results.Ok(new { message = "Hesap zaten aktif." });
    }

    // Hesabı aktifleştir
    targetUser.Aktif = true;
    targetUser.UpdDate = DateTime.UtcNow;
    targetUser.UpdUser = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    
    db.Kullanicilar.Update(targetUser);
    await db.SaveChangesAsync();

    return Results.Ok(new { message = "Hesap başarıyla onaylandı." });
}).RequireAuthorization();

// Onay bekleyen hesapları getir (Admin için)
authGroup.MapGet("/pending-accounts", async (EduTakipDBContext db, HttpContext httpContext) =>
{
    // Admin kontrolü
    var user = httpContext.User;
    var isAdmin = user.IsInRole("Admin");
    if (!isAdmin)
    {
        return Results.Forbid();
    }

    var pendingAccounts = await db.Kullanicilar
        .Include(x => x.KullaniciTip)
        .Where(x => x.DelDate == null 
            && x.EmailConfirmed 
            && !x.Aktif)
        .Select(x => new
        {
            x.ID,
            x.Adi,
            x.Soyadi,
            x.EPosta,
            KullaniciTip = x.KullaniciTip.Adi,
            x.CreDate
        })
        .ToListAsync();

    return Results.Ok(pendingAccounts);
}).RequireAuthorization();

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
