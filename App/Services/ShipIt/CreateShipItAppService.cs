// using App.Dto;
// using App.Dtos;
// using Domain.Entities;
// using Domain.Interfaces;
//
// namespace App.Services.ShipIt;
//
// public class CreateShipItAppService
// {
//     private readonly IShipItRepository _shipItRepository;
//
//     public CreateShipItAppService(IShipItRepository shipItRepository)
//     {
//         this._shipItRepository = shipItRepository;
//     }
//
//     public async Task<ShipItDto> Create(Guid ownerId, string targetName1, string targetName2, string targetEmail1, string targetEmail2, CancellationToken ct)
//     {
//         ShipItEntity shipItEntity = ShipItEntity.CreateOne(ownerId, targetName1, targetName2, targetEmail1, targetEmail2);
//         ShipItEntity res = await _shipItRepository.CreateShipIt(shipItEntity);
//
//         ShipItDto dto = new ShipItDto
//         {
//             Id = res.Id,
//             OwnerId = res.OwnerId,
//             TargetName1 = res.TargetName1,
//             TargetName2 = res.TargetName2,
//             TargetEmail1 = res.TargetEmail1,
//             TargetEmail2 = res.TargetEmail2,
//             Status = res.Status,
//             Summary = res.Summary,
//             Score = res.Score,
//             InvitedAt = res.InvitedAt
//         };
//
//         return dto;
//     }
// }