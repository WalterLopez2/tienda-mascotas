using TiendaMascotas.Core.Pagos;
using TiendaMascotas.Core.Precios;
using ProcesadorLegacy = TiendaMascotas.Legacy.ProcesadorPedidos;

namespace TiendaMascotas.Tests.Caracterizacion;

/// <summary>
/// El refactor es seguro si, para las mismas entradas, la versión nueva
/// cobra exactamente lo mismo que la vieja.
/// </summary>
public class EquivalenciaLegacyTests : IDisposable
{
    private readonly string _archivoTemporal = Path.GetTempFileName();

    public void Dispose() => File.Delete(_archivoTemporal);

    [Theory]
    [InlineData("Croquetas;Alimento;250;2|Pelota;Juguete;45;3|Cama;Accesorio;600;1", "HUELLITAS10", "tarjeta")]
    [InlineData("Croquetas;Alimento;250;2|Pelota;Juguete;45;3|Cama;Accesorio;600;1", "HUELLITAS10", "efectivo")]
    [InlineData("Snack;Alimento;30;2", null, "efectivo")]
    [InlineData("Antipulgas;Medicina;200;1", null, "tarjeta")]
    [InlineData("Vacuna;Medicina;1000;1", null, "efectivo")]
    [InlineData("Cama;Accesorio;600;3|Snack;Alimento;30;10", "HUELLITAS10", "tarjeta")]
    public void MontoACobrar_MismasEntradas_EsIgualAlDelLegacy(string items, string? cupon, string pago)
    {
        var montoLegacy = MontoLegacy(items, cupon, pago);

        var montoNuevo = MontoNuevo(items, cupon, pago);

        Assert.Equal(montoLegacy, montoNuevo);
    }

    /// <summary>
    /// CAMBIO INTENCIONAL (feature/cupones-tdd): el legacy ignoraba "huellitas10" en minúsculas.
    /// La red de seguridad lo detectó; como es el requisito nuevo, se documenta aquí
    /// en lugar de borrar el caso en silencio.
    /// </summary>
    [Fact]
    public void MontoACobrar_CuponEnMinusculas_AhoraSiDescuentaAunqueElLegacyNo()
    {
        const string items = "Collar;Accesorio;120;1";

        var montoLegacy = MontoLegacy(items, "huellitas10", "efectivo");
        var montoNuevo = MontoNuevo(items, "huellitas10", "efectivo");

        Assert.Equal(117.30m, montoLegacy);
        Assert.Equal(105.57m, montoNuevo);
    }

    private decimal MontoLegacy(string items, string? cupon, string pago)
    {
        var legacy = new ProcesadorLegacy { archivo = _archivoTemporal };
        var resultado = legacy.Procesar(new object?[] { "Camila", true, items, pago, cupon, "c@correo.com" }!);
        return ConvertidorLegacy.MontoDelResultadoLegacy(resultado);
    }

    private static decimal MontoNuevo(string items, string? cupon, string pago)
    {
        var calculadora = new CalculadoraPrecios([new DescuentoCompraGrande(), new DescuentoCupon()]);
        IMetodoPago metodoPago = pago == "tarjeta" ? new PagoTarjeta() : new PagoEfectivo();
        var total = calculadora.CalcularTotal(ConvertidorLegacy.APedido(items, cupon));
        return Math.Round(metodoPago.CalcularMontoACobrar(total), 2);
    }
}
