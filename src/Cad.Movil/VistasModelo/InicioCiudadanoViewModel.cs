using Cad.Movil.Servicios;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Cad.Movil.VistasModelo;

/// <summary>Inicio del modo ciudadano con la preparación de la geolocalización (HU06).</summary>
public sealed partial class InicioCiudadanoViewModel(SesionServicio sesion, UbicacionServicio ubicacion, SosViewModel sos)
    : InicioModoViewModel(sesion)
{
    /// <summary>Botón SOS (HU07/HU08).</summary>
    public SosViewModel Sos { get; } = sos;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Titulo), nameof(Detalle), nameof(TextoAccion), nameof(HayAccion),
        nameof(UbicacionLista), nameof(Comprobando))]
    private EstadoUbicacion estado = EstadoUbicacion.Comprobando;

    public bool UbicacionLista => Estado == EstadoUbicacion.Disponible;
    public bool Comprobando => Estado == EstadoUbicacion.Comprobando;

    public string Titulo => Estado switch
    {
        EstadoUbicacion.Disponible => "Ubicación lista",
        EstadoUbicacion.GpsDesactivado => "GPS desactivado",
        EstadoUbicacion.NoDisponible => "No se pudo obtener tu ubicación",
        EstadoUbicacion.Comprobando => "Comprobando ubicación…",
        _ => "Ubicación sin permiso"
    };

    public string Detalle => Estado switch
    {
        EstadoUbicacion.Disponible =>
            "Solo se enviará cuando pidas un SOS, si eliges usarla en un reporte o durante un acompañamiento.",
        EstadoUbicacion.GpsDesactivado => "Actívalo desde los ajustes rápidos del teléfono.",
        EstadoUbicacion.NoDisponible => "Revisa que estés en un lugar con señal e inténtalo de nuevo.",
        EstadoUbicacion.PermisoBloqueado =>
            "Puedes enviar un SOS igual, pero sin ubicación. Para permitirla, actívala en los ajustes de la app.",
        EstadoUbicacion.Comprobando => "",
        _ => "Puedes enviar un SOS igual, pero la central no sabrá dónde estás."
    };

    public string TextoAccion => Estado switch
    {
        EstadoUbicacion.PermisoBloqueado => "Abrir ajustes",
        EstadoUbicacion.PermisoPendiente or EstadoUbicacion.PermisoDenegado => "Permitir ubicación",
        _ => "Comprobar de nuevo"
    };

    public bool HayAccion => Estado is not (EstadoUbicacion.Disponible or EstadoUbicacion.Comprobando);

    /// <summary>
    /// Al abrir el modo ciudadano: la primera vez en la ejecución explica la finalidad y pide el permiso;
    /// después solo actualiza el estado, sin volver a preguntar.
    /// </summary>
    [RelayCommand]
    private async Task PrepararAsync()
    {
        Estado = EstadoUbicacion.Comprobando;
        var permiso = await ubicacion.ConsultarPermisoAsync();

        if (permiso == PermissionStatus.Granted)
        {
            Estado = await ubicacion.ComprobarDisponibilidadAsync();
            return;
        }

        if (ubicacion.PermisoSolicitado)
        {
            Estado = EstadoSinPermiso();
            return;
        }

        await PedirPermisoConExplicacionAsync();
    }

    [RelayCommand]
    private async Task EjecutarAccionAsync()
    {
        switch (Estado)
        {
            case EstadoUbicacion.PermisoBloqueado:
                ubicacion.AbrirAjustes();
                break;
            case EstadoUbicacion.PermisoPendiente or EstadoUbicacion.PermisoDenegado:
                await PedirPermisoAsync();
                break;
            default:
                Estado = EstadoUbicacion.Comprobando;
                Estado = await ubicacion.ComprobarDisponibilidadAsync();
                break;
        }
    }

    // RF04: se explica la finalidad antes de que aparezca el diálogo del sistema.
    private async Task PedirPermisoConExplicacionAsync()
    {
        var aceptar = await Shell.Current.DisplayAlertAsync(
            "¿Para qué usamos tu ubicación?",
            "Cuando envíes un SOS, la central verá dónde estás para enviar ayuda. " +
            "Tu ubicación solo se comparte en un SOS, en un reporte si tú lo eliges o durante un acompañamiento " +
            "que actives. Abrir la app no la envía.",
            "Continuar",
            "Ahora no");

        if (!aceptar)
        {
            Estado = EstadoUbicacion.PermisoPendiente;
            return;
        }

        await PedirPermisoAsync();
    }

    private async Task PedirPermisoAsync()
    {
        var permiso = await ubicacion.SolicitarPermisoAsync();
        Estado = permiso == PermissionStatus.Granted
            ? await ubicacion.ComprobarDisponibilidadAsync()
            : EstadoSinPermiso();
    }

    private EstadoUbicacion EstadoSinPermiso() =>
        ubicacion.PuedeVolverAPedirse() ? EstadoUbicacion.PermisoDenegado : EstadoUbicacion.PermisoBloqueado;
}
