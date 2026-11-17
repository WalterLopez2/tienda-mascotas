namespace TiendaMascotas.Core.Pagos;

public sealed class PagoTarjeta : IMetodoPago, IReembolsable
{
    private const decimal Recargo = 0.03m;

    public string Nombre => "Tarjeta";

    public decimal CalcularMontoACobrar(decimal total) => total * (1 + Recargo);

    // Se devuelve todo lo cobrado, recargo incluido.
    public decimal CalcularReembolso(decimal montoCobrado) => montoCobrado;
}
