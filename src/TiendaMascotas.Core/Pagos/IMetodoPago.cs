namespace TiendaMascotas.Core.Pagos;

/// <summary>
/// Polimorfismo en vez de enum + switch: cada método de pago sabe su propio recargo.
/// Agregar "Transferencia" no obliga a tocar ServicioPedidos (OCP).
/// </summary>
public interface IMetodoPago
{
    string Nombre { get; }
    decimal CalcularMontoACobrar(decimal total);
}
