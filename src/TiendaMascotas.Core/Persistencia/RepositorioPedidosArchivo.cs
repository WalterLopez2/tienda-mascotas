using System.Text.Json;
using TiendaMascotas.Core.Dominio;

namespace TiendaMascotas.Core.Persistencia;

/// <summary>Única responsabilidad: guardar y leer pedidos procesados en un archivo JSON Lines.</summary>
public sealed class RepositorioPedidosArchivo : IRepositorioPedidos
{
    private readonly string _ruta;

    public RepositorioPedidosArchivo(string ruta) => _ruta = ruta;

    public void Guardar(PedidoProcesado pedido) =>
        File.AppendAllText(_ruta, JsonSerializer.Serialize(pedido) + Environment.NewLine);

    public IReadOnlyList<PedidoProcesado> LeerTodos()
    {
        if (!File.Exists(_ruta)) return [];

        return File.ReadLines(_ruta)
            .Where(linea => !string.IsNullOrWhiteSpace(linea))
            .Select(linea => JsonSerializer.Deserialize<PedidoProcesado>(linea)!)
            .ToList();
    }
}
