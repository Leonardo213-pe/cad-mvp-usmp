using Cad.Contratos.Salud;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Liveness: no toca la base de datos para no despertar Neon (ADR-05).
app.MapGet("/salud", () => Results.Ok(new EstadoSaludDto("ok", DateTimeOffset.UtcNow)));

app.Run();

// Expuesto para WebApplicationFactory en Cad.Server.Tests.
public partial class Program;
