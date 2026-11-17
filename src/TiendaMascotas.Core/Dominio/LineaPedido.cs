namespace TiendaMascotas.Core.Dominio;

public sealed record LineaPedido
{
    public Producto Producto { get; }
    public int Cantidad { get; }

    public LineaPedido(Producto producto, int cantidad)
    {
        if (cantidad <= 0)
            throw new ArgumentOutOfRangeException(nameof(cantidad), "La cantidad debe ser mayor que cero.");

        Producto = producto;
        Cantidad = cantidad;
    }

    public decimal Importe => Producto.Precio * Cantidad;
}
