using TiendaMascotas.Core.Dominio;
using TiendaMascotas.Core.Notificaciones;

namespace TiendaMascotas.Tests.Dobles;

/// <summary>
/// SPY (espía): se comporta como un notificador cualquiera pero ANOTA cada llamada.
/// Después, la prueba le pregunta qué pasó y hace sus propios Assert.
/// </summary>
internal sealed class NotificadorEspia : INotificador
{
    private readonly List<(Cliente Cliente, string Mensaje)> _enviados = [];

    public IReadOnlyList<(Cliente Cliente, string Mensaje)> Enviados => _enviados;

    public void Notificar(Cliente cliente, string mensaje) => _enviados.Add((cliente, mensaje));
}
