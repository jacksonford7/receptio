using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace RECEPTIO.CapaPresentacion.UI.MVVM
{
    public static class BaseXml
    {
        public static string ObtenerDatoDeXml(string xml, string descendiente, string atributo)
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

        public static IEnumerable<XElement> ObtenerEtiquetas(string xml, string descendiente)
        {
            var txtUtf8 = new UTF8Encoding();
            var xmltr = new XmlTextReader(new MemoryStream(txtUtf8.GetBytes(xml)));
            var reader = XDocument.Load(xmltr);
            return reader.Descendants(descendiente);
        }

        public static string ObtenerValorEtiqueta(string xml, string descendiente)
        {
            var etiqueta = ObtenerEtiquetas(xml, descendiente).FirstOrDefault();
            if (etiqueta == null)
                return "";
            else
                return etiqueta.ToString().Replace(descendiente, "").Replace(descendiente, "");
        }
    }
}
