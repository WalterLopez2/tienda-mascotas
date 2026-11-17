using TiendaMascotas.Core.Pagos;

namespace TiendaMascotas.Tests.Unitarias;

public class MetodosPagoTests
{
    public static TheoryData<IMetodoPago> TodosLosMetodosDePago => [new PagoTarjeta(), new PagoEfectivo()];

    [Theory]
    [MemberData(nameof(TodosLosMetodosDePago))]
    public void CalcularMontoACobrar_CualquierMetodo_NuncaCobraMenosQueElTotal(IMetodoPago metodo)
    {
        // Arrange
        const decimal total = 500m;

        // Act
        var monto = metodo.CalcularMontoACobrar(total);

        // Assert
        Assert.True(monto >= total, $"{metodo.Nombre} cobró {monto}, menos que el total {total}");
    }

    [Fact]
    public void CalcularMontoACobrar_Tarjeta_AgregaRecargoDel3PorCiento()
    {
        // Arrange
        var tarjeta = new PagoTarjeta();

        // Act
        var monto = tarjeta.CalcularMontoACobrar(100m);

        // Assert
        Assert.Equal(103m, monto);
    }

    [Fact]
    public void CalcularMontoACobrar_Efectivo_CobraElTotalExacto()
    {
        // Arrange
        var efectivo = new PagoEfectivo();

        // Act
        var monto = efectivo.CalcularMontoACobrar(100m);

        // Assert
        Assert.Equal(100m, monto);
    }

    [Fact]
    public void Efectivo_NoEsReembolsable_PorDisenoNoPorExcepcion()
    {
        // Arrange
        IMetodoPago efectivo = new PagoEfectivo();

        // Act
        var esReembolsable = efectivo is IReembolsable;

        // Assert
        Assert.False(esReembolsable);
    }
}
