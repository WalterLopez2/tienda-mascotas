namespace TiendaMascotas.Core.Pagos;

public sealed class PagoTransferencia : IMetodoPago, IReembolsable
{
    private const decimal ComisionBancaria = 15m;

    public string Nombre => "Transferencia";

    public decimal CalcularMontoACobrar(decimal total) => total + ComisionBancaria;

    // El banco no devuelve su comisión.
    public decimal CalcularReembolso(decimal montoCobrado) => montoCobrado - ComisionBancaria;
}
