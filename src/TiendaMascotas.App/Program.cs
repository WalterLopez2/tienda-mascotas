using TiendaMascotas.Legacy;

var procesador = new ProcesadorPedidos();
var datos = new object[]
{
    "Camila", true, "Croquetas;Alimento;250;2|Pelota;Juguete;45;3|Cama;Accesorio;600;1",
    "tarjeta", "HUELLITAS10", "camila@correo.com"
};

Console.WriteLine(procesador.Procesar(datos));
foreach (var linea in procesador.log)
    Console.WriteLine(linea);
