namespace TiendaMascotas.Core.Pagos;

public sealed class PagoEfectivo : IMetodoPago
{
    public string Nombre => "Efectivo";

    public decimal CalcularMontoACobrar(decimal total) => total;
}
