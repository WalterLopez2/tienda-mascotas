using TiendaMascotas.Legacy;

namespace TiendaMascotas.Tests.Caracterizacion;

/// <summary>
/// Pruebas de caracterización ("golden master").
/// No dicen si el código legacy está BIEN: fotografían lo que HACE HOY.
/// Son la red de seguridad para refactorizar sin romper (Módulo 1 · Refactoring seguro).
/// </summary>
public class ProcesadorLegacyCaracterizacionTests : IDisposable
{
    private readonly string _archivoTemporal = Path.GetTempFileName();
    private readonly ProcesadorPedidos _procesador;

    public ProcesadorLegacyCaracterizacionTests()
    {
        _procesador = new ProcesadorPedidos { archivo = _archivoTemporal };
    }

    public void Dispose() => File.Delete(_archivoTemporal);

    [Theory]
    [InlineData("Croquetas;Alimento;250;2|Pelota;Juguete;45;3|Cama;Accesorio;600;1", "tarjeta", "HUELLITAS10", "OK:1061.63")]
    [InlineData("Snack;Alimento;30;2", "efectivo", null, "OK:65.55")]
    [InlineData("Antipulgas;Medicina;200;1", "efectivo", null, "OK:230")]
    [InlineData("Collar;Accesorio;120;1", "efectivo", "huellitas10", "OK:117.3")]
    [InlineData("Vacuna;Medicina;1000;1", "efectivo", null, "OK:1150")]
    public void Procesar_PedidoValido_DevuelveElTotalQueCobraHoy(string items, string pago, string? cupon, string esperado)
    {
        var datos = new object?[] { "Camila", true, items, pago, cupon, "camila@correo.com" };

        var resultado = _procesador.Procesar(datos!);

        Assert.Equal(esperado, resultado);
    }

    [Fact]
    public void Procesar_ClienteInactivo_DevuelveError()
    {
        var datos = new object?[] { "Luis", false, "Snack;Alimento;30;1", "efectivo", null, "luis@correo.com" };

        var resultado = _procesador.Procesar(datos!);

        Assert.Equal("ERROR: cliente inactivo", resultado);
    }

    [Fact]
    public void Procesar_SinItems_DevuelveError()
    {
        var datos = new object?[] { "Luis", true, "", "efectivo", null, "luis@correo.com" };

        var resultado = _procesador.Procesar(datos!);

        Assert.Equal("ERROR: sin items", resultado);
    }

    [Fact]
    public void Procesar_MetodoDePagoDesconocido_DevuelveError()
    {
        var datos = new object?[] { "Luis", true, "Snack;Alimento;30;1", "bitcoin", null, "luis@correo.com" };

        var resultado = _procesador.Procesar(datos!);

        Assert.Equal("ERROR", resultado);
    }
}
