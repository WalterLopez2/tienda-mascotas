using TiendaMascotas.Core.Dominio;

namespace TiendaMascotas.Core.Notificaciones;

public interface INotificador
{
    void Notificar(Cliente cliente, string mensaje);
}
