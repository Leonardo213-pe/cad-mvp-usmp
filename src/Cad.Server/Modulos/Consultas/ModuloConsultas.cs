using Cad.Server.Modulos.Consultas.Aplicacion;

namespace Cad.Server.Modulos.Consultas;

public static class ModuloConsultas
{
    public static IServiceCollection AddModuloConsultas(this IServiceCollection services)
    {
        services.AddScoped<ConsultasCentral>();
        return services;
    }
}
