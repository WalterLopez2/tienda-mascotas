using TiendaMascotas.Core.Precios;
using static TiendaMascotas.Tests.Utilidades.PedidoBuilder;

namespace TiendaMascotas.Tests.Unitarias;

/// <summary>
/// Desarrollado con TDD (ver historial de commits de la rama feature/cupones-tdd):
/// cada requisito nuevo entró primero como prueba en ROJO.
/// </summary>
public class DescuentoCuponTests
{
    private readonly DescuentoCupon _politica = new();

    [Fact]
    public void Aplicar_CuponHuellitas10_Descuenta10PorCiento()
    {
        // Arrange
        var pedido = UnPedido().ConCupon("HUELLITAS10").Construir();

        // Act
        var resultado = _politica.Aplicar(100m, pedido);

        // Assert
        Assert.Equal(90m, resultado);
    }

    [Fact]
    public void Aplicar_SinCupon_NoDescuenta()
    {
        // Arrange
        var pedido = UnPedido().Construir();

        // Act
        var resultado = _politica.Aplicar(100m, pedido);

        // Assert
        Assert.Equal(100m, resultado);
    }

    // Requisito nuevo: los clientes escriben el cupón como sea.
    [Theory]
    [InlineData("huellitas10")]
    [InlineData("  Huellitas10 ")]
    public void Aplicar_CuponEnMinusculasOConEspacios_IgualDescuenta(string cuponEscrito)
    {
        // Arrange
        var pedido = UnPedido().ConCupon(cuponEscrito).Construir();

        // Act
        var resultado = _politica.Aplicar(100m, pedido);

        // Assert
        Assert.Equal(90m, resultado);
    }
}
