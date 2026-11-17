using TiendaMascotas.Core.Precios;
using TiendaMascotas.Tests.Dobles;
using static TiendaMascotas.Tests.Utilidades.PedidoBuilder;

namespace TiendaMascotas.Tests.Unitarias;

/// <summary>
/// Desarrollado con TDD (ver historial de commits de la rama feature/cupones-tdd):
/// cada requisito nuevo entró primero como prueba en ROJO.
/// </summary>
public class DescuentoCuponTests
{
    private static readonly DateTime AntesDeBlackFriday = new(2026, 11, 20);
    private static readonly DateTime DespuesDeBlackFriday = new(2026, 12, 1);

    private readonly DescuentoCupon _politica = new(new RelojFijo(AntesDeBlackFriday));

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

    // Requisito nuevo: campaña de bienvenida.
    [Fact]
    public void Aplicar_CuponPrimeraCompra_Descuenta15PorCiento()
    {
        // Arrange
        var pedido = UnPedido().ConCupon("PRIMERACOMPRA").Construir();

        // Act
        var resultado = _politica.Aplicar(100m, pedido);

        // Assert
        Assert.Equal(85m, resultado);
    }

    [Fact]
    public void Aplicar_CuponQueNoExiste_NoDescuenta()
    {
        // Arrange
        var pedido = UnPedido().ConCupon("GRATIS100").Construir();

        // Act
        var resultado = _politica.Aplicar(100m, pedido);

        // Assert
        Assert.Equal(100m, resultado);
    }

    // Requisito nuevo: campaña de Black Friday.
    [Fact]
    public void Aplicar_CuponBlackFriday_Descuenta20PorCiento()
    {
        // Arrange
        var pedido = UnPedido().ConCupon("BLACKFRIDAY").Construir();

        // Act
        var resultado = _politica.Aplicar(100m, pedido);

        // Assert
        Assert.Equal(80m, resultado);
    }

    // Requisito nuevo: los cupones de campaña vencen.
    [Fact]
    public void Aplicar_CuponBlackFridayYaVencido_NoDescuenta()
    {
        // Arrange
        var politica = new DescuentoCupon(new RelojFijo(DespuesDeBlackFriday));
        var pedido = UnPedido().ConCupon("BLACKFRIDAY").Construir();

        // Act
        var resultado = politica.Aplicar(100m, pedido);

        // Assert
        Assert.Equal(100m, resultado);
    }
}
