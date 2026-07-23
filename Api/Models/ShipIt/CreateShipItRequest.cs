namespace Api.Models.ShipIt;

public sealed record CreateShipItRequest(
	string Description,
	ShipItPersonRequest PersonA,
	ShipItPersonRequest PersonB);

public sealed record ShipItPersonRequest(string Name, string Email);