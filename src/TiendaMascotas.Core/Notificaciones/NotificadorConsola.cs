using TiendaMascotas.Core.Dominio;

namespace TiendaMascotas.Core.Notificaciones;

public sealed class NotificadorConsola : INotificador
{
    public void Notificar(Cliente cliente, string mensaje) =>
        Console.WriteLine($"  [correo a {cliente.Correo}] {mensaje}");
}
