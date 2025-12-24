using System.Net.Http.Json;
using EduTakip.Models;
using Microsoft.AspNetCore.Components;

namespace EduTakip.Services
{
    public interface IAuthService
    {
        Task<AuthResponse?> LoginAsync(LoginRequest request);
        Task<AuthUserDto?> RegisterAsync(RegisterRequest request);
        Task<List<UserTypeDto>?> GetUserTypesAsync();
    }

    public record UserTypeDto(int ID, string Adi);

    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;

        public AuthService(HttpClient httpClient, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _navigationManager = navigationManager;
            _httpClient.BaseAddress = new Uri(_navigationManager.BaseUri);
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/auth/login", request);
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<AuthResponse>();
                }
                
                // Hata durumunda response body'yi oku
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Giriş başarısız: {errorMessage}");
            }
            catch (HttpRequestException)
            {
                throw; // HttpRequestException'ı tekrar fırlat
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"Giriş sırasında bir hata oluştu: {ex.Message}", ex);
            }
        }

        public async Task<AuthUserDto?> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/auth/register", request);
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<AuthUserDto>();
                }
                
                // Hata durumunda response body'yi oku
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Kayıt başarısız: {errorMessage}");
            }
            catch (HttpRequestException)
            {
                throw; // HttpRequestException'ı tekrar fırlat
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"Kayıt sırasında bir hata oluştu: {ex.Message}", ex);
            }
        }

        public async Task<List<UserTypeDto>?> GetUserTypesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/auth/user-types");
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<UserTypeDto>>();
                }
                
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}

