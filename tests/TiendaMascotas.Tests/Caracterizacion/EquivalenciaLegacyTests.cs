using TiendaMascotas.Core.Precios;
using TiendaMascotas.Legacy;

namespace TiendaMascotas.Tests.Caracterizacion;

/// <summary>
/// El refactor es seguro si, para las mismas entradas, la versión nueva
/// produce exactamente lo mismo que la vieja.
/// </summary>
public class EquivalenciaLegacyTests : IDisposable
{
    private readonly string _archivoTemporal = Path.GetTempFileName();

    public void Dispose() => File.Delete(_archivoTemporal);

    [Theory]
    [InlineData("Croquetas;Alimento;250;2|Pelota;Juguete;45;3|Cama;Accesorio;600;1", "HUELLITAS10")]
    [InlineData("Snack;Alimento;30;2", null)]
    [InlineData("Antipulgas;Medicina;200;1", null)]
    [InlineData("Collar;Accesorio;120;1", "huellitas10")]
    [InlineData("Vacuna;Medicina;1000;1", null)]
    [InlineData("Cama;Accesorio;600;3|Snack;Alimento;30;10", "HUELLITAS10")]
    public void CalcularTotal_MismasEntradas_DaElMismoTotalQueElLegacy(string items, string? cupon)
    {
        var legacy = new ProcesadorPedidos { archivo = _archivoTemporal };
        var totalLegacy = ConvertidorLegacy.MontoDelResultadoLegacy(
            legacy.Procesar(new object?[] { "Camila", true, items, "efectivo", cupon, "c@correo.com" }!));

        var totalNuevo = new CalculadoraPrecios().CalcularTotal(ConvertidorLegacy.APedido(items, cupon));

        Assert.Equal(totalLegacy, Math.Round(totalNuevo, 2));
    }
}
