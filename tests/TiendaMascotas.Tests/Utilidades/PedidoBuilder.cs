using TiendaMascotas.Core.Dominio;

namespace TiendaMascotas.Tests.Utilidades;

/// <summary>Arma pedidos de prueba con valores por defecto.</summary>
internal sealed class PedidoBuilder
{
    private Cliente _cliente = new("Camila", "camila@correo.com", EstaActivo: true);
    private readonly List<LineaPedido> _lineas = [];
    private string? _cupon;
    private Guid? _id;

    public static PedidoBuilder UnPedido() => new();

    public PedidoBuilder DeClienteInactivo()
    {
        _cliente = _cliente with { EstaActivo = false };
        return this;
    }

    public PedidoBuilder Con(string nombre, Categoria categoria, decimal precio, int cantidad = 1)
    {
        _lineas.Add(new LineaPedido(new Producto(nombre, categoria, precio), cantidad));
        return this;
    }

    public PedidoBuilder ConCupon(string cupon)
    {
        _cupon = cupon;
        return this;
    }

    public PedidoBuilder ConId(Guid id)
    {
        _id = id;
        return this;
    }

    public Pedido Construir() => new(_cliente, _lineas, _cupon, _id);
}
