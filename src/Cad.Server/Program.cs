using Cad.Contratos.Salud;

var builder = WebApplication.CreateBuilder(args);

// Panel de central servido por el mismo proceso (sección 3.2).
builder.Services.AddRazorPages(o => o.RootDirectory = "/Panel/Pages");

var app = builder.Build();

app.UseStaticFiles();

// Liveness: no toca la base de datos para no despertar Neon (ADR-05).
app.MapGet("/salud", () => Results.Ok(new EstadoSaludDto("ok", DateTimeOffset.UtcNow)));

app.MapRazorPages();

app.Run();

// Expuesto para WebApplicationFactory en Cad.Server.Tests.
public partial class Program;
