using TiendaMascotas.Core.Dominio;

namespace TiendaMascotas.Core.Notificaciones;

/// <summary>Única responsabilidad: avisarle al cliente.</summary>
public sealed class NotificadorConsola
{
    public void Notificar(Cliente cliente, string mensaje) =>
        Console.WriteLine($"  [correo a {cliente.Correo}] {mensaje}");
}
