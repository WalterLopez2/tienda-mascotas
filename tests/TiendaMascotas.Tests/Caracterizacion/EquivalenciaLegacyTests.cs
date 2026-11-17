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
    [InlineData("Collar;Accesorio;120;1", "huellitas10", "efectivo")]
    [InlineData("Vacuna;Medicina;1000;1", null, "efectivo")]
    [InlineData("Cama;Accesorio;600;3|Snack;Alimento;30;10", "HUELLITAS10", "tarjeta")]
    public void MontoACobrar_MismasEntradas_EsIgualAlDelLegacy(string items, string? cupon, string pagoLegacy)
    {
        var legacy = new ProcesadorLegacy { archivo = _archivoTemporal };
        var montoLegacy = ConvertidorLegacy.MontoDelResultadoLegacy(
            legacy.Procesar(new object?[] { "Camila", true, items, pagoLegacy, cupon, "c@correo.com" }!));

        var calculadora = new CalculadoraPrecios([new DescuentoCompraGrande(), new DescuentoCupon()]);
        IMetodoPago metodoPago = pagoLegacy == "tarjeta" ? new PagoTarjeta() : new PagoEfectivo();
        var total = calculadora.CalcularTotal(ConvertidorLegacy.APedido(items, cupon));
        var montoNuevo = Math.Round(metodoPago.CalcularMontoACobrar(total), 2);

        Assert.Equal(montoLegacy, montoNuevo);
    }
}
