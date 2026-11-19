using System.Net.Http.Json;
using System.Net.Http.Headers;
using Fcg.Web.DTOs.Auth;

namespace Fcg.Web.Services.Auth;

public sealed class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<TokenResponseDto> LoginAsync(
        string email,
        string senha,
        CancellationToken cancellationToken = default)
    {
        var request = new LoginRequestDto(email, senha);

        var response = await _httpClient.PostAsJsonAsync(
            "api/v1/auth/login",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<TokenResponseDto>(
            cancellationToken);

        return result
            ?? throw new InvalidOperationException(
                "A API não retornou os dados de autenticação.");
    }

    public async Task<TokenResponseDto> RefreshAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var request = new RefreshTokenRequestDto(refreshToken);

        var response = await _httpClient.PostAsJsonAsync(
            "api/v1/auth/refresh",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<TokenResponseDto>(
            cancellationToken);

        return result
            ?? throw new InvalidOperationException(
                "A API não retornou os dados de autenticação.");
    }

    public async Task LogoutAsync(
        string refreshToken,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new ArgumentException("Refresh Token inválido.", nameof(refreshToken));

        if (string.IsNullOrWhiteSpace(accessToken))
            throw new ArgumentException("Access Token inválido.", nameof(accessToken));

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "api/v1/auth/logout");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        request.Content = JsonContent.Create(
            new RefreshTokenRequestDto(refreshToken));

        using var response = await _httpClient.SendAsync(
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}
