using RECEPTIO.CapaAplicacion.Transaction.Aplicacion.Contratos;
using RECEPTIO.CapaAplicacion.Transaction.Aplicacion.Interfaces;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaDominio.Nucleo.ServiciosDominio.N4;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace RECEPTIO.CapaDominio.Transaction.ServiciosDominio
{
    public class ServicioN4 : IProcesosN4
    {
        private readonly IRepositorioTransaccionQuiosco _repositorioTransaccionQuiosco;
        private readonly IRepositorioPreGate _repositorioPreGate;
        private EstadoN4 _estado;
        internal readonly IConector Conector;
        internal readonly IRepositorioValidaAduana RepositorioValidaAduana;
        internal DatosEntradaN4 DatosEntrada;
        internal DatosPreGateSalida DatosSalida;

        public ServicioN4(IConector conector, IRepositorioTransaccionQuiosco repositorioTransaccionQuiosco, IRepositorioPreGate repositorioPreGate, IRepositorioValidaAduana repositorioValidaAduana) => (Conector, _repositorioTransaccionQuiosco, _repositorioPreGate, RepositorioValidaAduana) = (conector, repositorioTransaccionQuiosco, repositorioPreGate, repositorioValidaAduana);

        public DatosN4 EjecutarProcesosEntrada(DatosEntradaN4 datos)
        {
            DatosEntrada = datos;
            _estado = new EntradaDeliveryImportFullBrBkReceiveExportFullDrayOffRecepcionVacios();
            return _estado.Ejecutar(this);
        }

        public DatosN4 EjecutarProcesosSalida(DatosPreGateSalida datos)
        {
            DatosSalida = datos;
            _estado = new SalidaDeliveryImportFullBrBkDrayOff();
            return _estado.Ejecutar(this);
        }

        internal DatosN4 SetearEstado(EstadoN4 estado)
        {
            _estado = estado;
            return _estado.Ejecutar(this);
        }

        internal DatosN4 GrabarRegistroProcesoEntrada(bool fueOk, int idMensaje, string respuesta)
        {
            var servicioTransaccionQuiosco = new ServicioTransaccionQuiosco(_repositorioTransaccionQuiosco, _repositorioPreGate);
            servicioTransaccionQuiosco.RegistrarProceso(new KIOSK_TRANSACTION
            {
                TRANSACTION_ID = Convert.ToInt32(DatosEntrada.IdTransaccion),
                PRE_GATE_ID = DatosEntrada.IdPreGate,
                PROCESSES = new List<PROCESS> { new PROCESS
                {
                    STEP = "PROCESO_N4",
                    IS_OK = fueOk,
                    MESSAGE_ID = idMensaje,
                    RESPONSE = respuesta
                } },
                KIOSK = new KIOSK { IS_IN = true}
            });
            servicioTransaccionQuiosco.LiberarRecursos();
            return new DatosN4 { FueOk = fueOk, Mensaje = idMensaje.ToString(), Xml = respuesta };
        }

        internal DatosN4 GrabarRegistroProcesoSalida(bool fueOk, int idMensaje, string respuesta)
        {
            var servicioTransaccionQuiosco = new ServicioTransaccionQuiosco(_repositorioTransaccionQuiosco, _repositorioPreGate);
            servicioTransaccionQuiosco.RegistrarProceso(new KIOSK_TRANSACTION
            {
                TRANSACTION_ID = Convert.ToInt32(DatosSalida.IdTransaccion),
                PRE_GATE_ID = DatosSalida.PreGate.PRE_GATE_ID,
                PROCESSES = new List<PROCESS> { new PROCESS
                {
                    STEP = "PROCESO_N4",
                    IS_OK = fueOk,
                    MESSAGE_ID = idMensaje,
                    RESPONSE = respuesta
                } },
                KIOSK = new KIOSK { IS_IN = false }
            });
            servicioTransaccionQuiosco.LiberarRecursos();
            return new DatosN4 { FueOk = fueOk, Mensaje = idMensaje.ToString(), Xml = respuesta };
        }

        internal DatosN4 GrabarRegistroProcesoSalida(string step ,bool fueOk, int idMensaje, string respuesta)
        {
            var servicioTransaccionQuiosco = new ServicioTransaccionQuiosco(_repositorioTransaccionQuiosco, _repositorioPreGate);
            servicioTransaccionQuiosco.RegistrarProceso(new KIOSK_TRANSACTION
            {
                TRANSACTION_ID = Convert.ToInt32(DatosSalida.IdTransaccion),
                PRE_GATE_ID = DatosSalida.PreGate.PRE_GATE_ID,
                PROCESSES = new List<PROCESS> { new PROCESS
                {
                    STEP = step,
                    IS_OK = fueOk,
                    MESSAGE_ID = idMensaje,
                    RESPONSE = respuesta
                } },
                KIOSK = new KIOSK { IS_IN = false }
            });
            return new DatosN4 { FueOk = fueOk, Mensaje = idMensaje.ToString(), Xml = respuesta };
        }
    }

    public abstract class EstadoN4
    {
        internal abstract DatosN4 Ejecutar(ServicioN4 servicio);
    }

    public class EntradaDeliveryImportFullBrBkReceiveExportFullDrayOffRecepcionVacios : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            if (servicio.DatosEntrada.TipoTransaccion == 1 || servicio.DatosEntrada.TipoTransaccion == 2 || servicio.DatosEntrada.TipoTransaccion == 3 || servicio.DatosEntrada.TipoTransaccion == 6 || servicio.DatosEntrada.TipoTransaccion == 7 || servicio.DatosEntrada.TipoTransaccion == 10 || servicio.DatosEntrada.TipoTransaccion == 18 || servicio.DatosEntrada.TipoTransaccion == 19)
                return servicio.SetearEstado(new EstadoProcessTruckFull());
            else
                return servicio.SetearEstado(new EntradaReceiveExportBrBkCfs());
        }
    }

    public class EntradaReceiveExportBrBkCfs : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            if (servicio.DatosEntrada.TipoTransaccion == 4 || servicio.DatosEntrada.TipoTransaccion == 5)
                return servicio.SetearEstado(new EstadoGroovyPesajeEntrada());
            else
                return servicio.SetearEstado(new EntradaReceiveExportBanano());
        }
    }

    public class EntradaReceiveExportBanano : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            if (servicio.DatosEntrada.TipoTransaccion == 8)
                return servicio.SetearEstado(new EstadoGroovyPesajeEntradaBanano());
            else
                return servicio.SetearEstado(new EntradaReceiveExportBananoCfs());
        }
    }

    public class EntradaReceiveExportBananoCfs : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            if (servicio.DatosEntrada.TipoTransaccion == 9)
                return servicio.SetearEstado(new EstadoProcessTruckFullBanano());
            else
                return servicio.SetearEstado(new EntradaReceiveImportMTYBooking());
        }
    }

    //<JGUSQUI 2019-03-01>
    public class EntradaReceiveImportMTYBooking : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            if (servicio.DatosEntrada.TipoTransaccion == 16)
                return servicio.SetearEstado(new EstadoProcessTruckVaciosSAV());
            else
                return servicio.SetearEstado(new EntradaEntregaVacioYProveedores());
        }
    }
    //</JGUSQUI 2019-03-01>

    public class EntradaEntregaVacioYProveedores : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            if (servicio.DatosEntrada.TipoTransaccion == 11 || servicio.DatosEntrada.TipoTransaccion == 12 || servicio.DatosEntrada.TipoTransaccion == 13 || servicio.DatosEntrada.TipoTransaccion == 14 || servicio.DatosEntrada.TipoTransaccion == 15 || servicio.DatosEntrada.TipoTransaccion == 17)
                return servicio.GrabarRegistroProcesoEntrada(true, 13, "");
            else
                return servicio.GrabarRegistroProcesoEntrada(false, 24, "");
        }
    }

    public class EstadoGroovyPesajeEntradaBanano : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            var xml = ArmarXml(servicio.DatosEntrada);
            var respuesta = servicio.Conector.Invocacion(xml);
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("ERROR")))
                return servicio.SetearEstado(new EstadoProcessTruckFullBanano());
            return servicio.GrabarRegistroProcesoEntrada(false, 14, $"N4 RETORNO ERROR EN GROOVY :{respuesta.ToString()}");
        }

        public static string ArmarXml(DatosEntradaN4 datos)
        {
            var fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return $@"<groovy class-name=""CGSATrkTranWeightWS"" class-location=""code-extension"">
                        <parameters>
                            <parameter id=""tt-key"" value=""{datos.IdPreGate * -1}""/>
                            <parameter id=""update-bl"" value=""Y""/>
                            <parameter id=""direccion"" value=""IN""/>
                            <parameter id=""kiosco"" value=""{datos.NombreQuiosco}""/>
                            <parameter id=""fecha"" value=""{fecha}""/>
                            <parameter id=""weight"" value=""{datos.Peso}""/>
                        </parameters>
                     </groovy>";
        }
    }

    public class EstadoProcessTruckFullBanano : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            var xml = ArmarXml(servicio.DatosEntrada);
            var respuesta = servicio.Conector.Invocacion(xml);
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
                return servicio.GrabarRegistroProcesoEntrada(true, 13, respuesta.ResultadosConsultas.FirstOrDefault().Result);
            return servicio.GrabarRegistroProcesoEntrada(false, 14, $"N4 RETORNO ERROR EN PROCESS TRUCK :{respuesta.ToString()}");
        }

        public static string ArmarXml(DatosEntradaN4 datos)
        {
            var fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            return $@"<gate>
    	                <process-truck scan-status=""0"" no-content=""false"">
                            <gate-id>RECEPTIO_BAN</gate-id>
		                    <stage-id>banano_in</stage-id>
                            <lane-id>{datos.NombreQuiosco}</lane-id>
		                    <truck license-nbr=""{datos.PlacaVehiculo}""/>
                            <driver license-nbr=""{datos.CedulaChofer}""/>
                            <truck-visit gos-tv-key=""{(datos.IdPreGate * -1)}""/>
                            <timestamp>{fecha}</timestamp>
                        </process-truck>
                      </gate>";
        }
    }

    public class EstadoGroovyPesajeEntrada : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            var xml = ArmarXml(servicio.DatosEntrada);
            var respuesta = servicio.Conector.Invocacion(xml);
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("ERROR")))
                return servicio.SetearEstado(new EstadoProcessTruckFull());
            return servicio.GrabarRegistroProcesoEntrada(false, 14, $"N4 RETORNO ERROR EN GROOVY :{respuesta.ToString()}");
        }

        public static string ArmarXml(DatosEntradaN4 datos)
        {
            var fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return $@"<groovy class-name=""CGSATrkTranWeightWS"" class-location=""code-extension"">
                        <parameters>
                            <parameter id=""tt-key"" value=""{datos.IdPreGate * -1}""/>
                            <parameter id=""update-bl"" value=""Y""/>
                            <parameter id=""direccion"" value=""IN""/>
                            <parameter id=""kiosco"" value=""{datos.NombreQuiosco}""/>
                            <parameter id=""fecha"" value=""{fecha}""/>
                            <parameter id=""weight"" value=""{datos.Peso}""/>
                        </parameters>
                     </groovy>";
        }
    }

    public class EstadoProcessTruckFull : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            var xml = ArmarXml(servicio.DatosEntrada);
            var respuesta = servicio.Conector.Invocacion(xml);
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
                return servicio.GrabarRegistroProcesoEntrada(true, 13, respuesta.ResultadosConsultas.FirstOrDefault().Result);
            return servicio.GrabarRegistroProcesoEntrada(false, 14, $"N4 RETORNO ERROR EN PROCESS TRUCK :{respuesta.ToString()}");
        }

        public static string ArmarXml(DatosEntradaN4 datos)
        {
            var fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            return $@"<gate>
    	                <process-truck scan-status=""0"" no-content=""false"">
                            <gate-id>RECEPTIO</gate-id>
		                    <stage-id>ingate</stage-id>
                            <lane-id>{datos.NombreQuiosco}</lane-id>
		                    <truck license-nbr=""{datos.PlacaVehiculo}""/>
                            <driver license-nbr=""{datos.CedulaChofer}""/>
                            <truck-visit gos-tv-key=""{(datos.IdPreGate * -1)}""/>
                            <timestamp>{fecha}</timestamp>
                        </process-truck>
                      </gate>";
        }
    }

    public class SalidaDeliveryImportFullBrBkDrayOff : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            if (servicio.DatosSalida.TipoTransaccion == 1 || servicio.DatosSalida.TipoTransaccion == 2 || servicio.DatosSalida.TipoTransaccion == 6 || servicio.DatosSalida.TipoTransaccion == 18)
                return servicio.SetearEstado(new EstadoSalidaProcessTruckFull());
            else
                return servicio.SetearEstado(new SalidaReceiveExportFull());
        }
    }

    public class SalidaReceiveExportFull : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            if (servicio.DatosSalida.TipoTransaccion == 3 || servicio.DatosSalida.TipoTransaccion == 19)
                return servicio.SetearEstado(new EstadoIieExportFull());
            else
                return servicio.SetearEstado(new SalidaReceiveExportBrBk());
        }
    }

    public class SalidaReceiveExportBrBk : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            if (servicio.DatosSalida.TipoTransaccion == 4)
                return servicio.SetearEstado(new EstadoIieExportBrBk());
            else
                return servicio.SetearEstado(new SalidaReceiveExportCfs());
        }
    }

    public class SalidaReceiveExportCfs : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            if (servicio.DatosSalida.TipoTransaccion == 5)
                return servicio.SetearEstado(new EstadoGroovyPesajeSalida());
            else
                return servicio.SetearEstado(new SalidaReceiveEmpty());
        }
    }

    public class SalidaReceiveEmpty : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            if (servicio.DatosSalida.TipoTransaccion == 7)
                return servicio.SetearEstado(new EstadoImdt());
            else
                return servicio.SetearEstado(new SalidaReceiveEmptyMix());
        }
    }

    public class SalidaReceiveEmptyMix : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            if (servicio.DatosSalida.TipoTransaccion == 10)
                return servicio.SetearEstado(new EstadoImdtMix());
            else
                return servicio.SetearEstado(new SalidaReceiveBanano());
        }
    }

    internal class EstadoImdtMix : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            foreach (var detalle in servicio.DatosSalida.PreGate.PRE_GATE_DETAILS.Where(pgd => pgd.REFERENCE_ID == "E"))
            {
                var xml = ArmarXml(servicio.DatosSalida.NombreQuiosco, servicio.RepositorioValidaAduana.ObtenerGKeyContenedor(detalle.CONTAINERS.FirstOrDefault().NUMBER));
                var servicioWeb = new ServicioImdt.n4ServiceSoapClient();
                var respuesta = servicioWeb.basicInvoke(ConfigurationManager.AppSettings["scopeCoordinateIds"], xml);
                if (!respuesta.Contains(@"Code>0</Code"))
                    return servicio.GrabarRegistroProcesoSalida(false, 14, $"SERVICIO IMDT RETORNO : {respuesta.ToString()}");
            }
            return servicio.SetearEstado(new EstadoSalidaProcessTruckFull());
        }

        public static string ArmarXml(string nombreQuiosco, long gkey)
        {
            return $@"<imdt><tipo>E</tipo><usuario>{nombreQuiosco}</usuario><validar>0</validar><parametros><gkey>{gkey}</gkey></parametros></imdt>";
        }
    }

    public class SalidaReceiveBanano : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            if (servicio.DatosSalida.TipoTransaccion == 8)
                return servicio.SetearEstado(new EstadoIieBanano());
            else
                return servicio.SetearEstado(new SalidaReceiveBananoCfs());
        }
    }

    public class SalidaReceiveBananoCfs : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            if (servicio.DatosSalida.TipoTransaccion == 9)
                return servicio.SetearEstado(new EstadoSalidaProcessTruckFullBanano());
            else
                return servicio.SetearEstado(new SalidaDeliveryEmpty());
        }
    }

    public class SalidaDeliveryEmpty : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            if (servicio.DatosSalida.TipoTransaccion == 11)
                return servicio.SetearEstado(new EstadoSalidaProcessTruckDeliveryEmpty());
            else
                return servicio.SetearEstado(new SalidaReceiveImportMTYBooking());
        }
    }

    public class SalidaReceiveImportMTYBooking : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            if (servicio.DatosSalida.TipoTransaccion == 16)
                return servicio.SetearEstado(new EstadoSalidaProcessTruckVaciosSAV());
            else
                return servicio.SetearEstado(new SalidaDeliveryImportEmptyDEPOT());
        }
    }

    public class SalidaDeliveryImportEmptyDEPOT : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            if (servicio.DatosSalida.TipoTransaccion == 17)
                return servicio.SetearEstado(new EstadoICU());
            else
                return servicio.SetearEstado(new EstadoSalidaProveedores());
        }
    }

    public class EstadoSalidaProveedores : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            if (servicio.DatosSalida.TipoTransaccion == 12 || servicio.DatosSalida.TipoTransaccion == 13 || servicio.DatosSalida.TipoTransaccion == 14 || servicio.DatosSalida.TipoTransaccion == 15)
                return servicio.GrabarRegistroProcesoSalida(true, 13, "");
            else
                return servicio.GrabarRegistroProcesoSalida(false, 24, "");
        }
    }

    public class EstadoSalidaProcessTruckDeliveryEmpty : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            var xml = ArmarXml(servicio.DatosSalida);
            var respuesta = servicio.Conector.Invocacion(xml);
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
                return servicio.GrabarRegistroProcesoSalida(true, 13, respuesta.ResultadosConsultas.FirstOrDefault().Result);
            return servicio.GrabarRegistroProcesoSalida(false, 14, $"N4 RETORNO ERROR EN PROCESS TRUCK :{respuesta.ToString()}");
        }

        public static string ArmarXml(DatosPreGateSalida datos)
        {
            var fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            return $@"<gate>
    	                <process-truck scan-status=""0"" no-content=""false"">
                            <gate-id>RECEPTIO_MTY</gate-id>
		                    <stage-id>mty_out</stage-id>
                            <lane-id>{datos.NombreQuiosco}</lane-id>
		                    <truck license-nbr=""{datos.PreGate.TRUCK_LICENCE}""/>
                            <driver license-nbr=""{datos.PreGate.DRIVER_ID}""/>
                            <truck-visit gos-tv-key=""{(datos.PreGate.PRE_GATE_ID * -1)}""/>
                            <timestamp>{fecha}</timestamp>
                        </process-truck>
                      </gate>";
        }
    }

    public class EstadoIieBanano : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            var xml = ArmarXml(servicio.DatosSalida.NombreQuiosco, servicio.DatosSalida.PreGate.PRE_GATE_DETAILS.FirstOrDefault().TRANSACTION_NUMBER);
            var servicioWeb = new ServicioIIE.n4ServiceSoapClient();
            var respuesta = servicioWeb.basicInvoke(ConfigurationManager.AppSettings["scopeCoordinateIds"], xml);
            if (!respuesta.Contains(@"Code>0</Code"))
                return servicio.GrabarRegistroProcesoSalida(false, 14, $"SERVICIO IIE RETORNO : {respuesta.ToString()}");
            return servicio.SetearEstado(new EstadoSalidaProcessTruckFullBanano());
        }

        public static string ArmarXml(string nombreQuiosco, string aisv)
        {
            return $@"<iie><tipo>B</tipo><usuario>{nombreQuiosco}</usuario><validar>1</validar><parametros><aisv>{aisv}</aisv><descripcion>BANANO</descripcion><embalaje>035</embalaje><peso>1</peso><cantidad>1</cantidad><documento>{aisv}</documento></parametros></iie>";
        }
    }

    public class EstadoSalidaProcessTruckFullBanano : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            var xml = ArmarXml(servicio.DatosSalida);
            var respuesta = servicio.Conector.Invocacion(xml);
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
                return servicio.GrabarRegistroProcesoSalida(true, 13, respuesta.ResultadosConsultas.FirstOrDefault().Result);
            return servicio.GrabarRegistroProcesoSalida(false, 14, $"N4 RETORNO ERROR EN PROCESS TRUCK :{respuesta.ToString()}");
        }

        public static string ArmarXml(DatosPreGateSalida datos)
        {
            var fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            return $@"<gate>
    	                <process-truck scan-status=""0"" no-content=""false"">
                            <gate-id>RECEPTIO_BAN</gate-id>
		                    <stage-id>banano_out</stage-id>
                            <lane-id>{datos.NombreQuiosco}</lane-id>
		                    <truck license-nbr=""{datos.PreGate.TRUCK_LICENCE}""/>
                            <driver license-nbr=""{datos.PreGate.DRIVER_ID}""/>
                            <truck-visit gos-tv-key=""{(datos.PreGate.PRE_GATE_ID * -1)}""/>
                            <timestamp>{fecha}</timestamp>
                        </process-truck>
                      </gate>";
        }
    }

    public class EstadoImdt : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            foreach (var detalle in servicio.DatosSalida.PreGate.PRE_GATE_DETAILS)
            {
                var xml = ArmarXml(servicio.DatosSalida.NombreQuiosco, servicio.RepositorioValidaAduana.ObtenerGKeyContenedor(detalle.CONTAINERS.FirstOrDefault().NUMBER));
                var servicioWeb = new ServicioImdt.n4ServiceSoapClient();
                var respuesta = servicioWeb.basicInvoke(ConfigurationManager.AppSettings["scopeCoordinateIds"], xml);
                if (!respuesta.Contains(@"Code>0</Code"))
                    return servicio.GrabarRegistroProcesoSalida(false, 14, $"SERVICIO IMDT RETORNO : {respuesta.ToString()}");
            }
            return servicio.SetearEstado(new EstadoSalidaProcessTruckFull());
        }

        public static string ArmarXml(string nombreQuiosco, long gkey)
        {
            return $@"<imdt><tipo>E</tipo><usuario>{nombreQuiosco}</usuario><validar>0</validar><parametros><gkey>{gkey}</gkey></parametros></imdt>";
        }
    }

    public class EstadoGroovyPesajeSalida : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            var xml = ArmarXml(servicio.DatosSalida);
            var respuesta = servicio.Conector.Invocacion(xml);
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("ERROR")))
                return servicio.SetearEstado(new EstadoSalidaProcessTruckFull());
            return servicio.GrabarRegistroProcesoSalida(false, 14, $"N4 RETORNO ERROR EN GROOVY :{respuesta.ToString()}");
        }

        public static string ArmarXml(DatosPreGateSalida datos)
        {
            var fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return $@"<groovy class-name=""CGSATrkTranWeightWS"" class-location=""code-extension"">
                        <parameters>
                            <parameter id=""tt-key"" value=""{datos.PreGate.PRE_GATE_ID * -1}""/>
                            <parameter id=""update-bl"" value=""Y""/>
                            <parameter id=""direccion"" value=""OUT""/>
                            <parameter id=""kiosco"" value=""{datos.NombreQuiosco}""/>
                            <parameter id=""fecha"" value=""{fecha}""/>
                            <parameter id=""weight"" value=""{datos.Peso}""/>
                        </parameters>
                     </groovy>";
        }
    }

    public class EstadoIieExportFull : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            foreach (var detalle in servicio.DatosSalida.PreGate.PRE_GATE_DETAILS)
            {
                var xml = ArmarXml(servicio.DatosSalida.NombreQuiosco, servicio.RepositorioValidaAduana.ObtenerGKeyContenedor(detalle.CONTAINERS.FirstOrDefault().NUMBER));
                var servicioWeb = new ServicioIIE.n4ServiceSoapClient();
                var respuesta = servicioWeb.basicInvoke(ConfigurationManager.AppSettings["scopeCoordinateIds"], xml);
                if (!respuesta.Contains(@"Code>0</Code"))
                    return servicio.GrabarRegistroProcesoSalida(false, 14, $"SERVICIO IIE RETORNO : {respuesta.ToString()}");
            }
            return servicio.SetearEstado(new EstadoSalidaProcessTruckFull());
        }

        public static string ArmarXml(string nombreQuiosco, long gkey)
        {
            return $@"<iie><tipo>C</tipo><usuario>{nombreQuiosco}</usuario><validar>1</validar><parametros><gkey>{gkey}</gkey><peso>1</peso></parametros></iie>";
        }
    }

    public class EstadoIieExportBrBk : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            var xml = ArmarXml(servicio.DatosSalida.NombreQuiosco, servicio.DatosSalida.PreGate.PRE_GATE_DETAILS.FirstOrDefault().TRANSACTION_NUMBER);
            var servicioWeb = new ServicioIIE.n4ServiceSoapClient();
            var respuesta = servicioWeb.basicInvoke(ConfigurationManager.AppSettings["scopeCoordinateIds"], xml);
            if (!respuesta.Contains(@"Code>0</Code"))
                return servicio.GrabarRegistroProcesoSalida(false, 14, $"SERVICIO IIE RETORNO : {respuesta.ToString()}");
            return servicio.SetearEstado(new EstadoGroovyPesajeSalida());
        }

        public static string ArmarXml(string nombreQuiosco, string aisv)
        {
            return $@"<iie><tipo>B</tipo><usuario>{nombreQuiosco}</usuario><validar>1</validar><parametros><aisv>{aisv}</aisv><documento>{aisv}</documento><peso>1</peso><cantidad>1</cantidad></parametros></iie>";
        }
    }

    public class EstadoSalidaProcessTruckFull : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            var xml = ArmarXml(servicio.DatosSalida);
            var respuesta = servicio.Conector.Invocacion(xml);
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
                return servicio.GrabarRegistroProcesoSalida(true, 13, respuesta.ResultadosConsultas.FirstOrDefault().Result);
            return servicio.GrabarRegistroProcesoSalida(false, 14, $"N4 RETORNO ERROR EN PROCESS TRUCK :{respuesta.ToString()}");
        }

        public static string ArmarXml(DatosPreGateSalida datos)
        {
            var fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            return $@"<gate>
    	                <process-truck scan-status=""0"" no-content=""false"">
                            <gate-id>RECEPTIO</gate-id>
		                    <stage-id>outgate</stage-id>
                            <lane-id>{datos.NombreQuiosco}</lane-id>
		                    <truck license-nbr=""{datos.PreGate.TRUCK_LICENCE}""/>
                            <driver license-nbr=""{datos.PreGate.DRIVER_ID}""/>
                            <truck-visit gos-tv-key=""{(datos.PreGate.PRE_GATE_ID * -1)}""/>
                            <timestamp>{fecha}</timestamp>
                        </process-truck>
                      </gate>";
        }
    }

    //<JGUSQUI 2019-03-01>
    //CAMBIOS DE ESTADOS EN N4 EN INGRESO Y SALIDA DE VEHICULOS CON CONTENEDORES VACIOS FARBEN  
    public class EstadoProcessTruckVaciosSAV : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            var xml = ArmarXml(servicio.DatosEntrada);
            var respuesta = servicio.Conector.Invocacion(xml);
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
                return servicio.GrabarRegistroProcesoEntrada(true, 13, respuesta.ResultadosConsultas.FirstOrDefault().Result);
            return servicio.GrabarRegistroProcesoEntrada(false, 14, $"N4 RETORNO ERROR EN PROCESS TRUCK :{respuesta.ToString()}");
        }

        public static string ArmarXml(DatosEntradaN4 datos)
        {
            var fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            return $@"<gate>
    	                <process-truck scan-status=""0"" no-content=""false"">
                            <gate-id>RECEPTIO_BOK</gate-id>
		                    <stage-id>bok_reg</stage-id>
                            <lane-id>{datos.NombreQuiosco}</lane-id>
		                    <truck license-nbr=""{datos.PlacaVehiculo}""/>
                            <driver license-nbr=""{datos.CedulaChofer}""/>
                            <truck-visit gos-tv-key=""{(datos.IdPreGate * -1)}""/>
                            <timestamp>{fecha}</timestamp>
                        </process-truck>
                      </gate>";
        }
    }

    public class EstadoSalidaProcessTruckVaciosSAV : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            var xml = ArmarXml(servicio.DatosSalida);
            var respuesta = servicio.Conector.Invocacion(xml);
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
                return servicio.GrabarRegistroProcesoSalida(true, 13, respuesta.ResultadosConsultas.FirstOrDefault().Result);
            return servicio.GrabarRegistroProcesoSalida(false, 14, $"N4 RETORNO ERROR EN PROCESS TRUCK :{respuesta.ToString()}");
        }

        public static string ArmarXml(DatosPreGateSalida datos)
        {
            var fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            return $@"<gate>
    	                <process-truck scan-status=""0"" no-content=""false"">
                            <gate-id>RECEPTIO_BOK</gate-id>
		                    <stage-id>bok_out</stage-id>
                            <lane-id>{datos.NombreQuiosco}</lane-id>
		                    <truck license-nbr=""{datos.PreGate.TRUCK_LICENCE}""/>
                            <driver license-nbr=""{datos.PreGate.DRIVER_ID}""/>
                            <truck-visit gos-tv-key=""{(datos.PreGate.PRE_GATE_ID * -1)}""/>
                            <timestamp>{fecha}</timestamp>
                        </process-truck>
                      </gate>";
        }
    }
    
    //<JGUSQUI 2020-03-06>
    public class EstadoICU : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            var xml = ArmarXml(servicio.RepositorioValidaAduana.ObtenerGKeyContenedorVacio(servicio.DatosSalida.PreGate.PRE_GATE_DETAILS.FirstOrDefault().CONTAINERS.FirstOrDefault().NUMBER));
            var respuesta = servicio.Conector.Invocacion(xml);
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
            {
                servicio.GrabarRegistroProcesoSalida("ICU",true, 13, $"ICU OK - XML SOLICITUD:{xml} - XML RESPUESTA: {respuesta.ToString()}");
                return servicio.SetearEstado(new EstadoSalidaProcessTruckVaciosDepot());
            }                
            return servicio.GrabarRegistroProcesoSalida(false, 14, $"N4 RETORNO ERROR EN ICU :{respuesta.ToString()}");
        }

        public static string ArmarXml( long gkey)
        {
            long v_ = gkey;
            var fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return $@"<groovy class-name=""CGSAUnitEventWS"" class-location=""code-extension"">
                        <parameters>
                            <parameter id=""UNIT"" value=""{v_}""/>
                            <parameter id=""EVENT"" value=""UNIT_OUT_CISE""/>
                            <parameter id=""NOTES"" value=""Gate Out Depot by Kiosco #7""/>
                            <parameter id=""USER"" value=""{ConfigurationManager.AppSettings["UserNameServicio"]}""/>
                            <parameter id=""DATE"" value=""{fecha}""/>
                        </parameters>
                     </groovy>";
        }
    }
    //<JGUSQUI 2019-03-01>
    public class EstadoSalidaProcessTruckVaciosDepot : EstadoN4
    {
        internal override DatosN4 Ejecutar(ServicioN4 servicio)
        {
            var xml = ArmarXml(servicio.DatosSalida);
            var respuesta = servicio.Conector.Invocacion(xml);
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
                return servicio.GrabarRegistroProcesoSalida(true, 13, respuesta.ResultadosConsultas.FirstOrDefault().Result);
            return servicio.GrabarRegistroProcesoSalida(false, 14, $"N4 RETORNO ERROR EN PROCESS TRUCK :{respuesta.ToString()}");
        }

        public static string ArmarXml(DatosPreGateSalida datos)
        {
            var fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            return $@"<gate>
                        <process-truck scan-status=""0"" no-content=""false"">
                            <gate-id>CISE</gate-id>
                            <stage-id>salida_cise</stage-id>
                            <lane-id>{datos.NombreQuiosco}</lane-id>
                            <truck license-nbr=""{datos.PreGate.TRUCK_LICENCE}"" />
                            <driver license-nbr=""{datos.PreGate.DRIVER_ID}""/>
                            <truck-visit gos-tv-key=""{(datos.PreGate.PRE_GATE_ID * -1)}"" />
                            <timestamp>{fecha}</timestamp>
                        </process-truck>
                    </gate>";
        }
    }

}