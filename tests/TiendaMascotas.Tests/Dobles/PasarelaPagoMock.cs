using TiendaMascotas.Core.Pagos;

namespace TiendaMascotas.Tests.Dobles;

internal sealed class PasarelaPagoMock : IPasarelaPago
{
    private readonly bool _aprobarCobros;
    private readonly List<(Guid PedidoId, decimal Monto)> _cobrosRecibidos = [];
    private (Guid PedidoId, decimal Monto)? _cobroEsperado;

    public PasarelaPagoMock(bool aprobarCobros = true) => _aprobarCobros = aprobarCobros;

    public int CantidadDeCobros => _cobrosRecibidos.Count;

    public PasarelaPagoMock EsperarUnSoloCobro(Guid pedidoId, decimal monto)
    {
        _cobroEsperado = (pedidoId, monto);
        return this;
    }

    public bool Cobrar(Guid pedidoId, decimal monto)
    {
        _cobrosRecibidos.Add((pedidoId, monto));
        return _aprobarCobros;
    }

    public void Verificar()
    {
        if (_cobroEsperado is null)
            throw new InvalidOperationException("El mock no tiene expectativas configuradas.");

        var cobro = Assert.Single(_cobrosRecibidos);
        Assert.Equal(_cobroEsperado.Value, cobro);
    }
}
