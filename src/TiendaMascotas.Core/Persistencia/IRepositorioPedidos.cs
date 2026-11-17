using TiendaMascotas.Core.Dominio;

namespace TiendaMascotas.Core.Persistencia;

public interface IRepositorioPedidos
{
    void Guardar(PedidoProcesado pedido);
}
