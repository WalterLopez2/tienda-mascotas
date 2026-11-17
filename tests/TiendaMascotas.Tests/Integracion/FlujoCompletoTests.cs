using TiendaMascotas.Core.Dominio;
using TiendaMascotas.Core.Pagos;
using TiendaMascotas.Core.Persistencia;
using TiendaMascotas.Tests.Dobles;
using TiendaMascotas.Tests.Utilidades;

namespace TiendaMascotas.Tests.Integracion;

public class FlujoCompletoTests : IDisposable
{
    private readonly string _archivo = Path.Combine(Path.GetTempPath(), $"pedidos-{Guid.NewGuid()}.jsonl");

    public void Dispose() => File.Delete(_archivo);

    [Fact]
    public void RepositorioArchivo_GuardarYLeer_ConservaTodosLosDatos()
    {
        // Arrange
        var repositorio = new RepositorioPedidosArchivo(_archivo);
        var original = new PedidoProcesado(Guid.NewGuid(), "Camila", 1061.63m, "Tarjeta", Fabrica.FechaFija);

        // Act
        repositorio.Guardar(original);
        var leidos = new RepositorioPedidosArchivo(_archivo).LeerTodos();

        // Assert
        Assert.Equal(original, Assert.Single(leidos));
    }

    [Fact]
    public void Procesar_ConRepositorioEnArchivo_RecuerdaElPedidoEntreReinicios()
    {
        // Arrange
        var pedido = Fabrica.PedidoDeCamila();
        var pasarela = new PasarelaPagoMock();
        Fabrica.Servicio(pasarela: pasarela, repositorio: new RepositorioPedidosArchivo(_archivo))
            .Procesar(pedido, new PagoTarjeta());

        // Act: "reinicio" la app creando todo de nuevo sobre el mismo archivo
        var servicioReiniciado = Fabrica.Servicio(pasarela: pasarela, repositorio: new RepositorioPedidosArchivo(_archivo));
        var resultado = servicioReiniciado.Procesar(pedido, new PagoTarjeta());

        // Assert
        Assert.False(resultado.Exitoso);
        Assert.Equal(1, pasarela.CantidadDeCobros);
    }
}
