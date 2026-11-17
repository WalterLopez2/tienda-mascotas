using TiendaMascotas.Core.Dominio;
using TiendaMascotas.Core.Precios;
using static TiendaMascotas.Tests.Utilidades.PedidoBuilder;

namespace TiendaMascotas.Tests.Unitarias;

public class CalculadoraPreciosTests
{
    private static CalculadoraPrecios SinPoliticas() => new([]);

    [Theory]
    [InlineData(Categoria.Alimento, 95)]
    [InlineData(Categoria.Juguete, 90)]
    [InlineData(Categoria.Accesorio, 85)]
    [InlineData(Categoria.Medicina, 100)]
    public void CalcularSubtotal_UnProductoDe100_AplicaElDescuentoDeSuCategoria(Categoria categoria, decimal esperado)
    {
        // Arrange
        var pedido = UnPedido().Con("Producto", categoria, 100m).Construir();

        // Act
        var subtotal = SinPoliticas().CalcularSubtotal(pedido);

        // Assert
        Assert.Equal(esperado, subtotal);
    }

    [Fact]
    public void CalcularSubtotal_VariasLineas_MultiplicaPrecioPorCantidadYSuma()
    {
        // Arrange
        var pedido = UnPedido()
            .Con("Vacuna", Categoria.Medicina, 200m, cantidad: 2)
            .Con("Jarabe", Categoria.Medicina, 50m, cantidad: 3)
            .Construir();

        // Act
        var subtotal = SinPoliticas().CalcularSubtotal(pedido);

        // Assert
        Assert.Equal(550m, subtotal);
    }

    [Fact]
    public void CalcularTotal_SinPoliticas_SoloAgregaIsvDel15PorCiento()
    {
        // Arrange
        var pedido = UnPedido().Con("Vacuna", Categoria.Medicina, 100m).Construir();

        // Act
        var total = SinPoliticas().CalcularTotal(pedido);

        // Assert
        Assert.Equal(115m, total);
    }

    [Fact]
    public void CalcularTotal_ConPoliticas_LasAplicaEnOrdenAntesDelIsv()
    {
        // Arrange
        var calculadora = new CalculadoraPrecios([new RestarDiez(), new RestarDiez()]);
        var pedido = UnPedido().Con("Vacuna", Categoria.Medicina, 100m).Construir();

        // Act
        var total = calculadora.CalcularTotal(pedido);

        // Assert: (100 - 10 - 10) * 1.15
        Assert.Equal(92m, total);
    }

    private sealed class RestarDiez : IPoliticaDescuento
    {
        public decimal Aplicar(decimal monto, Pedido pedido) => monto - 10m;
    }
}
