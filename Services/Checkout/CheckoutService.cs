using System.Net.Http.Json;
using Fcg.Web.DTOs.Checkout;

namespace Fcg.Web.Services.Checkout;

public sealed class CheckoutService : ICheckoutService
{
    private readonly HttpClient _httpClient;

    public CheckoutService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CompraAceitaResponseDto> AdquirirAsync(
        string jogoId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(jogoId))
            throw new ArgumentException(
                "O identificador do jogo é obrigatório.",
                nameof(jogoId));

        var request = new AdquirirJogoRequestDto(jogoId);

        using var response = await _httpClient.PostAsJsonAsync(
            "api/v1/biblioteca",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var resultado = await response.Content
            .ReadFromJsonAsync<CompraAceitaResponseDto>(
                cancellationToken: cancellationToken);

        return resultado ?? throw new InvalidOperationException(
            "A API não retornou os dados do pedido.");
    }

    public async Task<PedidoResponseDto> ObterPedidoAsync(
        string orderId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            throw new ArgumentException(
                "O identificador do pedido é obrigatório.",
                nameof(orderId));

        var id = Uri.EscapeDataString(orderId);

        var pedido = await _httpClient.GetFromJsonAsync<PedidoResponseDto>(
            $"api/v1/pedidos/{id}",
            cancellationToken);

        return pedido ?? throw new InvalidOperationException(
            "A API não retornou os dados do pedido.");
    }
}
