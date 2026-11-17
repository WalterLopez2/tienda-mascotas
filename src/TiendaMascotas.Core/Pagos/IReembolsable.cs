namespace TiendaMascotas.Core.Pagos;

public interface IReembolsable
{
    decimal CalcularReembolso(decimal montoCobrado);
}
