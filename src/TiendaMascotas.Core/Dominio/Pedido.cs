namespace TiendaMascotas.Core.Dominio;

public sealed class Pedido
{
    public Guid Id { get; }
    public Cliente Cliente { get; }
    public IReadOnlyList<LineaPedido> Lineas { get; }
    public string? Cupon { get; }

    public Pedido(Cliente cliente, IEnumerable<LineaPedido> lineas, string? cupon = null, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        Cliente = cliente;
        Lineas = lineas.ToList();
        Cupon = cupon;
    }

    public bool TieneLineas => Lineas.Count > 0;
}
