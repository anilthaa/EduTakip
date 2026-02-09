using System.Net;
using System.Net.Mail;

namespace EduTakip.Services
{
    public interface IEmailService
    {
        Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetToken, string resetUrl);
        Task<bool> SendAccountConfirmationEmailAsync(string toEmail, string confirmationToken, string confirmationUrl);
        Task<bool> SendConfirmationCodeEmailAsync(string toEmail, string confirmationCode);
        Task<bool> SendAccountActivationEmailAsync(string toEmail, string temporaryPassword, string activationUrl);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetToken, string resetUrl)
        {
            try
            {
                var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = _configuration.GetValue<int>("Email:SmtpPort", 587);
                var smtpUsername = _configuration["Email:SmtpUsername"];
                var smtpPassword = _configuration["Email:SmtpPassword"];
                var fromEmail = _configuration["Email:FromEmail"] ?? smtpUsername;
                var fromName = _configuration["Email:FromName"] ?? "EduTakip";

                // Eğer SMTP ayarları yoksa, log'a yaz ve false döndür
                if (string.IsNullOrWhiteSpace(smtpUsername) || string.IsNullOrWhiteSpace(smtpPassword))
                {
                    _logger.LogWarning("SMTP ayarları yapılandırılmamış. E-posta gönderilemedi. Email:SmtpUsername ve Email:SmtpPassword appsettings.json'da ayarlanmalı.");
                    return false;
                }

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(smtpUsername, smtpPassword)
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = "Şifre Sıfırlama - EduTakip",
                    Body = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <h2>Şifre Sıfırlama İsteği</h2>
                            <p>Merhaba,</p>
                            <p>EduTakip hesabınız için şifre sıfırlama talebinde bulundunuz.</p>
                            <p>Şifrenizi sıfırlamak için aşağıdaki linke tıklayın:</p>
                            <p><a href='{resetUrl}' style='background-color: #667eea; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block;'>Şifremi Sıfırla</a></p>
                            <p>Veya aşağıdaki linki tarayıcınıza yapıştırın:</p
                            <p><strong>Bu link 1 saat geçerlidir.</strong></p>
                            <p>Eğer bu talebi siz yapmadıysanız, bu e-postayı görmezden gelebilirsiniz.</p>
                            <p>Saygılarımızla,<br/>EduTakip Ekibi</p>
                        </body>
                        </html>",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"Şifre sıfırlama e-postası gönderildi: {toEmail}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"E-posta gönderilirken hata oluştu: {toEmail}");
                return false;
            }
        }

        public async Task<bool> SendAccountConfirmationEmailAsync(string toEmail, string confirmationToken, string confirmationUrl)
        {
            try
            {
                var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = _configuration.GetValue<int>("Email:SmtpPort", 587);
                var smtpUsername = _configuration["Email:SmtpUsername"];
                var smtpPassword = _configuration["Email:SmtpPassword"];
                var fromEmail = _configuration["Email:FromEmail"] ?? smtpUsername;
                var fromName = _configuration["Email:FromName"] ?? "EduTakip";

                // Eğer SMTP ayarları yoksa, log'a yaz ve false döndür
                if (string.IsNullOrWhiteSpace(smtpUsername) || string.IsNullOrWhiteSpace(smtpPassword))
                {
                    _logger.LogWarning("SMTP ayarları yapılandırılmamış. E-posta gönderilemedi. Email:SmtpUsername ve Email:SmtpPassword appsettings.json'da ayarlanmalı.");
                    return false;
                }

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(smtpUsername, smtpPassword)
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = "Hesap Onayı - EduTakip",
                    Body = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <h2>Hesap Onayı</h2>
                            <p>Merhaba,</p>
                            <p>EduTakip sistemine kayıt olduğunuz için teşekkür ederiz.</p>
                            <p>Hesabınızı aktifleştirmek için aşağıdaki linke tıklayın:</p>
                            <p><a href='{confirmationUrl}' style='background-color: #667eea; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block;'>Hesabımı Onayla</a></p>
                            <p>Veya aşağıdaki linki tarayıcınıza yapıştırın:</p>
                            <p><a href='{confirmationUrl}'>{confirmationUrl}</a></p>
                            <p><strong>Bu link 24 saat geçerlidir.</strong></p>
                            <p>Hesabınız onaylandıktan sonra giriş yapabilirsiniz. Ayrıca hesabınızın admin tarafından onaylanması gerekmektedir.</p>
                            <p>Saygılarımızla,<br/>EduTakip Ekibi</p>
                        </body>
                        </html>",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"Hesap onay e-postası gönderildi: {toEmail}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"E-posta gönderilirken hata oluştu: {toEmail}");
                return false;
            }
        }

        public async Task<bool> SendConfirmationCodeEmailAsync(string toEmail, string confirmationCode)
        {
            try
            {
                var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = _configuration.GetValue<int>("Email:SmtpPort", 587);
                var smtpUsername = _configuration["Email:SmtpUsername"];
                var smtpPassword = _configuration["Email:SmtpPassword"];
                var fromEmail = _configuration["Email:FromEmail"] ?? smtpUsername;
                var fromName = _configuration["Email:FromName"] ?? "EduTakip";

                // Eğer SMTP ayarları yoksa, log'a yaz ve false döndür
                if (string.IsNullOrWhiteSpace(smtpUsername) || string.IsNullOrWhiteSpace(smtpPassword))
                {
                    _logger.LogWarning("SMTP ayarları yapılandırılmamış. E-posta gönderilemedi. Email:SmtpUsername ve Email:SmtpPassword appsettings.json'da ayarlanmalı.");
                    return false;
                }

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(smtpUsername, smtpPassword)
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = "E-posta Doğrulama Kodu - EduTakip",
                    Body = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <h2>E-posta Doğrulama Kodu</h2>
                            <p>Merhaba,</p>
                            <p>EduTakip sistemine kayıt olduğunuz için teşekkür ederiz.</p>
                            <p>Hesabınızı aktifleştirmek için aşağıdaki doğrulama kodunu kullanın:</p>
                            <div style='background-color: #f0f0f0; padding: 20px; text-align: center; border-radius: 5px; margin: 20px 0;'>
                                <h1 style='color: #667eea; font-size: 32px; margin: 0; letter-spacing: 5px;'>{confirmationCode}</h1>
                            </div>
                            <p><strong>Bu kod 10 dakika geçerlidir.</strong></p>
                            <p>Hesabınız onaylandıktan sonra giriş yapabilirsiniz. Ayrıca hesabınızın admin tarafından onaylanması gerekmektedir.</p>
                            <p>Eğer bu talebi siz yapmadıysanız, bu e-postayı görmezden gelebilirsiniz.</p>
                            <p>Saygılarımızla,<br/>EduTakip Ekibi</p>
                        </body>
                        </html>",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"Doğrulama kodu e-postası gönderildi: {toEmail}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"E-posta gönderilirken hata oluştu: {toEmail}");
                return false;
            }
        }

        public async Task<bool> SendAccountActivationEmailAsync(string toEmail, string temporaryPassword, string activationUrl)
        {
            try
            {
                var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = _configuration.GetValue<int>("Email:SmtpPort", 587);
                var smtpUsername = _configuration["Email:SmtpUsername"];
                var smtpPassword = _configuration["Email:SmtpPassword"];
                var fromEmail = _configuration["Email:FromEmail"] ?? smtpUsername;
                var fromName = _configuration["Email:FromName"] ?? "EduTakip";

                // Eğer SMTP ayarları yoksa, log'a yaz ve false döndür
                if (string.IsNullOrWhiteSpace(smtpUsername) || string.IsNullOrWhiteSpace(smtpPassword))
                {
                    _logger.LogWarning("SMTP ayarları yapılandırılmamış. E-posta gönderilemedi. Email:SmtpUsername ve Email:SmtpPassword appsettings.json'da ayarlanmalı.");
                    return false;
                }

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(smtpUsername, smtpPassword)
                };

                string emailBody;
                string subject;

                if (!string.IsNullOrWhiteSpace(temporaryPassword))
                {
                    // Geçici şifre ile aktifleştirme
                    subject = "Hesap Aktifleştirme - Geçici Şifre - EduTakip";
                    emailBody = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <h2>Hesabınız Aktifleştirildi</h2>
                            <p>Merhaba,</p>
                            <p>EduTakip sistemindeki hesabınız admin tarafından aktifleştirilmiştir.</p>
                            <p>Aşağıdaki geçici şifre ile giriş yapabilirsiniz:</p>
                            <div style='background-color: #f0f0f0; padding: 20px; text-align: center; border-radius: 5px; margin: 20px 0;'>
                                <h1 style='color: #667eea; font-size: 24px; margin: 0; letter-spacing: 3px;'>{temporaryPassword}</h1>
                            </div>
                            <p><strong>Güvenliğiniz için lütfen ilk girişinizde şifrenizi değiştirin.</strong></p>
                            <p>Giriş yapmak için aşağıdaki linke tıklayın:</p>
                            <p><a href='{activationUrl}' style='background-color: #667eea; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block;'>Giriş Yap</a></p>
                            <p>Veya aşağıdaki linki tarayıcınıza yapıştırın:</p>
                            <p><a href='{activationUrl}'>{activationUrl}</a></p>
                            <p>Saygılarımızla,<br/>EduTakip Ekibi</p>
                        </body>
                        </html>";
                }
                else
                {
                    // Sadece aktifleştirme linki
                    subject = "Hesap Aktifleştirme - EduTakip";
                    emailBody = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <h2>Hesap Aktifleştirme</h2>
                            <p>Merhaba,</p>
                            <p>EduTakip sistemindeki hesabınızı aktifleştirmek için aşağıdaki linke tıklayın:</p>
                            <p><a href='{activationUrl}' style='background-color: #667eea; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block;'>Hesabımı Aktifleştir</a></p>
                            <p>Veya aşağıdaki linki tarayıcınıza yapıştırın:</p>
                            <p><a href='{activationUrl}'>{activationUrl}</a></p>
                            <p><strong>Bu link 7 gün geçerlidir.</strong></p>
                            <p>Saygılarımızla,<br/>EduTakip Ekibi</p>
                        </body>
                        </html>";
                }

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = emailBody,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"Hesap aktifleştirme e-postası gönderildi: {toEmail}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"E-posta gönderilirken hata oluştu: {toEmail}");
                return false;
            }
        }
    }
}
