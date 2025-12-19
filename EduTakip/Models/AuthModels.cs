using System.ComponentModel.DataAnnotations;

namespace EduTakip.Models
{
    public record RegisterRequest(
        [property: Required] string Adi,
        [property: Required] string Soyadi,
        [property: Required, EmailAddress] string EPosta,
        [property: Required, MinLength(6)] string Sifre,
        [property: Required] int KullaniciTip_ID);

    public record LoginRequest(
        [property: Required, EmailAddress] string EPosta,
        [property: Required] string Sifre);

    public record AuthUserDto(int ID, string Adi, string Soyadi, string EPosta, string KullaniciTip);

    public record AuthResponse(string Token, AuthUserDto Kullanici);
}

