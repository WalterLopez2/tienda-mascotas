using TiendaMascotas.Core.Dominio;
using TiendaMascotas.Core.Precios;
using static TiendaMascotas.Tests.Utilidades.PedidoBuilder;

namespace TiendaMascotas.Tests.Unitarias;

public class DescuentoCompraGrandeTests
{
    private readonly DescuentoCompraGrande _politica = new();
    private readonly Pedido _cualquierPedido = UnPedido().Construir();

    [Fact]
    public void Aplicar_MontoMayorA1000_Descuenta10PorCiento()
    {
        // Arrange
        const decimal monto = 2000m;

        // Act
        var resultado = _politica.Aplicar(monto, _cualquierPedido);

        // Assert
        Assert.Equal(1800m, resultado);
    }

    [Theory]
    [InlineData(1000)]
    [InlineData(999.99)]
    public void Aplicar_MontoHasta1000_NoDescuenta(decimal monto)
    {
        // Act
        var resultado = _politica.Aplicar(monto, _cualquierPedido);

        // Assert
        Assert.Equal(monto, resultado);
    }
}
