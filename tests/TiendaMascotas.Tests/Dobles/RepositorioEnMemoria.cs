using TiendaMascotas.Core.Dominio;
using TiendaMascotas.Core.Persistencia;

namespace TiendaMascotas.Tests.Dobles;

/// <summary>
/// FAKE: una implementación que FUNCIONA de verdad, pero simplificada (una lista en memoria
/// en vez de un archivo o base de datos). Rápida y sin efectos secundarios en disco.
/// </summary>
internal sealed class RepositorioEnMemoria : IRepositorioPedidos
{
    private readonly List<PedidoProcesado> _pedidos = [];

    public IReadOnlyList<PedidoProcesado> Pedidos => _pedidos;

    public void Guardar(PedidoProcesado pedido) => _pedidos.Add(pedido);

    public bool YaFueProcesado(Guid pedidoId) => _pedidos.Any(pedido => pedido.PedidoId == pedidoId);
}
