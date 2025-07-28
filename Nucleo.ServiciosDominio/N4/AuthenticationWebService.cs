using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using RECEPTIO.CapaDominio.Nucleo.ServiciosDominio.ServicioN4;
using System.Collections.Generic;

namespace RECEPTIO.CapaDominio.Nucleo.ServiciosDominio.N4
{
    public class AuthenticationWebService : ArgoService, IConector
    {
        private readonly string _b64Credenciales;
        private readonly string _coordenadas;

        protected override System.Net.WebRequest GetWebRequest(Uri uri)
        {
            var httpRequest = (System.Net.HttpWebRequest)base.GetWebRequest(uri);
            httpRequest.Headers.Add("Authorization", $"Basic {_b64Credenciales}");
            httpRequest.ServicePoint.MaxIdleTime = 10000;
            return httpRequest;
        }

        public AuthenticationWebService()
        {
            var bCredentials = Encoding.ASCII.GetBytes($"{ConfigurationManager.AppSettings["UserNameServicio"]}:{ConfigurationManager.AppSettings["PasswordServicio"]}");
            _b64Credenciales = Convert.ToBase64String(bCredentials);
            _coordenadas = ConfigurationManager.AppSettings["scopeCoordinateIds"];
        }

        public RespuestaServicioN4 Invocacion(string xml)
        {
            var respuesta =  genericInvoke(new genericInvoke
            {
                scopeCoordinateIdsWsType = new ScopeCoordinateIdsWsType
                {
                    complexId = _coordenadas.Split('/')[1],
                    externalUserId = "",
                    facilityId = _coordenadas.Split('/')[2],
                    operatorId = _coordenadas.Split('/')[0],
                    yardId = _coordenadas.Split('/')[3]
                },
                xmlDoc = xml
            }).genericInvokeResponse1;
            return DecodificarRespuesta(respuesta);
        }

        private RespuestaServicioN4 DecodificarRespuesta(GenericInvokeResponseWsType respuesta)
        {
            return new RespuestaServicioN4
            {
                EstadoRecepcionXml = ObtenerDatoDeXml(respuesta.responsePayLoad, "document-update", "status"),
                RecepcionXmlOk = ObtenerDatoDeXml(respuesta.responsePayLoad, "document-update", "status") == "ACCEPTED",
                IdEstado = Convert.ToInt16(respuesta.commonResponse.Status),
                Estado = respuesta.commonResponse.StatusDescription,
                Mensajes = respuesta.commonResponse.MessageCollector.ToList(),
                ResultadosConsultas = respuesta.commonResponse.QueryResults == null ? new List<QueryResultType>() : respuesta.commonResponse.QueryResults.ToList()
            };
        }

        private string ObtenerDatoDeXml(string xml, string descendiente, string atributo)
        {
            var txtUtf8 = new UTF8Encoding();
            var xmltr = new XmlTextReader(new MemoryStream(txtUtf8.GetBytes(xml)));
            var reader = XDocument.Load(xmltr);
            var valor = "";
            foreach (var xAttribute in reader.Descendants(descendiente).Select(reg => reg.Attributes(atributo).FirstOrDefault()))
            {
                if (xAttribute != null)
                    valor = xAttribute.Value;
                break;
            }
            return valor;
        }
    }
}
