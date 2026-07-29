using App.Dto.CoupleAi;

namespace App.Interfaces.CoupleAi;

public interface ICoupleAiService
{
    Task<CoupleMessageResponseDto> ChatWithAi(CoupleMessageRequestDto messageData, CancellationToken ct);
}