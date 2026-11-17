using TiendaMascotas.Core.Dominio;

namespace TiendaMascotas.Core.Notificaciones;

/// <summary>
/// ISP: el INotificador del legacy tenía 4 métodos (correo, SMS, WhatsApp, ticket)
/// y NotificadorCorreo tiraba NotImplementedException en 3 de ellos.
/// Una interfaz pequeña: cada canal implementa solo lo que sabe hacer.
/// </summary>
public interface INotificador
{
    void Notificar(Cliente cliente, string mensaje);
}
