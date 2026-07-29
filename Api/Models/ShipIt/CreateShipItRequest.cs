namespace Api.Models.ShipIt;

public sealed record CreateShipItRequest(
	string Description,
	ShipItPersonRequest PersonA,
	ShipItPersonRequest PersonB,
	string? Language = null);

public sealed record ShipItPersonRequest(string Name, string Email, string[] Tags);