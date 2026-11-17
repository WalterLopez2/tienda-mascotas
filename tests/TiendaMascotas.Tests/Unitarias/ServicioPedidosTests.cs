using TiendaMascotas.Core.Dominio;
using TiendaMascotas.Core.Pagos;
using TiendaMascotas.Tests.Dobles;
using TiendaMascotas.Tests.Utilidades;
using static TiendaMascotas.Tests.Utilidades.PedidoBuilder;

namespace TiendaMascotas.Tests.Unitarias;

/// <summary>
/// Pruebas del orquestador usando los 5 dobles de prueba.
/// Gracias a DIP, ninguna toca disco, red ni consola: corren en milisegundos.
/// </summary>
public class ServicioPedidosTests
{
    private const decimal MontoPedidoDeCamilaConTarjeta = 1061.63m;

    [Fact] // DUMMY + SPY
    public void Procesar_ClienteInactivo_NoCobraNiNotifica()
    {
        // Arrange
        var notificador = new NotificadorEspia();
        var servicio = Fabrica.Servicio(pasarela: new PasarelaDummy(), notificador: notificador);
        var pedido = UnPedido().DeClienteInactivo().Con("Snack", Categoria.Alimento, 30m).Construir();

        // Act
        var resultado = servicio.Procesar(pedido, new PagoEfectivo());

        // Assert
        Assert.False(resultado.Exitoso);
        Assert.Equal("Cliente inactivo", resultado.Mensaje);
        Assert.Empty(notificador.Enviados);
    }

    [Fact] // MOCK
    public void Procesar_PedidoValido_CobraElMontoCorrectoUnaSolaVez()
    {
        // Arrange
        var pedido = Fabrica.PedidoDeCamila();
        var pasarela = new PasarelaPagoMock().EsperarUnSoloCobro(pedido.Id, MontoPedidoDeCamilaConTarjeta);
        var servicio = Fabrica.Servicio(pasarela: pasarela);

        // Act
        servicio.Procesar(pedido, new PagoTarjeta());

        // Assert
        pasarela.Verificar();
    }

    [Fact] // SPY
    public void Procesar_PedidoValido_NotificaAlClienteConElTotal()
    {
        // Arrange
        var notificador = new NotificadorEspia();
        var servicio = Fabrica.Servicio(notificador: notificador);

        // Act
        servicio.Procesar(Fabrica.PedidoDeCamila(), new PagoTarjeta());

        // Assert
        var (cliente, mensaje) = Assert.Single(notificador.Enviados);
        Assert.Equal("camila@correo.com", cliente.Correo);
        Assert.Contains("L 1,061.63", mensaje);
    }

    [Fact] // FAKE + STUB
    public void Procesar_PedidoValido_GuardaElPedidoConLaFechaDelReloj()
    {
        // Arrange
        var repositorio = new RepositorioEnMemoria();
        var servicio = Fabrica.Servicio(repositorio: repositorio, reloj: new RelojFijo(Fabrica.FechaFija));
        var pedido = Fabrica.PedidoDeCamila();

        // Act
        servicio.Procesar(pedido, new PagoTarjeta());

        // Assert
        var guardado = Assert.Single(repositorio.Pedidos);
        Assert.Equal(pedido.Id, guardado.PedidoId);
        Assert.Equal(MontoPedidoDeCamilaConTarjeta, guardado.MontoCobrado);
        Assert.Equal("Tarjeta", guardado.MetodoPago);
        Assert.Equal(Fabrica.FechaFija, guardado.Fecha);
    }

    [Fact] // MOCK configurado para rechazar + FAKE + SPY
    public void Procesar_PagoRechazado_NoGuardaNiNotifica()
    {
        // Arrange
        var repositorio = new RepositorioEnMemoria();
        var notificador = new NotificadorEspia();
        var servicio = Fabrica.Servicio(
            pasarela: new PasarelaPagoMock(aprobarCobros: false),
            repositorio: repositorio,
            notificador: notificador);

        // Act
        var resultado = servicio.Procesar(Fabrica.PedidoDeCamila(), new PagoTarjeta());

        // Assert
        Assert.False(resultado.Exitoso);
        Assert.Equal("El pago fue rechazado", resultado.Mensaje);
        Assert.Empty(repositorio.Pedidos);
        Assert.Empty(notificador.Enviados);
    }

    [Fact]
    public void Procesar_PedidoSinProductos_DevuelveFallo()
    {
        // Arrange
        var servicio = Fabrica.Servicio(pasarela: new PasarelaDummy());

        // Act
        var resultado = servicio.Procesar(UnPedido().Construir(), new PagoEfectivo());

        // Assert
        Assert.False(resultado.Exitoso);
        Assert.Equal("El pedido no tiene productos", resultado.Mensaje);
    }
}
