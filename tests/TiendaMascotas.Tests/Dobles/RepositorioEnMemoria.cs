using TiendaMascotas.Core.Dominio;
using TiendaMascotas.Core.Persistencia;

namespace TiendaMascotas.Tests.Dobles;

internal sealed class RepositorioEnMemoria : IRepositorioPedidos
{
    private readonly List<PedidoProcesado> _pedidos = [];

    public IReadOnlyList<PedidoProcesado> Pedidos => _pedidos;

    public void Guardar(PedidoProcesado pedido) => _pedidos.Add(pedido);

    public bool YaFueProcesado(Guid pedidoId) => _pedidos.Any(pedido => pedido.PedidoId == pedidoId);
}
