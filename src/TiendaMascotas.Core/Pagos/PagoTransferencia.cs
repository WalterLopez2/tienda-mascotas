namespace TiendaMascotas.Core.Pagos;

// clase de pago por transferencia
public sealed class PagoTransferencia : IMetodoPago
{
    public string Nombre => "Transferencia";

    public decimal CalcularMontoACobrar(decimal total) => total + 15m;
}
