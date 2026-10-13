using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Cad.Server.Persistencia;

/// <summary>Solo para `dotnet ef migrations`: no se conecta a ninguna base de datos.</summary>
public sealed class CadDbContextFactory : IDesignTimeDbContextFactory<CadDbContext>
{
    public CadDbContext CreateDbContext(string[] args)
    {
        var opciones = new DbContextOptionsBuilder<CadDbContext>()
            .UseNpgsql("Host=localhost;Database=cad_diseno")
            .UseSnakeCaseNamingConvention()
            .Options;
        return new CadDbContext(opciones);
    }
}
