using TiendaMascotas.Core.Dominio;

namespace TiendaMascotas.Tests.Unitarias;

public class DominioTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void LineaPedido_CantidadNoPositiva_LanzaExcepcion(int cantidad)
    {
        // Arrange
        var producto = new Producto("Pelota", Categoria.Juguete, 45m);

        // Act
        var crear = () => new LineaPedido(producto, cantidad);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(crear);
    }

    [Fact]
    public void Producto_PrecioNegativo_LanzaExcepcion()
    {
        // Act
        var crear = () => new Producto("Pelota", Categoria.Juguete, -1m);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(crear);
    }

    [Fact]
    public void LineaPedido_Importe_EsPrecioPorCantidad()
    {
        // Arrange
        var linea = new LineaPedido(new Producto("Snack", Categoria.Alimento, 30m), 4);

        // Act
        var importe = linea.Importe;

        // Assert
        Assert.Equal(120m, importe);
    }
}
