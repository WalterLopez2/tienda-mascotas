using TiendaMascotas.Core.Pagos;

namespace TiendaMascotas.Tests.Unitarias;

public class PagoTransferenciaTests
{
    [Fact]
    public void CalcularMontoACobrar_Transferencia_SumaLaComisionBancariaFija()
    {
        // Arrange
        var transferencia = new PagoTransferencia();

        // Act
        var monto = transferencia.CalcularMontoACobrar(100m);

        // Assert
        Assert.Equal(115m, monto);
    }

    [Fact]
    public void CalcularReembolso_Transferencia_DevuelveTodoMenosLaComision()
    {
        // Arrange
        var transferencia = new PagoTransferencia();

        // Act
        var reembolso = transferencia.CalcularReembolso(115m);

        // Assert
        Assert.Equal(100m, reembolso);
    }
}
