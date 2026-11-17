using TiendaMascotas.Core.Dominio;
using TiendaMascotas.Core.Pagos;
using TiendaMascotas.Core.Riesgo;
using TiendaMascotas.Tests.Dobles;
using TiendaMascotas.Tests.Utilidades;
using static TiendaMascotas.Tests.Utilidades.PedidoBuilder;
using ProcesadorLegacy = TiendaMascotas.Legacy.ProcesadorPedidos;

namespace TiendaMascotas.Tests.KnightCapital;

/// <summary>
/// Knight Capital, 1 de agosto de 2012:
///  1. Reutilizaron una bandera vieja ("Power Peg") para una función nueva.
///  2. El despliegue llegó a 7 de 8 servidores; el octavo seguía con el código muerto.
///  3. Al activar la bandera, ese servidor revivió el código viejo y mandó órdenes sin parar.
///  4. No había un freno automático: ~440 millones de dólares perdidos en 45 minutos.
///
/// Estas pruebas convierten cada lección en algo que el CI verifica en cada commit.
/// </summary>
public class KnightCapitalTests : IDisposable
{
    private readonly string _archivoTemporal = Path.GetTempFileName();

    public void Dispose() => File.Delete(_archivoTemporal);

    /// <summary>Lección 1: código muerto + bandera reutilizada. Así se comporta el legacy.</summary>
    [Fact]
    public void Legacy_BanderaViejaReactivada_DuplicaElCobroSinAvisar()
    {
        // Arrange
        var legacy = new ProcesadorLegacy { archivo = _archivoTemporal, modoViejo = true };
        var datos = new object?[] { "Camila", true, "Snack;Alimento;30;2", "efectivo", null, "c@correo.com" };

        // Act
        var resultado = legacy.Procesar(datos!);

        // Assert: debería cobrar L 65.55 y cobra el doble, con un "OK" como si nada.
        Assert.Equal("OK:131.1", resultado);
    }

    /// <summary>Lección 1 (versión nueva): la bandera "modoViejo" fue eliminada, no "desactivada".</summary>
    [Fact]
    public void ServicioNuevo_NoTieneBanderasOcultas()
    {
        var miembros = typeof(Core.Servicios.ServicioPedidos).GetMembers()
            .Select(m => m.Name.ToLowerInvariant());

        Assert.DoesNotContain(miembros, nombre => nombre.Contains("modo") || nombre.Contains("viejo"));
    }

    /// <summary>Lección 3: el sistema repetía órdenes. Aquí, el mismo pedido nunca se cobra dos veces.</summary>
    [Fact]
    public void Procesar_MismoPedidoDosVeces_CobraUnaSolaVez()
    {
        // Arrange
        var pasarela = new PasarelaPagoMock();
        var servicio = Fabrica.Servicio(pasarela: pasarela);
        var pedido = Fabrica.PedidoDeCamila();

        // Act
        var primero = servicio.Procesar(pedido, new PagoTarjeta());
        var segundo = servicio.Procesar(pedido, new PagoTarjeta());

        // Assert
        Assert.True(primero.Exitoso);
        Assert.False(segundo.Exitoso);
        Assert.Equal("El pedido ya fue procesado", segundo.Mensaje);
        Assert.Equal(1, pasarela.CantidadDeCobros);
    }

    /// <summary>Lección 4: un monto absurdo para un solo pedido se bloquea antes de llegar al banco.</summary>
    [Fact]
    public void Procesar_MontoMayorAlLimitePorPedido_NoLlegaALaPasarela()
    {
        // Arrange
        var servicio = Fabrica.Servicio(
            pasarela: new PasarelaDummy(),
            controlDeRiesgo: new ControlDeRiesgo(montoMaximoPorPedido: 1_000m, montoMaximoAcumulado: 100_000m));
        var pedidoEnorme = UnPedido().Con("Vacuna", Categoria.Medicina, 5_000m).Construir();

        // Act
        var resultado = servicio.Procesar(pedidoEnorme, new PagoEfectivo());

        // Assert
        Assert.False(resultado.Exitoso);
        Assert.Equal("Bloqueado por control de riesgo", resultado.Mensaje);
    }

    /// <summary>Lección 4: el "kill switch". Si los cobros se disparan, el sistema se detiene solo.</summary>
    [Fact]
    public void Procesar_AlSuperarElLimiteAcumulado_SeDetieneYNoCobraMas()
    {
        // Arrange
        var pasarela = new PasarelaPagoMock();
        var controlDeRiesgo = new ControlDeRiesgo(montoMaximoPorPedido: 200m, montoMaximoAcumulado: 500m);
        var servicio = Fabrica.Servicio(pasarela: pasarela, controlDeRiesgo: controlDeRiesgo);

        // Act: una "avalancha" de 10 pedidos de L 115 cada uno
        var resultados = Enumerable.Range(0, 10)
            .Select(_ => UnPedido().Con("Vacuna", Categoria.Medicina, 100m).Construir())
            .Select(pedido => servicio.Procesar(pedido, new PagoEfectivo()))
            .ToList();

        // Assert: pasan 4 (L 460), el 5.º superaría L 500 y activa el freno
        Assert.Equal(4, pasarela.CantidadDeCobros);
        Assert.Equal(4, resultados.Count(r => r.Exitoso));
        Assert.True(controlDeRiesgo.EstaDetenido);
    }
}
