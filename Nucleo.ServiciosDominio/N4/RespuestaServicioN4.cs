using RECEPTIO.CapaDominio.Nucleo.ServiciosDominio.ServicioN4;
using System.Collections.Generic;
using System.Text;

namespace RECEPTIO.CapaDominio.Nucleo.ServiciosDominio.N4
{
    public class RespuestaServicioN4
    {
        public bool RecepcionXmlOk { get; set; }

        public string EstadoRecepcionXml { get; set; }

        public short IdEstado { get; set; }

        public string Estado { get; set; }

        public IEnumerable<MessageType> Mensajes { get; set; }

        public IEnumerable<QueryResultType> ResultadosConsultas { get; set; }

        public override string ToString()
        {
            return $"EstadoRecepcionXml = {EstadoRecepcionXml ?? ""}; Estado = {Estado ?? ""}; Mensajes = {ObtenerMensajes(Mensajes ?? new List<MessageType>())}; ResultadosConsultas = {ObtenerResultadosConsultas(ResultadosConsultas ?? new List<QueryResultType>())}";
        }

        private string ObtenerMensajes(IEnumerable<MessageType> mensajes)
        {
            var resultado = new StringBuilder();
            foreach (var item in mensajes)
                resultado.Append($"SeverityLevel : {item.SeverityLevel}, Message : {item.Message};");
            return resultado.ToString();
        }

        private string ObtenerResultadosConsultas(IEnumerable<QueryResultType> resultadosConsultas)
        {
            var resultado = new StringBuilder();
            foreach (var item in resultadosConsultas)
                resultado.Append($"Result : {item.Result};");
            return resultado.ToString();
        }
    }
}