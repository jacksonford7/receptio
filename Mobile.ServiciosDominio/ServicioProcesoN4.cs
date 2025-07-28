using RECEPTIO.CapaAplicacion.Mobile.Aplicacion.Contratos;
using RECEPTIO.CapaAplicacion.Mobile.Aplicacion.Interfaces;
using RECEPTIO.CapaDominio.Mobile.Dominio.InterfacesRepositorios;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaDominio.Nucleo.ServiciosDominio.N4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RECEPTIO.CapaDominio.Mobile.ServiciosDominio
{
    public class ServicioProcesoN4 : IProcesoN4
    {
        private readonly IRepositorioTosProccess _repositorioTosProccess;
        private EstadoProcesoN4 _estado;
        internal readonly IConector Conector;
        internal DatosN4 Datos;
        internal DatosDeliveryImportBrbkCfs DatosBrBkCfs;
        internal DatosReceiveExport DatosReceiveExport;
        internal DatosReceiveExportBrBk DatosReceiveExportBrBk;
        internal DatosReceiveExportBanano DatosReceiveExportBanano;
        internal DatosDeliveryImportP2D DatosP2D;

        public ServicioProcesoN4(IRepositorioTosProccess repositorioTosProccess, IConector conector)
        {
            _repositorioTosProccess = repositorioTosProccess;
            Conector = conector;
        }

        public RespuestaProceso EjecutarProcesosDeliveryImportFull(DatosN4 datos)
        {
            Datos = datos;
            _estado = new EstadoTruckVisitImportFull();
            return _estado.Ejecutar(this);
        }

        public RespuestaProceso EjecutarProcesosDeliveryImportBrBkCfs(DatosDeliveryImportBrbkCfs datos)
        {
            DatosBrBkCfs = datos;
            _estado = new EstadoTruckVisitImportBrBkCfs();
            return _estado.Ejecutar(this);
        }

        public RespuestaProceso EjecutarProcesosReceiveExport(DatosReceiveExport datos)
        {
            DatosReceiveExport = datos;
            _estado = new EstadoTruckVisitReceiveExport();
            return _estado.Ejecutar(this);
        }

        public RespuestaProceso EjecutarProcesosReceiveExportBrBk(DatosReceiveExportBrBk datos)
        {
            DatosReceiveExportBrBk = datos;
            _estado = new EstadoTruckVisitReceiveExportBrBkCfs();
            return _estado.Ejecutar(this);
        }

        public RespuestaProceso EjecutarProcesosReceiveExportBanano(DatosReceiveExportBanano datos)
        {
            DatosReceiveExportBanano = datos;
            _estado = new EstadoTruckVisitReceiveExportBanano();
            return _estado.Ejecutar(this);
        }

        public RespuestaProceso LiberarHold(DatosN4 datos)
        {
            Datos = datos;
            _estado = new DesbloqueaUnit();
            return _estado.Ejecutar(this);
        }

        public RespuestaProceso CambiarHold(DatosN4 datos)
        {
            Datos = datos;
            _estado = new CambiobloqueoUnit();
            return _estado.Ejecutar(this);
        }

        public void LiberarRecursos()
        {
            _repositorioTosProccess.LiberarRecursos();
        }

        internal RespuestaProceso SetearEstado(EstadoProcesoN4 estado)
        {
            _estado = estado;
            return _estado.Ejecutar(this);
        }

        internal long RegistrarProceso(TOS_PROCCESS proceso)
        {
            _repositorioTosProccess.Agregar(proceso);
            return proceso.ID;
        }

        public RespuestaProceso EjecutarProcesosDeliveryImportMTYBooking(DatosN4 datos)
        {
            Datos = datos;
            _estado = new EstadoTruckVisitImportMTYBooking();
            return _estado.Ejecutar(this);
        }

        public RespuestaProceso EjecutarProcesosDeliveryImportP2D(DatosDeliveryImportP2D datos)
        {
            DatosP2D = datos;
            _estado = new EstadoTruckVisitImportP2D();
            return _estado.Ejecutar(this);
        }
    }

    public abstract class EstadoProcesoN4
    {
        internal abstract RespuestaProceso Ejecutar(ServicioProcesoN4 servicioProcesoN4);
    }

    public class EstadoTruckVisitImportFull : EstadoProcesoN4
    {
        internal override RespuestaProceso Ejecutar(ServicioProcesoN4 servicioProcesoN4)
        {
            var proceso = new TOS_PROCCESS { PRE_GATE_ID = Convert.ToInt64(servicioProcesoN4.Datos.IdPreGate), STEP = "TRUCK VISIT", STEP_DATE = DateTime.Now };
            var xml = ArmarXml(servicioProcesoN4.Datos);
            var respuesta = servicioProcesoN4.Conector.Invocacion(xml);
            proceso.RESPONSE = respuesta.ToString();
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
            {
                proceso.IS_OK = true;
                servicioProcesoN4.RegistrarProceso(proceso);
                return servicioProcesoN4.SetearEstado(new EstadoSubmitImportFull());
            }
            var id = servicioProcesoN4.RegistrarProceso(proceso);
            return new RespuestaProceso { IdTosProcess = id, Mensaje = $"N4 RETORNO ERROR EN CREATE TRUCK VISIT :{respuesta.ToString()}"};
        }

        public static string ArmarXml(DatosN4 datos)
        {
            var fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            return $@"<gate>
    	                <create-truck-visit>
		                    <gate-id>RECEPTIO</gate-id>
		                    <stage-id>pregatein</stage-id>
		                    <truck license-nbr=""{datos.PlacaVehiculo}"" trucking-co-id=""{datos.IdCompania}""/>
                            <driver license-nbr=""{datos.CedulaChofer}""/>
                            <truck-visit gos-tv-key=""{datos.GosTvKey}""/>
                            <timestamp>{fecha}</timestamp>
                        </create-truck-visit>
                      </gate>";
        }
    }

    public class EstadoSubmitImportFull : EstadoProcesoN4
    {
        internal override RespuestaProceso Ejecutar(ServicioProcesoN4 servicioProcesoN4)
        {
            var proceso = new TOS_PROCCESS { PRE_GATE_ID = Convert.ToInt64(servicioProcesoN4.Datos.IdPreGate), STEP = "SUBMIT", STEP_DATE = DateTime.Now };
            foreach (var numeroTransaccion in servicioProcesoN4.Datos.NumerosTransacciones)
            {
                var xml = ArmarXml(servicioProcesoN4.Datos.GosTvKey, numeroTransaccion);
                var respuesta = servicioProcesoN4.Conector.Invocacion(xml);
                proceso.RESPONSE = respuesta.ToString();
                if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
                {
                    proceso.IS_OK = true;
                    servicioProcesoN4.RegistrarProceso(proceso);
                    continue;
                }
                var id = servicioProcesoN4.RegistrarProceso(proceso);
                return new RespuestaProceso { IdTosProcess = id, Mensaje = $"N4 RETORNO ERROR EN SUBMIT :{respuesta.ToString()}" };
            }
            return servicioProcesoN4.SetearEstado(new EstadoStageDoneImportFull());
        }

        public static string ArmarXml(int gosTvKey, string numeroTransaccion)
        {
            return $@"<gate>
    	                <submit-transaction>
		                    <gate-id>RECEPTIO</gate-id>
		                    <stage-id>pregatein</stage-id>
                            <truck-visit gos-tv-key=""{gosTvKey}""/>
                            <truck-transaction appointment-nbr=""{numeroTransaccion}""/>
                        </submit-transaction>
                      </gate>";
        }
    }

    public class EstadoStageDoneImportFull : EstadoProcesoN4
    {
        internal override RespuestaProceso Ejecutar(ServicioProcesoN4 servicioProcesoN4)
        {
            var proceso = new TOS_PROCCESS { PRE_GATE_ID = Convert.ToInt64(servicioProcesoN4.Datos.IdPreGate), STEP = "STAGE DONE", STEP_DATE = DateTime.Now };
            var xml = ArmarXml(servicioProcesoN4.Datos);
            var respuesta = servicioProcesoN4.Conector.Invocacion(xml);
            proceso.RESPONSE = respuesta.ToString();
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
            {
                proceso.IS_OK = true;
                servicioProcesoN4.RegistrarProceso(proceso);
                return new RespuestaProceso { FueOk = true, Mensaje = "Proceso Ok." };
            }
            var id = servicioProcesoN4.RegistrarProceso(proceso);
            return new RespuestaProceso { IdTosProcess = id, Mensaje = $"N4 RETORNO ERROR EN STAGE DONE :{respuesta.ToString()}" };
        }

        public static string ArmarXml(DatosN4 datos)
        {
            return $@"<gate>
    	                <stage-done>
		                    <gate-id>RECEPTIO</gate-id>
		                    <stage-id>pregatein</stage-id>
                            <truck-visit gos-tv-key=""{datos.GosTvKey}""/>
                        </stage-done>
                      </gate>";
        }
    }

    public class EstadoTruckVisitImportBrBkCfs : EstadoProcesoN4
    {
        internal override RespuestaProceso Ejecutar(ServicioProcesoN4 servicioProcesoN4)
        {
            var proceso = new TOS_PROCCESS { PRE_GATE_ID = Convert.ToInt64(servicioProcesoN4.DatosBrBkCfs.IdPreGate), STEP = "TRUCK VISIT", STEP_DATE = DateTime.Now };
            var xml = ArmarXml(servicioProcesoN4.DatosBrBkCfs);
            var respuesta = servicioProcesoN4.Conector.Invocacion(xml);
            proceso.RESPONSE = respuesta.ToString();
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
            {
                proceso.IS_OK = true;
                servicioProcesoN4.RegistrarProceso(proceso);
                return servicioProcesoN4.SetearEstado(new EstadoGroovyImportBrBkCfs());
            }
            var id = servicioProcesoN4.RegistrarProceso(proceso);
            return new RespuestaProceso { IdTosProcess = id, Mensaje = $"N4 RETORNO ERROR EN CREATE TRUCK VISIT :{respuesta.ToString()}" };
        }

        public static string ArmarXml(DatosDeliveryImportBrbkCfs datos)
        {
            var fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            return $@"<gate>
    	                <create-truck-visit>
		                    <gate-id>RECEPTIO</gate-id>
		                    <stage-id>pregatein</stage-id>
		                    <truck license-nbr=""{datos.PlacaVehiculo}"" trucking-co-id=""{datos.IdCompania}""/>
                            <driver license-nbr=""{datos.CedulaChofer}""/>
                            <truck-visit gos-tv-key=""{datos.GosTvKey}""/>
                            <timestamp>{fecha}</timestamp>
                        </create-truck-visit>
                      </gate>";
        }
    }

    public class EstadoGroovyImportBrBkCfs : EstadoProcesoN4
    {
        internal override RespuestaProceso Ejecutar(ServicioProcesoN4 servicioProcesoN4)
        {
            var proceso = new TOS_PROCCESS { PRE_GATE_ID = Convert.ToInt64(servicioProcesoN4.DatosBrBkCfs.IdPreGate), STEP = "GROOVY", STEP_DATE = DateTime.Now };
            foreach (var numeroTransaccion in servicioProcesoN4.DatosBrBkCfs.NumerosTransacciones)
            {
                var xml = ArmarXml(servicioProcesoN4.DatosBrBkCfs, numeroTransaccion);
                var respuesta = servicioProcesoN4.Conector.Invocacion(xml);
                proceso.RESPONSE = respuesta.ToString();
                if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("ERROR")))
                {
                    proceso.IS_OK = true;
                    servicioProcesoN4.RegistrarProceso(proceso);
                    continue;
                }
                var id = servicioProcesoN4.RegistrarProceso(proceso);
                return new RespuestaProceso { IdTosProcess = id, Mensaje = $"N4 RETORNO ERROR EN GROOVY :{respuesta.ToString()}" };
            }
            return servicioProcesoN4.SetearEstado(new EstadoStageDoneImportBrBkCfs());
        }

        public static string ArmarXml(DatosDeliveryImportBrbkCfs datos, string numeroTransaccion)
        {
            var fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return $@"<groovy class-name=""CGSABRBKGateTransactionInWS"" class-location=""code-extension"">
                          <parameters>
		                    <parameter id=""gos-tv-key"" value=""{datos.GosTvKey}""/>
                            <parameter id=""order-nbr"" value=""{numeroTransaccion}""/>
                            <parameter id=""fecha"" value=""{fecha}""/>
                            <parameter id=""bl-nbr"" value=""{datos.Bl}""/>
                            <parameter id=""kiosco"" value=""""/>
                            <parameter id=""stage-id"" value=""pregatein""/>
                            <parameter id=""transaction-type"" value=""DB""/>
                            <parameter id=""gate-id"" value=""RECEPTIO""/>
                            <parameter id=""nextstage-id"" value=""ingate""/>
                            <parameter id=""type-doc"" value=""""/>
                            <parameter id=""item-qty"" value=""{datos.Qty}""/>
                            </parameters>
                      </groovy>";
        }
    }

    public class EstadoStageDoneImportBrBkCfs : EstadoProcesoN4
    {
        internal override RespuestaProceso Ejecutar(ServicioProcesoN4 servicioProcesoN4)
        {
            var proceso = new TOS_PROCCESS { PRE_GATE_ID = Convert.ToInt64(servicioProcesoN4.DatosBrBkCfs.IdPreGate), STEP = "STAGE DONE", STEP_DATE = DateTime.Now };
            var xml = ArmarXml(servicioProcesoN4.DatosBrBkCfs);
            var respuesta = servicioProcesoN4.Conector.Invocacion(xml);
            proceso.RESPONSE = respuesta.ToString();
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
            {
                proceso.IS_OK = true;
                servicioProcesoN4.RegistrarProceso(proceso);
                return new RespuestaProceso { FueOk = true, Mensaje = "Proceso Ok." };
            }
            var id = servicioProcesoN4.RegistrarProceso(proceso);
            return new RespuestaProceso { IdTosProcess = id, Mensaje = $"N4 RETORNO ERROR EN STAGE DONE :{respuesta.ToString()}" };
        }

        public static string ArmarXml(DatosDeliveryImportBrbkCfs datos)
        {
            return $@"<gate>
    	                <stage-done>
		                    <gate-id>RECEPTIO</gate-id>
		                    <stage-id>pregatein</stage-id>
                            <truck-visit gos-tv-key=""{datos.GosTvKey}""/>
                        </stage-done>
                      </gate>";
        }
    }

    public class EstadoTruckVisitReceiveExport : EstadoProcesoN4
    {
        internal override RespuestaProceso Ejecutar(ServicioProcesoN4 servicioProcesoN4)
        {
            var proceso = new TOS_PROCCESS { PRE_GATE_ID = Convert.ToInt64(servicioProcesoN4.DatosReceiveExport.IdPreGate), STEP = "TRUCK VISIT", STEP_DATE = DateTime.Now };
            var xml = ArmarXml(servicioProcesoN4.DatosReceiveExport);
            var respuesta = servicioProcesoN4.Conector.Invocacion(xml);
            proceso.RESPONSE = respuesta.ToString();
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
            {
                proceso.IS_OK = true;
                servicioProcesoN4.RegistrarProceso(proceso);
                return servicioProcesoN4.SetearEstado(new EstadoSubmitReceiveExport());
            }
            var id = servicioProcesoN4.RegistrarProceso(proceso);
            return new RespuestaProceso { IdTosProcess = id, Mensaje = $"N4 RETORNO ERROR EN CREATE TRUCK VISIT :{respuesta.ToString()}" };
        }

        public static string ArmarXml(DatosReceiveExport datos)
        {
            var fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            return $@"<gate>
    	                <create-truck-visit>
		                    <gate-id>RECEPTIO</gate-id>
		                    <stage-id>pregatein</stage-id>
		                    <truck license-nbr=""{datos.PlacaVehiculo}"" trucking-co-id=""{datos.IdCompania}""/>
                            <driver license-nbr=""{datos.CedulaChofer}""/>
                            <truck-visit gos-tv-key=""{datos.GosTvKey}""/>
                            <timestamp>{fecha}</timestamp>
                        </create-truck-visit>
                      </gate>";
        }
    }

    public class EstadoSubmitReceiveExport : EstadoProcesoN4
    {
        internal override RespuestaProceso Ejecutar(ServicioProcesoN4 servicioProcesoN4)
        {
            var proceso = new TOS_PROCCESS { PRE_GATE_ID = Convert.ToInt64(servicioProcesoN4.DatosReceiveExport.IdPreGate), STEP = "SUBMIT", STEP_DATE = DateTime.Now };
            var xml = ArmarXml(servicioProcesoN4.DatosReceiveExport);
            var respuesta = servicioProcesoN4.Conector.Invocacion(xml);
            proceso.RESPONSE = respuesta.ToString();
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
            {
                proceso.IS_OK = true;
                servicioProcesoN4.RegistrarProceso(proceso);
                return servicioProcesoN4.SetearEstado(new EstadoStageDoneReceiveExport());
            }
            var id = servicioProcesoN4.RegistrarProceso(proceso);
            return new RespuestaProceso { IdTosProcess = id, Mensaje = $"N4 RETORNO ERROR EN SUBMIT :{respuesta.ToString()}" };
        }

        public static string ArmarXml(DatosReceiveExport datos)
        {
            var xml = new StringBuilder();
            xml.Append($@"<gate>
    	                <submit-multiple-transactions>
		                    <gate-id>RECEPTIO</gate-id>
		                    <stage-id>pregatein</stage-id>
                            <truck-visit gos-tv-key=""{datos.GosTvKey}""/>
                            <truck-transactions>");
            foreach (var dataContenedor in datos.DataContenedores)
            {
                var sello1 = dataContenedor.Contenedor.SEALS == null ? "" : dataContenedor.Contenedor.SEALS.Any(s => s.CAPTION == "SEAL-1") ? dataContenedor.Contenedor.SEALS.FirstOrDefault(s => s.CAPTION == "SEAL-1").VALUE : "";
                var sello2 = dataContenedor.Contenedor.SEALS == null ? "" : dataContenedor.Contenedor.SEALS.Any(s => s.CAPTION == "SEAL-2") ? dataContenedor.Contenedor.SEALS.FirstOrDefault(s => s.CAPTION == "SEAL-2").VALUE : "";
                var sello3 = dataContenedor.Contenedor.SEALS == null ? "" : dataContenedor.Contenedor.SEALS.Any(s => s.CAPTION == "SEAL-3") ? dataContenedor.Contenedor.SEALS.FirstOrDefault(s => s.CAPTION == "SEAL-3").VALUE : "";
                var sello4 = dataContenedor.Contenedor.SEALS == null ? "" : dataContenedor.Contenedor.SEALS.Any(s => s.CAPTION == "SEAL-4") ? dataContenedor.Contenedor.SEALS.FirstOrDefault(s => s.CAPTION == "SEAL-4").VALUE : "";
                var danos = new StringBuilder();
                if (dataContenedor.Contenedor.DAMAGES != null)
                {
                    danos.Append("<damages>");
                    foreach (var item in dataContenedor.Contenedor.DAMAGES)
                        danos.Append($@"<damage severity=""{item.SEVERITY}"" type=""{item.DAMAGE_TYPE}"" component-id=""{item.COMPONENT}"" description=""{item.NOTES}""/>");
                    danos.Append("</damages>");
                }
                xml.Append($@"<truck-transaction tran-type=""RE"" order-nbr=""{dataContenedor.Aisv}"" notes="""">
                                    <eq-order order-nbr=""{dataContenedor.Aisv}"" order-type=""BOOK"" line-id=""{dataContenedor.Linea}"" freight-kind=""{dataContenedor.TipoCarga}"">
                                        <eq-order-items>
                                            <eq-order-item type=""{dataContenedor.Iso}"" eq-length=""{dataContenedor.Tamano}"" />
                                        </eq-order-items>
                                    </eq-order>
                                    <container eqid=""{dataContenedor.Contenedor.NUMBER}"" departure-order-nbr=""{dataContenedor.Aisv}"" seal-1=""{sello1}"" seal-2=""{sello2}"" seal-3=""{sello3}"" seal-4=""{sello4}"">
                                        {danos.ToString()}
                                    </container>
                                    <extension />
                                </truck-transaction>");
            }
            xml.Append($@"</truck-transactions>
                        </submit-multiple-transactions>
                      </gate>");
            return xml.ToString();
        }
    }

    public class EstadoStageDoneReceiveExport : EstadoProcesoN4
    {
        internal override RespuestaProceso Ejecutar(ServicioProcesoN4 servicioProcesoN4)
        {
            var proceso = new TOS_PROCCESS { PRE_GATE_ID = Convert.ToInt64(servicioProcesoN4.DatosReceiveExport.IdPreGate), STEP = "STAGE DONE", STEP_DATE = DateTime.Now };
            var xml = ArmarXml(servicioProcesoN4.DatosReceiveExport);
            var respuesta = servicioProcesoN4.Conector.Invocacion(xml);
            proceso.RESPONSE = respuesta.ToString();
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
            {
                proceso.IS_OK = true;
                servicioProcesoN4.RegistrarProceso(proceso);
                return new RespuestaProceso { FueOk = true, Mensaje = "Proceso Ok." };
            }
            var id = servicioProcesoN4.RegistrarProceso(proceso);
            return new RespuestaProceso { IdTosProcess = id, Mensaje = $"N4 RETORNO ERROR EN STAGE DONE :{respuesta.ToString()}" };
        }

        public static string ArmarXml(DatosReceiveExport datos)
        {
            return $@"<gate>
    	                <stage-done>
		                    <gate-id>RECEPTIO</gate-id>
		                    <stage-id>pregatein</stage-id>
                            <truck-visit gos-tv-key=""{datos.GosTvKey}""/>
                        </stage-done>
                      </gate>";
        }
    }

    public class EstadoTruckVisitReceiveExportBrBkCfs : EstadoProcesoN4
    {
        internal override RespuestaProceso Ejecutar(ServicioProcesoN4 servicioProcesoN4)
        {
            var proceso = new TOS_PROCCESS { PRE_GATE_ID = Convert.ToInt64(servicioProcesoN4.DatosReceiveExportBrBk.IdPreGate), STEP = "TRUCK VISIT", STEP_DATE = DateTime.Now };
            var xml = ArmarXml(servicioProcesoN4.DatosReceiveExportBrBk);
            var respuesta = servicioProcesoN4.Conector.Invocacion(xml);
            proceso.RESPONSE = respuesta.ToString();
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
            {
                proceso.IS_OK = true;
                servicioProcesoN4.RegistrarProceso(proceso);
                return servicioProcesoN4.SetearEstado(new EstadoGroovyReceiveExportBrBkCfs());
            }
            var id = servicioProcesoN4.RegistrarProceso(proceso);
            return new RespuestaProceso { IdTosProcess = id, Mensaje = $"N4 RETORNO ERROR EN CREATE TRUCK VISIT :{respuesta.ToString()}" };
        }

        public static string ArmarXml(DatosReceiveExportBrBk datos)
        {
            var fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            return $@"<gate>
    	                <create-truck-visit>
		                    <gate-id>RECEPTIO</gate-id>
		                    <stage-id>pregatein</stage-id>
		                    <truck license-nbr=""{datos.PlacaVehiculo}"" trucking-co-id=""{datos.IdCompania}""/>
                            <driver license-nbr=""{datos.CedulaChofer}""/>
                            <truck-visit gos-tv-key=""{datos.GosTvKey}""/>
                            <timestamp>{fecha}</timestamp>
                        </create-truck-visit>
                      </gate>";
        }
    }

    public class EstadoGroovyReceiveExportBrBkCfs : EstadoProcesoN4
    {
        internal override RespuestaProceso Ejecutar(ServicioProcesoN4 servicioProcesoN4)
        {
            var proceso = new TOS_PROCCESS { PRE_GATE_ID = Convert.ToInt64(servicioProcesoN4.DatosReceiveExportBrBk.IdPreGate), STEP = "GROOVY", STEP_DATE = DateTime.Now };
            var xml = ArmarXml(servicioProcesoN4.DatosReceiveExportBrBk);
            var respuesta = servicioProcesoN4.Conector.Invocacion(xml);
            proceso.RESPONSE = respuesta.ToString();
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("ERROR")))
            {
                proceso.IS_OK = true;
                servicioProcesoN4.RegistrarProceso(proceso);
                return servicioProcesoN4.SetearEstado(new EstadoStageDoneRecieveExportBrBkCfs());
            }
            var id = servicioProcesoN4.RegistrarProceso(proceso);
            return new RespuestaProceso { IdTosProcess = id, Mensaje = $"N4 RETORNO ERROR EN GROOVY :{respuesta.ToString()}" };
        }

        public static string ArmarXml(DatosReceiveExportBrBk datos)
        {
            var fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return $@"<groovy class-name=""CGSABRBKGateTransactionInWS"" class-location=""code-extension"">
                          <parameters>
		                    <parameter id=""gos-tv-key"" value=""{datos.GosTvKey}""/>
                            <parameter id=""fecha"" value=""{fecha}""/>
                            <parameter id=""bl-nbr"" value=""{datos.Bl}""/>
                            <parameter id=""kiosco"" value=""{datos.Ip}""/>
                            <parameter id=""stage-id"" value=""pregatein""/>
                            <parameter id=""nextstage-id"" value=""ingate""/>
		                    <parameter id=""bl-item-seq"" value=""1""/>
                            <parameter id=""item-qty"" value=""{datos.Cantidad}""/>
                            <parameter id=""transaction-type"" value=""RB""/>
		                    <parameter id=""vessel-visit"" value=""{datos.VeeselVisit}""/>
                            <parameter id = ""notes"" value=""{datos.Notas}"" />
                            <parameter id=""numdocumento"" value=""{datos.Dae}""/>                      
                          </parameters>
                      </groovy>";
        }
    }

    public class EstadoStageDoneRecieveExportBrBkCfs : EstadoProcesoN4
    {
        internal override RespuestaProceso Ejecutar(ServicioProcesoN4 servicioProcesoN4)
        {
            var proceso = new TOS_PROCCESS { PRE_GATE_ID = Convert.ToInt64(servicioProcesoN4.DatosReceiveExportBrBk.IdPreGate), STEP = "STAGE DONE", STEP_DATE = DateTime.Now };
            var xml = ArmarXml(servicioProcesoN4.DatosReceiveExportBrBk);
            var respuesta = servicioProcesoN4.Conector.Invocacion(xml);
            proceso.RESPONSE = respuesta.ToString();
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
            {
                proceso.IS_OK = true;
                servicioProcesoN4.RegistrarProceso(proceso);
                return new RespuestaProceso { FueOk = true, Mensaje = "Proceso Ok." };
            }
            var id = servicioProcesoN4.RegistrarProceso(proceso);
            return new RespuestaProceso { IdTosProcess = id, Mensaje = $"N4 RETORNO ERROR EN STAGE DONE :{respuesta.ToString()}" };
        }

        public static string ArmarXml(DatosReceiveExportBrBk datos)
        {
            return $@"<gate>
    	                <stage-done>
		                    <gate-id>RECEPTIO</gate-id>
		                    <stage-id>pregatein</stage-id>
                            <truck-visit gos-tv-key=""{datos.GosTvKey}""/>
                        </stage-done>
                      </gate>";
        }
    }

    public class EstadoTruckVisitReceiveExportBanano : EstadoProcesoN4
    {
        internal override RespuestaProceso Ejecutar(ServicioProcesoN4 servicioProcesoN4)
        {
            var proceso = new TOS_PROCCESS { PRE_GATE_ID = Convert.ToInt64(servicioProcesoN4.DatosReceiveExportBanano.IdPreGate), STEP = "TRUCK VISIT", STEP_DATE = DateTime.Now };
            var xml = ArmarXml(servicioProcesoN4.DatosReceiveExportBanano);
            var respuesta = servicioProcesoN4.Conector.Invocacion(xml);
            proceso.RESPONSE = respuesta.ToString();
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
            {
                proceso.IS_OK = true;
                servicioProcesoN4.RegistrarProceso(proceso);
                return servicioProcesoN4.SetearEstado(new EstadoGroovyReceiveExportBanano());
            }
            var id = servicioProcesoN4.RegistrarProceso(proceso);
            return new RespuestaProceso { IdTosProcess = id, Mensaje = $"N4 RETORNO ERROR EN CREATE TRUCK VISIT :{respuesta.ToString()}" };
        }

        public static string ArmarXml(DatosReceiveExportBanano datos)
        {
            var fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            return $@"<gate>
    	                <create-truck-visit>
		                    <gate-id>RECEPTIO_BAN</gate-id>
		                    <stage-id>banano_pre</stage-id>
		                    <truck license-nbr=""{datos.PlacaVehiculo}"" trucking-co-id=""{datos.IdCompania}""/>
                            <driver license-nbr=""{datos.CedulaChofer}""/>
                            <truck-visit gos-tv-key=""{datos.GosTvKey}""/>
                            <timestamp>{fecha}</timestamp>
                        </create-truck-visit>
                      </gate>";
        }
    }

    public class EstadoGroovyReceiveExportBanano : EstadoProcesoN4
    {
        internal override RespuestaProceso Ejecutar(ServicioProcesoN4 servicioProcesoN4)
        {
            var proceso = new TOS_PROCCESS { PRE_GATE_ID = Convert.ToInt64(servicioProcesoN4.DatosReceiveExportBanano.IdPreGate), STEP = "GROOVY", STEP_DATE = DateTime.Now };
            var xml = ArmarXml(servicioProcesoN4.DatosReceiveExportBanano);
            var respuesta = servicioProcesoN4.Conector.Invocacion(xml);
            proceso.RESPONSE = respuesta.ToString();
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("ERROR")))
            {
                proceso.IS_OK = true;
                servicioProcesoN4.RegistrarProceso(proceso);
                return servicioProcesoN4.SetearEstado(new EstadoStageDoneRecieveExportBanano());
            }
            var id = servicioProcesoN4.RegistrarProceso(proceso);
            return new RespuestaProceso { IdTosProcess = id, Mensaje = $"N4 RETORNO ERROR EN GROOVY :{respuesta.ToString()}" };
        }

        public static string ArmarXml(DatosReceiveExportBanano datos)
        {
            var fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return $@"<groovy class-name=""CGSABRBKGateTransactionInWS"" class-location=""code-extension"">
                          <parameters>
		                    <parameter id=""gos-tv-key"" value=""{datos.GosTvKey}""/>
                            <parameter id=""fecha"" value=""{fecha}""/>
                            <parameter id=""bl-nbr"" value=""{datos.Bl}""/>
                            <parameter id=""kiosco"" value=""{datos.Ip}""/>
                            <parameter id=""stage-id"" value=""banano_pre""/>
                            <parameter id=""nextstage-id"" value=""banano_in""/>
		                    <parameter id=""bl-item-seq"" value=""1""/>
                            <parameter id=""item-qty"" value=""{datos.Cantidad}""/>
                            <parameter id=""transaction-type"" value=""RB""/>
		                    <parameter id=""vessel-visit"" value=""{datos.VeeselVisit}""/>
                            <parameter id = ""notes"" value=""{datos.Notas}"" />
                            <parameter id=""numdocumento"" value=""{datos.Dae}""/>                      
                          </parameters>
                      </groovy>";
        }
    }

    public class EstadoStageDoneRecieveExportBanano : EstadoProcesoN4
    {
        internal override RespuestaProceso Ejecutar(ServicioProcesoN4 servicioProcesoN4)
        {
            var proceso = new TOS_PROCCESS { PRE_GATE_ID = Convert.ToInt64(servicioProcesoN4.DatosReceiveExportBanano.IdPreGate), STEP = "STAGE DONE", STEP_DATE = DateTime.Now };
            var xml = ArmarXml(servicioProcesoN4.DatosReceiveExportBanano);
            var respuesta = servicioProcesoN4.Conector.Invocacion(xml);
            proceso.RESPONSE = respuesta.ToString();
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
            {
                proceso.IS_OK = true;
                servicioProcesoN4.RegistrarProceso(proceso);
                return new RespuestaProceso { FueOk = true, Mensaje = "Proceso Ok." };
            }
            var id = servicioProcesoN4.RegistrarProceso(proceso);
            return new RespuestaProceso { IdTosProcess = id, Mensaje = $"N4 RETORNO ERROR EN STAGE DONE :{respuesta.ToString()}" };
        }

        public static string ArmarXml(DatosReceiveExportBanano datos)
        {
            return $@"<gate>
    	                <stage-done>
		                    <gate-id>RECEPTIO_BAN</gate-id>
		                    <stage-id>banano_pre</stage-id>
                            <truck-visit gos-tv-key=""{datos.GosTvKey}""/>
                        </stage-done>
                      </gate>";
        }
    }

    public class DesbloqueaUnit : EstadoProcesoN4
    {
        internal override RespuestaProceso Ejecutar(ServicioProcesoN4 servicioProcesoN4)
        {
            var xml = ArmarXml(servicioProcesoN4.Datos.IdCompania, servicioProcesoN4.Datos.CedulaChofer);
            var respuesta = servicioProcesoN4.Conector.Invocacion(xml);
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2)
                return new RespuestaProceso { FueOk = true, Mensaje = "Proceso Ok." };
            return new RespuestaProceso { Mensaje = $"N4 RETORNO ERROR EN HOLD :{respuesta.ToString()}" };
        }

        public static string ArmarXml(string cnt, string bloq)
        {
            return $@"<hpu>
    	                <entities>
		                    <units>
		                    <unit id=""{cnt}""/>
                            </units>
                        </entities>
                        <flags>
                            <flag hold-perm-id=""{bloq}"" action=""GRANT_PERMISSION""/>
                        </flags>
                        </hpu>";
        }
    }

    public class CambiobloqueoUnit : EstadoProcesoN4
    {
        internal override RespuestaProceso Ejecutar(ServicioProcesoN4 servicioProcesoN4)
        {
            var xml = ArmarXml(servicioProcesoN4.Datos.IdCompania);
            var respuesta = servicioProcesoN4.Conector.Invocacion(xml);
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2)
                return new RespuestaProceso { FueOk = true, Mensaje = "Proceso Ok." };
            return new RespuestaProceso { Mensaje = $"N4 RETORNO ERROR EN CAMBIO HOLD :{respuesta.ToString()}" };
        }

        public static string ArmarXml(string cnt)
        {
            return $@"<groovy class-name=""CGSAChangeAppointment"" class-location=""code-extension"">
                          <parameters>
                            <parameter id=""appointment-gkey"" value=""{cnt}""/>
                            <parameter id=""gate"" value=""RECEPTIO""/>
                            <parameter id=""notes"" value=""RECEPTIO""/>
                          </parameters>
                       </groovy>";
        }
    }

    public class EstadoTruckVisitImportMTYBooking : EstadoProcesoN4
    {
        internal override RespuestaProceso Ejecutar(ServicioProcesoN4 servicioProcesoN4)
        {
            var proceso = new TOS_PROCCESS { PRE_GATE_ID = Convert.ToInt64(servicioProcesoN4.Datos.IdPreGate), STEP = "TRUCK VISIT", STEP_DATE = DateTime.Now };
            var xml = ArmarXml(servicioProcesoN4.Datos);
            var respuesta = servicioProcesoN4.Conector.Invocacion(xml);
            proceso.RESPONSE = respuesta.ToString();
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
            {
                proceso.IS_OK = true;
                servicioProcesoN4.RegistrarProceso(proceso);
                return servicioProcesoN4.SetearEstado(new EstadoSubmitImportMTYBooking());
            }
            var id = servicioProcesoN4.RegistrarProceso(proceso);
            return new RespuestaProceso { IdTosProcess = id, Mensaje = $"N4 RETORNO ERROR EN CREATE TRUCK VISIT :{respuesta.ToString()}" };
        }

        public static string ArmarXml(DatosN4 datos)
        {
            var fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            return $@"<gate>
    	                <create-truck-visit>
		                    <gate-id>RECEPTIO_BOK</gate-id>
		                    <stage-id>bok_pre</stage-id>
		                    <truck license-nbr=""{datos.PlacaVehiculo}"" trucking-co-id=""{datos.IdCompania}""/>
                            <driver license-nbr=""{datos.CedulaChofer}""/>
                            <truck-visit gos-tv-key=""{datos.GosTvKey}""/>
                            <timestamp>{fecha}</timestamp>
                        </create-truck-visit>
                      </gate>";
        }
    }

    public class EstadoSubmitImportMTYBooking : EstadoProcesoN4
    {
        internal override RespuestaProceso Ejecutar(ServicioProcesoN4 servicioProcesoN4)
        {
            var proceso = new TOS_PROCCESS { PRE_GATE_ID = Convert.ToInt64(servicioProcesoN4.Datos.IdPreGate), STEP = "SUBMIT", STEP_DATE = DateTime.Now };
            foreach (var numeroTransaccion in servicioProcesoN4.Datos.NumerosTransacciones)
            {
                var xml = ArmarXml(servicioProcesoN4.Datos.GosTvKey, numeroTransaccion);
                var respuesta = servicioProcesoN4.Conector.Invocacion(xml);
                proceso.RESPONSE = respuesta.ToString();
                if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
                {
                    proceso.IS_OK = true;
                    servicioProcesoN4.RegistrarProceso(proceso);
                    continue;
                }
                var id = servicioProcesoN4.RegistrarProceso(proceso);
                return new RespuestaProceso { IdTosProcess = id, Mensaje = $"N4 RETORNO ERROR EN SUBMIT :{respuesta.ToString()}" };
            }
            return servicioProcesoN4.SetearEstado(new EstadoStageDoneImportMTYBooking());
        }

        public static string ArmarXml(int gosTvKey, string numeroTransaccion)
        {
            var transaccion = numeroTransaccion.Split('-');

            return $@"<gate>
                        <submit-transaction>
                            <gate-id>RECEPTIO_BOK</gate-id>
                            <stage-id>bok_pre</stage-id>
                            <truck-visit gos-tv-key=""{gosTvKey.ToString()}""/>
                            <truck-transaction tran-type = ""DM"" order-nbr = ""{transaccion[0].ToString()}"" notes = """" >
                                    <eq-order order-nbr=""{transaccion[0].ToString()}"" order-type=""BOOK"" line-id=""{transaccion[1].ToString()}"" freight-kind=""MTY"">
                                        <eq-order-items>
                                            <eq-order-item type=""{transaccion[2].ToString()}"" eq-length=""{transaccion[3].ToString()}"" eq-height=""NOM86"" eq-iso-group=""{transaccion[5].ToString()}""/>
                                         </eq-order-items>
							        </eq-order>
			                </truck-transaction>
                        </submit-transaction>
                    </gate>";
        }
    }

    public class EstadoStageDoneImportMTYBooking : EstadoProcesoN4
    {
        internal override RespuestaProceso Ejecutar(ServicioProcesoN4 servicioProcesoN4)
        {
            var proceso = new TOS_PROCCESS { PRE_GATE_ID = Convert.ToInt64(servicioProcesoN4.Datos.IdPreGate), STEP = "STAGE DONE", STEP_DATE = DateTime.Now };
            var xml = ArmarXml(servicioProcesoN4.Datos);
            var respuesta = servicioProcesoN4.Conector.Invocacion(xml);
            proceso.RESPONSE = respuesta.ToString();
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
            {
                proceso.IS_OK = true;
                servicioProcesoN4.RegistrarProceso(proceso);
                return new RespuestaProceso { FueOk = true, Mensaje = "Proceso Ok." };
            }
            var id = servicioProcesoN4.RegistrarProceso(proceso);
            return new RespuestaProceso { IdTosProcess = id, Mensaje = $"N4 RETORNO ERROR EN STAGE DONE :{respuesta.ToString()}" };
        }

        public static string ArmarXml(DatosN4 datos)
        {
            return $@"<gate>
    	                <stage-done>
		                    <gate-id>RECEPTIO_BOK</gate-id>
		                    <stage-id>bok_pre</stage-id>
                            <truck-visit gos-tv-key=""{datos.GosTvKey}""/>
                        </stage-done>
                      </gate>";
        }
    }

    public class EstadoTruckVisitImportP2D : EstadoProcesoN4
    {
        internal override RespuestaProceso Ejecutar(ServicioProcesoN4 servicioProcesoN4)
        {
            var proceso = new TOS_PROCCESS { PRE_GATE_ID = Convert.ToInt64(servicioProcesoN4.DatosP2D.IdPreGate), STEP = "TRUCK VISIT", STEP_DATE = DateTime.Now };
            var xml = ArmarXml(servicioProcesoN4.DatosP2D);
            var respuesta = servicioProcesoN4.Conector.Invocacion(xml);
            proceso.RESPONSE = respuesta?.ToString();
            if (respuesta != null)
            {
                if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
                {
                    proceso.IS_OK = true;
                    servicioProcesoN4.RegistrarProceso(proceso);
                    return servicioProcesoN4.SetearEstado(new EstadoGroovyImportP2D());
                }
                var id = servicioProcesoN4.RegistrarProceso(proceso);
                return new RespuestaProceso { IdTosProcess = id, Mensaje = $"N4 RETORNO ERROR EN CREATE TRUCK VISIT :{respuesta.ToString()}" };
            }
            else
            {
                proceso.RESPONSE = string.Empty;
               var id = servicioProcesoN4.RegistrarProceso(proceso);
                return new RespuestaProceso { IdTosProcess = id, Mensaje = $"N4 RETORNO ERROR EN CREATE TRUCK VISIT : N4 SIN RESPUESTA" };
            }
        }

        public static string ArmarXml(DatosDeliveryImportP2D datos)
        {
            var fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            return $@"<gate>
    	                <create-truck-visit>
		                    <gate-id>RECEPTIO</gate-id>
		                    <stage-id>pregatein</stage-id>
		                    <truck license-nbr=""{datos.PlacaVehiculo}"" trucking-co-id=""{datos.IdCompania}""/>
                            <driver license-nbr=""{datos.CedulaChofer}""/>
                            <truck-visit gos-tv-key=""{datos.GosTvKey}""/>
                            <timestamp>{fecha}</timestamp>
                        </create-truck-visit>
                      </gate>";
        }
    }

    public class EstadoGroovyImportP2D : EstadoProcesoN4
    {
        internal override RespuestaProceso Ejecutar(ServicioProcesoN4 servicioProcesoN4)
        {
            var proceso = new TOS_PROCCESS { PRE_GATE_ID = Convert.ToInt64(servicioProcesoN4.DatosP2D.IdPreGate), STEP = "GROOVY", STEP_DATE = DateTime.Now };
            foreach (var datosTransaccionP2D in servicioProcesoN4.DatosP2D.DatosTransaccionP2D)
            {
                var xml = ArmarXml(servicioProcesoN4.DatosP2D, datosTransaccionP2D);
                var respuesta = servicioProcesoN4.Conector.Invocacion(xml);
                proceso.RESPONSE = respuesta.ToString();
                if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("ERROR")))
                {
                    proceso.IS_OK = true;
                    servicioProcesoN4.RegistrarProceso(proceso);
                    continue;
                }
                var id = servicioProcesoN4.RegistrarProceso(proceso);
                return new RespuestaProceso { IdTosProcess = id, Mensaje = $"N4 RETORNO ERROR EN GROOVY :{respuesta.ToString()}" };
            }
            return servicioProcesoN4.SetearEstado(new EstadoStageDoneImportP2D());
        }

        public static string ArmarXml(DatosDeliveryImportP2D datos, DatosBLP2D datosTransaccionP2D)
        {
            var fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return $@"<groovy class-name=""CGSABRBKGateTransactionInWS"" class-location=""code-extension"">
                          <parameters>
		                    <parameter id=""gos-tv-key"" value=""{datos.GosTvKey}""/>
                            <parameter id=""order-nbr"" value=""{datosTransaccionP2D.NumeroTransaccion}""/>
                            <parameter id=""fecha"" value=""{fecha}""/>
                            <parameter id=""bl-nbr"" value=""{datosTransaccionP2D.Bl}""/>
                            <parameter id=""kiosco"" value=""""/>
                            <parameter id=""stage-id"" value=""pregatein""/>
                            <parameter id=""transaction-type"" value=""DB""/>
                            <parameter id=""gate-id"" value=""RECEPTIO""/>
                            <parameter id=""nextstage-id"" value=""ingate""/>
                            <parameter id=""type-doc"" value=""""/>
                            <parameter id=""item-qty"" value=""{datosTransaccionP2D.Qty}""/>
                            </parameters>
                      </groovy>";
        }
    }

    public class EstadoStageDoneImportP2D : EstadoProcesoN4
    {
        internal override RespuestaProceso Ejecutar(ServicioProcesoN4 servicioProcesoN4)
        {
            var proceso = new TOS_PROCCESS { PRE_GATE_ID = Convert.ToInt64(servicioProcesoN4.DatosP2D.IdPreGate), STEP = "STAGE DONE", STEP_DATE = DateTime.Now };
            var xml = ArmarXml(servicioProcesoN4.DatosP2D);
            var respuesta = servicioProcesoN4.Conector.Invocacion(xml);
            proceso.RESPONSE = respuesta.ToString();
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
            {
                proceso.IS_OK = true;
                servicioProcesoN4.RegistrarProceso(proceso);
                return new RespuestaProceso { FueOk = true, Mensaje = "Proceso Ok." };
            }
            var id = servicioProcesoN4.RegistrarProceso(proceso);
            return new RespuestaProceso { IdTosProcess = id, Mensaje = $"N4 RETORNO ERROR EN STAGE DONE :{respuesta.ToString()}" };
        }

        public static string ArmarXml(DatosDeliveryImportP2D datos)
        {
            return $@"<gate>
    	                <stage-done>
		                    <gate-id>RECEPTIO</gate-id>
		                    <stage-id>pregatein</stage-id>
                            <truck-visit gos-tv-key=""{datos.GosTvKey}""/>
                        </stage-done>
                      </gate>";
        }
    }
}
