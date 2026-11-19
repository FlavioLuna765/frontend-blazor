using System.Net.Http.Json;
using Fcg.Web.DTOs.Catalogo;

namespace Fcg.Web.Services.AdminJogos;

public sealed class AdminJogosService : IAdminJogosService
{
    private readonly HttpClient _httpClient;

    public AdminJogosService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<JogoResponseDto> CriarAsync(
        CriarJogoRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        using var response = await _httpClient.PostAsJsonAsync(
            "api/v1/jogos",
            dto,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<JogoResponseDto>(
                   cancellationToken)
               ?? throw new InvalidOperationException(
                   "A API não retornou o jogo criado.");
    }

    public async Task<JogoResponseDto> AtualizarAsync(
        string id,
        AtualizarJogoRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(dto);

        using var response = await _httpClient.PutAsJsonAsync(
            $"api/v1/jogos/{Uri.EscapeDataString(id)}",
            dto,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<JogoResponseDto>(
                   cancellationToken)
               ?? throw new InvalidOperationException(
                   "A API não retornou o jogo atualizado.");
    }

    public async Task DesativarAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using var response = await _httpClient.DeleteAsync(
            $"api/v1/jogos/{Uri.EscapeDataString(id)}",
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}
