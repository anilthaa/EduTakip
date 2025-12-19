using System.Net.Http.Headers;
using System.Net.Http.Json;
using EduTakip.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;
using System.Text.Json;

namespace EduTakip.Services
{
    public class AuthStateService : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;
        private AuthUserDto? _currentUser;
        private string? _token;

        public AuthStateService(HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
        }

        public AuthUserDto? CurrentUser => _currentUser;
        public bool IsAuthenticated => _currentUser != null && !string.IsNullOrEmpty(_token);

        public async Task InitializeAsync()
        {
            _token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
            var userJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authUser");

            if (!string.IsNullOrEmpty(_token) && !string.IsNullOrEmpty(userJson))
            {
                try
                {
                    _currentUser = JsonSerializer.Deserialize<AuthUserDto>(userJson);
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                    NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                }
                catch
                {
                    await LogoutAsync();
                }
            }
        }

        public async Task<bool> LoginAsync(string token, AuthUserDto user)
        {
            try
            {
                _token = token;
                _currentUser = user;

                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authUser", JsonSerializer.Serialize(user));

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task LogoutAsync()
        {
            _token = null;
            _currentUser = null;

            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authUser");

            _httpClient.DefaultRequestHeaders.Authorization = null;
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (_currentUser == null || string.IsNullOrEmpty(_token))
            {
                return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, _currentUser.ID.ToString()),
                new Claim(ClaimTypes.Email, _currentUser.EPosta),
                new Claim(ClaimTypes.Name, $"{_currentUser.Adi} {_currentUser.Soyadi}"),
                new Claim(ClaimTypes.Role, _currentUser.KullaniciTip)
            };

            var identity = new ClaimsIdentity(claims, "jwt");
            var principal = new ClaimsPrincipal(identity);

            return Task.FromResult(new AuthenticationState(principal));
        }
    }
}

