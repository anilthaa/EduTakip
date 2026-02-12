using System.ComponentModel.DataAnnotations;

namespace EduTakip.Models
{
    public record RegisterRequest(
        [property: Required] string Adi,
        [property: Required] string Soyadi,
        [property: Required, EmailAddress] string EPosta,
        [property: Required, MinLength(6)] string Sifre,
        int? KullaniciTip_ID = null,
        int? Sinif_ID = null,
        string OgrenciNo = null,
        int? Birim_ID = null);

    public record LoginRequest(
        [property: Required, EmailAddress] string EPosta,
        [property: Required] string Sifre);

    public record AuthUserDto(int ID, string Adi, string Soyadi, string EPosta, string KullaniciTip);

    public record AuthResponse(string Token, AuthUserDto Kullanici);

    public record ForgotPasswordRequest(
        [property: Required, EmailAddress] string EPosta);

    public record ResetPasswordRequest(
        [property: Required] string Token,
        [property: Required, MinLength(6)] string YeniSifre);

    public record SendConfirmationCodeRequest(
        [property: Required, EmailAddress] string Email);

    public record VerifyConfirmationCodeRequest(
        [property: Required, EmailAddress] string Email,
        [property: Required] string Code);

    public record SendActivationLinkRequest(
        [property: Required, EmailAddress] string Email);

    public record ActivateAccountWithPasswordRequest(
        [property: Required, EmailAddress] string Email);

    public record ChangePasswordRequest(
        [property: Required] string MevcutSifre,
        [property: Required, MinLength(6)] string YeniSifre);

    public record UpdateProfileRequest(
        [property: Required] string Adi,
        [property: Required] string Soyadi);
}

