namespace Domain.ShipIt;

public interface IShipItRepository
{
    Task<ShipItEntity> CreateShipIt(ShipItEntity shipItEntity);
}