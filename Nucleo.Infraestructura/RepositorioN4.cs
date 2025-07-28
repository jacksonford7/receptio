using RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio;
using RECEPTIO.CapaInfraestructura.Nucleo.Infraestructura.Modelo;
using System;
using System.Linq;

namespace RECEPTIO.CapaInfraestructura.Nucleo.Infraestructura
{
    public class RepositorioN4 : IRepositorioN4
    {
        public bool TieneTransaccionActiva(string gosTvKey)
        {
            using (var contexto = new ModeloReceptioContainer())
            {
                var resultado = contexto.mb_get_status_n4(Convert.ToDecimal(gosTvKey)).FirstOrDefault();
                return resultado != null && resultado != "CANCEL";
            }
        }

        public bool TieneTransaccionActivaPorPlaca(string placa)
        {
            using (var contexto = new ModeloReceptioContainer())
            {
                var resultado = contexto.mb_get_status_n4_placa(placa).FirstOrDefault();
                return resultado != null && resultado != "CANCEL";
            }
        }

        public bool TieneTransaccionActivaDepotPorPlaca(string placa)
        {
            using (var contexto = new ModeloReceptioContainer())
            {
                var resultado = contexto.mb_get_status_n4_placa(placa).FirstOrDefault();
                return resultado == "TROUBLE" || resultado == "OK";
            }
        }

        public bool EstaEnPatioContenedor(string contenedor)
        {
            using (var contexto = new ModeloReceptioContainer())
            {
                var resultado = contexto.mb_get_gkey_cont_yard(contenedor).FirstOrDefault();
                return resultado != null && resultado.HasValue;
            }
        }
    }
}


