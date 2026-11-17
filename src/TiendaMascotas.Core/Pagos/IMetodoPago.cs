namespace TiendaMascotas.Core.Pagos;

public interface IMetodoPago
{
    string Nombre { get; }
    decimal CalcularMontoACobrar(decimal total);
}
