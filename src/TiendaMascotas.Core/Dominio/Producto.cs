namespace TiendaMascotas.Core.Dominio;

public sealed record Producto
{
    public string Nombre { get; }
    public Categoria Categoria { get; }
    public decimal Precio { get; }

    public Producto(string nombre, Categoria categoria, decimal precio)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El producto necesita un nombre.", nameof(nombre));
        if (precio < 0)
            throw new ArgumentOutOfRangeException(nameof(precio), "El precio no puede ser negativo.");

        Nombre = nombre;
        Categoria = categoria;
        Precio = precio;
    }
}
