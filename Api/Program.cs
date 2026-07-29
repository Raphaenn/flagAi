using Api.Endpoints.CoupleAi;
using Api.Endpoints.ShipIt;
using Api.Extensions;
using Api.Middlewares;
using Infra;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddAppInjections(builder.Configuration);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UsePostBearerTokenGuard();

app.MapCreateCoupleAi();
app.MapCreateShipIt();

app.Run();