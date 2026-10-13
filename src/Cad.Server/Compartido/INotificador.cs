namespace Cad.Server.Compartido;

/// <summary>
/// Publica eventos de tiempo real. Lo define la aplicación y lo implementa la capa de presentación
/// (NotificadorSignalR), así los casos de uso no conocen SignalR (figura 10).
/// Se llama siempre después de confirmar la transacción: un evento perdido se recupera consultando la BD.
/// </summary>
public interface INotificador
{
    Task SosNuevoAsync(Guid casoId, CancellationToken ct);
}
