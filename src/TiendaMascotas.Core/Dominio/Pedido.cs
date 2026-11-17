namespace TiendaMascotas.Core.Dominio;

/// <summary>
/// Antes el pedido era un object[] donde d[3] significaba "tipo de pago".
/// Ahora cada dato tiene nombre y tipo.
/// </summary>
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
