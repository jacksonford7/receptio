using RECEPTIO.CapaPresentacion.UI.Interfaces.Impresora;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ZebraPrinter;

namespace RECEPTIO.CapaPresentacion.UI.ImpresoraZebra
{
    public class EstadoImpresora : IEstadoImpresora
    {
        private ProcesoObtencionEstadoImpresora _estado;

        public Tuple<List<string>, List<string>> VerEstado()
        {
            _estado = new Coneccion();
            return _estado.Procesar(this, null);
        }

        internal Tuple<List<string>, List<string>> SetearEstado(ProcesoObtencionEstadoImpresora estado, object objeto)
        {
            _estado = estado;
            return _estado.Procesar(this, objeto);
        }
    }

    internal abstract class ProcesoObtencionEstadoImpresora
    {
        internal abstract Tuple<List<string>, List<string>> Procesar(EstadoImpresora claseBase, object objeto);
    }

    internal class Coneccion : ProcesoObtencionEstadoImpresora
    {
        internal override Tuple<List<string>, List<string>> Procesar(EstadoImpresora claseBase, object objeto)
        {
            var enumDevices = UsbPrinterConnector.EnumDevices();
            if (enumDevices.Keys.Count <= 0)
                return new Tuple<List<string>, List<string>>(new List<string> { TablaErroresImpresoraZebra.NoConectada }, new List<string>());
            var key = enumDevices.Keys[0];
            var connector = new UsbPrinterConnector(key);
            var buffer = Encoding.ASCII.GetBytes("~HQES");
            connector.IsConnected = true;
            connector.Send(buffer);
            var miBuffer = new byte[512];
            connector.Read(miBuffer, 0, 512);
            connector.Dispose();
            if (miBuffer.ToList().All(b => b == 0))
                return Procesar(claseBase, objeto);
            return claseBase.SetearEstado(new FiltrarBits(), miBuffer);
        }
    }

    internal class FiltrarBits : ProcesoObtencionEstadoImpresora
    {
        internal override Tuple<List<string>, List<string>> Procesar(EstadoImpresora claseBase, object objeto)
        {
            var miBuffer = new byte[512];
            miBuffer = (byte[])objeto;
            if (ConveritrAsciiACaracter(miBuffer[66]) == "0" && ConveritrAsciiACaracter(miBuffer[108]) == "0")
                return new Tuple<List<string>, List<string>>(new List<string>(), new List<string>());
            var listaCodigoErroresYAdvertencias = new List<string>();
            for (var i = 80; i < 85; i++)
                listaCodigoErroresYAdvertencias.Add(ConveritrAsciiACaracter(miBuffer[i]));
            for (var i = 124; i < 127; i++)
                listaCodigoErroresYAdvertencias.Add(ConveritrAsciiACaracter(miBuffer[i]));
            return claseBase.SetearEstado(new ObtenerMensajes(), listaCodigoErroresYAdvertencias);
        }

        private string ConveritrAsciiACaracter(byte caracter)
        {
            char character = (char)caracter;
            return character.ToString();
        }
    }

    internal class ObtenerMensajes : ProcesoObtencionEstadoImpresora
    {
        internal override Tuple<List<string>, List<string>> Procesar(EstadoImpresora claseBase, object objeto)
        {
            var listaCodigoErroresYAdvertencias = new List<string>();
            listaCodigoErroresYAdvertencias = (List<string>)objeto;
            var listaMensajesErrores = new List<string>();
            var listaMensajesAdvertencias = new List<string>();
            var arregloCodigoErroresYAdvertencias = listaCodigoErroresYAdvertencias.ToArray();
            for (var i = 0; i < 5; i++)
            {
                if (arregloCodigoErroresYAdvertencias[i] != "0" && arregloCodigoErroresYAdvertencias[i] != "\0")
                    listaMensajesErrores.AddRange(ObtenerMensajesError(5 - i, arregloCodigoErroresYAdvertencias[i], 8));
            }
            for (var i = 5; i < 8; i++)
            {
                if (arregloCodigoErroresYAdvertencias[i] != "0" && arregloCodigoErroresYAdvertencias[i] != "\0")
                    listaMensajesAdvertencias.AddRange(ObtenerMensajesAdvertencias(8 - i, arregloCodigoErroresYAdvertencias[i], 8));
            }
            return new Tuple<List<string>, List<string>>(listaMensajesErrores, listaMensajesAdvertencias);
        }

        private IEnumerable<string> ObtenerMensajesError(int nibble, string caracter, int divisor)
        {
            var mensajesErrores = new List<string>();
            var valorDecimal = Convert.ToInt32(caracter, 16);
            int resultadoEnteroDivision = valorDecimal / divisor;
            int residuoDivision = valorDecimal % divisor;
            if (resultadoEnteroDivision == 1)
                mensajesErrores.Add(ObtenerMensajeError(nibble, divisor));
            if (residuoDivision == 0)
                return mensajesErrores;
            else
            {
                mensajesErrores.AddRange(ObtenerMensajesError(nibble, residuoDivision.ToString(), (divisor / 2)));
                return mensajesErrores;
            }
        }

        private string ObtenerMensajeError(int nibble, int divisor)
        {
            var myPropertyInfo = typeof(TablaErroresImpresoraZebra).GetRuntimeProperties();
            var propiedad = myPropertyInfo.FirstOrDefault(p => p.Name == $"Nibble{nibble}_{divisor}");
            return propiedad.GetValue(null).ToString();
        }

        private IEnumerable<string> ObtenerMensajesAdvertencias(int nibble, string caracter, int divisor)
        {
            var mensajesAdvertencias = new List<string>();
            var valorDecimal = Convert.ToInt32(caracter, 16);
            int resultadoEnteroDivision = valorDecimal / divisor;
            int residuoDivision = valorDecimal % divisor;
            if (resultadoEnteroDivision == 1)
                mensajesAdvertencias.Add(ObtenerMensajeAdvertencia(nibble, divisor));
            if (residuoDivision == 0)
                return mensajesAdvertencias;
            else
            {
                mensajesAdvertencias.AddRange(ObtenerMensajesAdvertencias(nibble, residuoDivision.ToString(), (divisor / 2)));
                return mensajesAdvertencias;
            }
        }

        private string ObtenerMensajeAdvertencia(int nibble, int divisor)
        {
            var myPropertyInfo = typeof(TablaAdvertenciasImpresoraZebra).GetRuntimeProperties();
            var propiedad = myPropertyInfo.FirstOrDefault(p => p.Name == $"Nibble{nibble}_{divisor}");
            return propiedad.GetValue(null).ToString();
        }
    }
}
