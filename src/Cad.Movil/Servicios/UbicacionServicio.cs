namespace Cad.Movil.Servicios;

public enum EstadoUbicacion
{
    Comprobando,
    PermisoPendiente,
    PermisoDenegado,

    /// <summary>Android ya no muestra el diálogo del sistema: solo se puede conceder desde los ajustes.</summary>
    PermisoBloqueado,

    GpsDesactivado,
    NoDisponible,
    Disponible
}

/// <summary>
/// HU06 / RF04: comprueba el permiso de ubicación y si el dispositivo puede obtener una posición.
/// RNF07: este servicio no envía nada al servidor; la posición de prueba se descarta.
/// La transmisión llegará solo con el SOS, el reporte (si el ciudadano lo elige) y el acompañamiento.
/// </summary>
public sealed class UbicacionServicio
{
    private static readonly TimeSpan TiempoMaximoPrueba = TimeSpan.FromSeconds(10);

    /// <summary>Si ya se pidió el permiso en esta ejecución; evita insistir cada vez que se vuelve a la pantalla.</summary>
    public bool PermisoSolicitado { get; private set; }

    public Task<PermissionStatus> ConsultarPermisoAsync() =>
        MainThread.InvokeOnMainThreadAsync(Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>);

    /// <summary>Solo "mientras se usa la app": no se pide ubicación en segundo plano (RNF11).</summary>
    public Task<PermissionStatus> SolicitarPermisoAsync()
    {
        PermisoSolicitado = true;
        return MainThread.InvokeOnMainThreadAsync(Permissions.RequestAsync<Permissions.LocationWhenInUse>);
    }

    /// <summary>
    /// Tras un rechazo, Android indica si todavía mostrará su diálogo. Si no, el permiso quedó bloqueado.
    /// </summary>
    public bool PuedeVolverAPedirse() => Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>();

    /// <summary>Intenta obtener una posición actual solo para saber si la ubicación funciona.</summary>
    public async Task<EstadoUbicacion> ComprobarDisponibilidadAsync(CancellationToken ct = default)
    {
        try
        {
            var solicitud = new GeolocationRequest(GeolocationAccuracy.Medium, TiempoMaximoPrueba);
            var posicion = await Geolocation.Default.GetLocationAsync(solicitud, ct);

            // La posición no se guarda ni se transmite (RNF07).
            return posicion is null ? EstadoUbicacion.NoDisponible : EstadoUbicacion.Disponible;
        }
        catch (FeatureNotEnabledException)
        {
            return EstadoUbicacion.GpsDesactivado;
        }
        catch (PermissionException)
        {
            return EstadoUbicacion.PermisoDenegado;
        }
        catch (FeatureNotSupportedException)
        {
            return EstadoUbicacion.NoDisponible;
        }
        catch (OperationCanceledException)
        {
            return EstadoUbicacion.NoDisponible;
        }
    }

    public void AbrirAjustes() => AppInfo.Current.ShowSettingsUI();
}
