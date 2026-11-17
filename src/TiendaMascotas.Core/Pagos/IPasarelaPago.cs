namespace TiendaMascotas.Core.Pagos;

/// <summary>El sistema externo que realmente mueve el dinero (banco, procesador de tarjetas).</summary>
public interface IPasarelaPago
{
    bool Cobrar(Guid pedidoId, decimal monto);
}
