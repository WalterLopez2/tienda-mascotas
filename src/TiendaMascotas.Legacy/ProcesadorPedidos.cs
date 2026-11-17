using System;
using System.Collections.Generic;
using System.IO;

namespace TiendaMascotas.Legacy
{
    // Clase que procesa los pedidos
    public class ProcesadorPedidos
    {
        public List<string> log = new List<string>();
        public string archivo = "pedidos.txt";
        public bool modoViejo = false; // NO TOCAR, lo usaba el sistema anterior

        // Procesa el pedido
        // d[0] = cliente, d[1] = activo, d[2] = items "nombre;categoria;precio;cantidad|...",
        // d[3] = tipo de pago, d[4] = cupon, d[5] = correo
        public string Procesar(object[] d)
        {
            string r = "";
            if (d != null)
            {
                if ((bool)d[1] == true)
                {
                    string[] its = ((string)d[2]).Split('|');
                    if (its.Length > 0 && (string)d[2] != "")
                    {
                        double t = 0;
                        for (int i = 0; i < its.Length; i++)
                        {
                            string[] x = its[i].Split(';');
                            double p = double.Parse(x[2], System.Globalization.CultureInfo.InvariantCulture);
                            int c = int.Parse(x[3]);
                            // aplicar descuento
                            if (x[1] == "Alimento") p = p - p * 0.05;
                            else if (x[1] == "Juguete") p = p - p * 0.1;
                            else if (x[1] == "Accesorio") p = p - p * 0.15;
                            t = t + p * c;
                        }
                        // descuento del 10% para clientes VIP
                        if (t > 1000) t = t - t * 0.1;
                        if (d[4] != null && (string)d[4] == "HUELLITAS10") t = t - t * 0.1;
                        // impuesto
                        t = t + t * 0.15;
                        if (modoViejo) { t = t * 2; }
                        // cobrar
                        if ((string)d[3] == "tarjeta")
                        {
                            t = t + t * 0.03;
                            log.Add("Cobrando con tarjeta " + t);
                        }
                        else if ((string)d[3] == "efectivo")
                        {
                            log.Add("Cobrando en efectivo " + t);
                        }
                        else
                        {
                            return "ERROR";
                        }
                        // guardar
                        File.AppendAllText(archivo, d[0] + "," + t + "," + DateTime.Now + "\n");
                        // enviar correo
                        log.Add("Enviando correo a " + d[5] + ": Su pedido fue procesado. Total: " + t);
                        r = "OK:" + Math.Round(t, 2).ToString(System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else r = "ERROR: sin items";
                }
                else r = "ERROR: cliente inactivo";
            }
            return r;
        }

        // calcula el total (copiado de Procesar para el reporte)
        public double calc2(string items)
        {
            double t = 0;
            string[] its = items.Split('|');
            for (int i = 0; i < its.Length; i++)
            {
                string[] x = its[i].Split(';');
                double p = double.Parse(x[2], System.Globalization.CultureInfo.InvariantCulture);
                int c = int.Parse(x[3]);
                if (x[1] == "Alimento") p = p - p * 0.05;
                else if (x[1] == "Juguete") p = p - p * 0.1;
                else if (x[1] == "Accesorio") p = p - p * 0.15;
                t = t + p * c;
            }
            return t;
        }
    }

    public abstract class MetodoPago
    {
        public abstract double Cobrar(double monto);
        public abstract void Reembolsar(double monto);
    }

    public class PagoTarjeta : MetodoPago
    {
        public override double Cobrar(double monto) => monto * 1.03;
        public override void Reembolsar(double monto) => Console.WriteLine("Reembolso a tarjeta: " + monto);
    }

    public class PagoEfectivo : MetodoPago
    {
        public override double Cobrar(double monto) => monto;

        // El efectivo no se puede reembolsar por sistema... pero la clase base lo obliga
        public override void Reembolsar(double monto) => throw new NotImplementedException();
    }

    public interface INotificador
    {
        void EnviarCorreo(string destino, string mensaje);
        void EnviarSms(string telefono, string mensaje);
        void EnviarWhatsApp(string telefono, string mensaje);
        void ImprimirTicket(string contenido);
    }

    public class NotificadorCorreo : INotificador
    {
        public void EnviarCorreo(string destino, string mensaje) => Console.WriteLine("Correo a " + destino + ": " + mensaje);
        public void EnviarSms(string telefono, string mensaje) => throw new NotImplementedException();
        public void EnviarWhatsApp(string telefono, string mensaje) => throw new NotImplementedException();
        public void ImprimirTicket(string contenido) => throw new NotImplementedException();
    }
}
