using Fcg.Web.DTOs.Checkout;

namespace Fcg.Web.Services.Checkout;

public interface ICheckoutService
{
    Task<CompraAceitaResponseDto> AdquirirAsync(string jogoId,
    CancellationToken cancellationToken = default);

    Task<PedidoResponseDto> ObterPedidoAsync(string orderId,
    CancellationToken cancellationToken = default);
}
