namespace TiendaMascotas.Core.Pagos;

public sealed class PagoTarjeta : IMetodoPago
{
    private const decimal Recargo = 0.03m;

    public string Nombre => "Tarjeta";

    public decimal CalcularMontoACobrar(decimal total) => total * (1 + Recargo);
}
