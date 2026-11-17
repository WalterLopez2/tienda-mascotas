namespace TiendaMascotas.Core.Precios;

/// <summary>Un cupón sabe su porcentaje y hasta cuándo vale (null = no vence).</summary>
public sealed record Cupon(string Codigo, decimal Porcentaje, DateOnly? VenceEl = null)
{
    public bool EstaVigenteEl(DateTime fecha) =>
        VenceEl is null || DateOnly.FromDateTime(fecha) <= VenceEl;
}
