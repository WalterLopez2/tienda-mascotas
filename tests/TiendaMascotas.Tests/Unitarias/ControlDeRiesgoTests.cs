using TiendaMascotas.Core.Riesgo;

namespace TiendaMascotas.Tests.Unitarias;

public class ControlDeRiesgoTests
{
    [Fact]
    public void Autoriza_MontoDentroDeLosLimites_DevuelveVerdadero()
    {
        // Arrange
        var control = new ControlDeRiesgo(montoMaximoPorPedido: 100m, montoMaximoAcumulado: 1_000m);

        // Act
        var autorizado = control.Autoriza(100m);

        // Assert
        Assert.True(autorizado);
    }

    [Fact]
    public void Autoriza_SuperaElAcumulado_SeDetieneYRechazaTodoLoQueSigue()
    {
        // Arrange
        var control = new ControlDeRiesgo(montoMaximoPorPedido: 100m, montoMaximoAcumulado: 150m);
        control.RegistrarCobro(100m);

        // Act
        var superaLimite = control.Autoriza(60m);
        var montoPequenoDespues = control.Autoriza(1m);

        // Assert: una vez detenido, ni siquiera L 1 pasa
        Assert.False(superaLimite);
        Assert.False(montoPequenoDespues);
        Assert.True(control.EstaDetenido);
    }

    [Theory]
    [InlineData(0, 100)]
    [InlineData(100, -1)]
    public void Constructor_LimitesNoPositivos_LanzaExcepcion(decimal porPedido, decimal acumulado)
    {
        // Act
        var crear = () => new ControlDeRiesgo(porPedido, acumulado);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(crear);
    }
}
