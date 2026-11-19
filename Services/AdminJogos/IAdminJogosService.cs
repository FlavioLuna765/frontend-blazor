using Fcg.Web.DTOs.Catalogo;

namespace Fcg.Web.Services.AdminJogos;

public interface IAdminJogosService
{
    Task<JogoResponseDto> CriarAsync(
        CriarJogoRequestDto dto,
        CancellationToken cancellationToken = default);

    Task<JogoResponseDto> AtualizarAsync(
        string id,
        AtualizarJogoRequestDto dto,
        CancellationToken cancellationToken = default);

    Task DesativarAsync(
        string id,
        CancellationToken cancellationToken = default);
}
