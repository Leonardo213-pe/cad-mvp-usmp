using Cad.Contratos.Salud;
using Cad.Server.Compartido.Seguridad;
using Cad.Server.Modulos.Cuentas;
using Cad.Server.Modulos.Cuentas.Endpoints;
using Cad.Server.Persistencia;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddPersistenciaCad(builder.Configuration);
builder.Services.AddSeguridadCad(builder.Configuration);
builder.Services.AddModuloCuentas(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

if (app.Configuration.GetValue("BaseDatos:MigrarAlArrancar", true))
{
    await app.MigrarBaseDeDatosAsync();
}

// Liveness: no toca la base de datos para no despertar Neon (ADR-05).
app.MapGet("/salud", () => Results.Ok(new EstadoSaludDto("ok", DateTimeOffset.UtcNow)));

app.MapCuentasEndpoints();

app.Run();

// Expuesto para WebApplicationFactory en Cad.Server.Tests.
public partial class Program;
