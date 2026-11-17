using TiendaMascotas.Core.Dominio;

namespace TiendaMascotas.Core.Precios;

public sealed class DescuentoCompraGrande : IPoliticaDescuento
{
    private const decimal MontoMinimo = 1000m;
    private const decimal Porcentaje = 0.10m;

    public decimal Aplicar(decimal monto, Pedido pedido) =>
        monto > MontoMinimo ? monto * (1 - Porcentaje) : monto;
}
