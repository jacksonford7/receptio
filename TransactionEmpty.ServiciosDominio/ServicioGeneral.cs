using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Contratos;
using RECEPTIO.CapaAplicacion.TransactionEmpty.Aplicacion.Contratos;
using RECEPTIO.CapaAplicacion.TransactionEmpty.Aplicacion.Interfaces;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaDominio.Nucleo.ServiciosDominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.ServiciosDominio.N4;
using RECEPTIO.CapaPresentacion.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;

namespace RECEPTIO.CapaDominio.TransactionEmpty.ServiciosDominio
{
    public class ServicioGeneral : IGeneral
    {
        internal readonly IRepositorioPreGate RepositorioPreGate;
        internal readonly IRepositorioTransaccionQuiosco RepositorioTransaccionQuiosco;
        internal readonly IRepositorioValidaAduana RepositorioValidaAduana;
        internal readonly IConector Conector;
        internal readonly IRepositorioN4 RepositorioN4;
        internal long? Id;
        internal KIOSK Kiosco;
        private EstadoKioscoVacio _estado;
        internal PRE_GATE PreGate;
        internal List<PROCESS> Procesos;
        internal PRE_GATE PreGateEntregaVacio;
        internal string Xml;
        internal string XmlRecepcion;
        internal List<string> ContenedoresAsignados;
        internal TOS_PROCCESS TosProcess;

        public ServicioGeneral(IRepositorioPreGate repositorioPreGate, IRepositorioTransaccionQuiosco repositorioTransaccionQuiosco, IRepositorioValidaAduana repositorioValidaAduana, IConector conector, IRepositorioN4 repositorioN4)
        {
            RepositorioPreGate = repositorioPreGate;
            RepositorioTransaccionQuiosco = repositorioTransaccionQuiosco;
            RepositorioValidaAduana = repositorioValidaAduana;
            Conector = conector;
            RepositorioN4 = repositorioN4;
        }

        public RespuestaN4 Procesar(long id, KIOSK kiosco)
        {
            Id = id;
            Kiosco = kiosco;
            Procesos = new List<PROCESS>();
            _estado = new EstadoVerificarExistenciaId();
            TosProcess = new TOS_PROCCESS();
            return _estado.Ejecutar(this);
        }

        public Respuesta RegistrarProceso(KIOSK_TRANSACTION transaccion)
        {
            using (var transaccionScope = new TransactionScope())
            {
                var transaccionQuiosco = RepositorioTransaccionQuiosco.ObtenerObjetos(new FiltroTransaccionQuioscoPorId(transaccion.TRANSACTION_ID)).FirstOrDefault();
                if (transaccionQuiosco == null)
                    return new Respuesta { Mensaje = $"No existe transacción de quiosco # {transaccion.TRANSACTION_ID}" };
                var fecha = DateTime.Now;
                var proceso = transaccion.PROCESSES.FirstOrDefault();
                if (proceso == null)
                    return new Respuesta { Mensaje = "No existe proceso para la transacción." };
                transaccionQuiosco.END_DATE = fecha;
                transaccionQuiosco.IS_OK = transaccion.IS_OK;
                transaccionQuiosco.PROCESSES.Add(new PROCESS
                {
                    IS_OK = proceso.IS_OK,
                    MESSAGE_ID = proceso.MESSAGE_ID,
                    RESPONSE = proceso.RESPONSE,
                    STEP = proceso.STEP,
                    STEP_DATE = fecha
                });
                RepositorioTransaccionQuiosco.Actualizar(transaccionQuiosco);
                ActualizarEntrada(transaccionQuiosco.PRE_GATE_ID.Value, transaccionQuiosco.IS_OK);
                transaccionScope.Complete();
                return new Respuesta { FueOk = true, Mensaje = "Registro Ok." };
            }
        }

        private void ActualizarEntrada(long idPreGate, bool procesoFinalizado)
        {
            var preGate = RepositorioPreGate.ObtenerObjetos(new FiltroPreGatePorId(idPreGate)).FirstOrDefault();
            if (procesoFinalizado)
                preGate.STATUS = "I";
            else
                return;
            RepositorioPreGate.Actualizar(preGate);
        }

        public  void ActualizarEntradaDepot(long idPreGate, string status)
        {
            var preGate = RepositorioPreGate.ObtenerObjetos(new FiltroPreGatePorId(idPreGate)).FirstOrDefault();
            preGate.STATUS = status;
            RepositorioPreGate.Actualizar(preGate);
        }

        internal RespuestaN4 SetearEstado(EstadoKioscoVacio estado)
        {
            _estado = estado;
            return _estado.Ejecutar(this);
        }

        internal int GrabarRegistroProceso(bool fueOk)
        {
            var transaccion = new KIOSK_TRANSACTION
            {
                PRE_GATE_ID = Id,
                END_DATE = DateTime.Now,
                IS_OK = fueOk,
                KIOSK_ID = Kiosco.KIOSK_ID,
                START_DATE = Procesos.FirstOrDefault().STEP_DATE,
                PROCESSES = Procesos
            };
            RepositorioTransaccionQuiosco.Agregar(transaccion);
            return transaccion.TRANSACTION_ID;
        }

        public void LiberarRecursos()
        {
            RepositorioPreGate.LiberarRecursos();
            RepositorioTransaccionQuiosco.LiberarRecursos();
        }
    }

    internal abstract class EstadoKioscoVacio
    {
        internal abstract RespuestaN4 Ejecutar(ServicioGeneral servicioGeneral);
    }

    internal class EstadoVerificarExistenciaId : EstadoKioscoVacio
    {
        internal override RespuestaN4 Ejecutar(ServicioGeneral servicioGeneral)
        {
            servicioGeneral.PreGate = servicioGeneral.RepositorioPreGate.ObtenerPreGateConDetalle(new FiltroPreGatePorId(servicioGeneral.Id.Value)).FirstOrDefault();
            if (servicioGeneral.PreGate == null)
            {
                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeNoExisteCodigo),
                    STEP = "PRE_GATE",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = ""
                });
                servicioGeneral.Id = null;
                return new RespuestaN4 { Mensaje = Mensajes.MensajeNoExisteCodigo, IdTransaccion = servicioGeneral.GrabarRegistroProceso(false) };
            }
            return servicioGeneral.SetearEstado(new EstadoValidarTipoTransaccion());
        }
    }

    public class FiltroPreGatePorId : Filtros<PRE_GATE>
    {
        private readonly long _id;

        internal FiltroPreGatePorId(long id)
        {
            _id = id;
        }

        public override Expression<Func<PRE_GATE, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<PRE_GATE>(p => p.PRE_GATE_ID == _id);
            return filtro.SastifechoPor();
        }
    }

    internal class EstadoValidarTipoTransaccion : EstadoKioscoVacio
    {
        internal override RespuestaN4 Ejecutar(ServicioGeneral servicioGeneral)
        {
            if (servicioGeneral.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 10) || servicioGeneral.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 11) || servicioGeneral.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 19))
                return servicioGeneral.SetearEstado(new EstadoValidarSalidaKiosco());
            servicioGeneral.Procesos.Add(new PROCESS
            {
                MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeTipoTransaccionesNoContempladas),
                STEP = "PRE_GATE",
                STEP_DATE = DateTime.Now,
                RESPONSE = $"El tipo de transacción del código ingresado es {servicioGeneral.PreGate.PRE_GATE_DETAILS.FirstOrDefault().TRANSACTION_TYPE.DESCRIPTION}"
            });
            return new RespuestaN4 { Mensaje = Mensajes.MensajeTipoTransaccionesNoContempladas, IdTransaccion = servicioGeneral.GrabarRegistroProceso(false) };
        }
    }

    internal class EstadoValidarSalidaKiosco : EstadoKioscoVacio
    {
        internal override RespuestaN4 Ejecutar(ServicioGeneral servicioGeneral)
        {
            if (servicioGeneral.PreGate.STATUS == "O")
            {
                if (servicioGeneral.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.STATUS == "O") || servicioGeneral.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.STATUS == "N"))
                {
                    servicioGeneral.Procesos.Add(new PROCESS
                    {
                        MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeSalidaExistente),
                        STEP = "PRE_GATE",
                        STEP_DATE = DateTime.Now,
                        RESPONSE = ""
                    });
                    return new RespuestaN4 { Mensaje = Mensajes.MensajeSalidaExistente, IdTransaccion = servicioGeneral.GrabarRegistroProceso(false) };
                }
                if (servicioGeneral.RepositorioN4.TieneTransaccionActivaPorPlaca(servicioGeneral.PreGate.TRUCK_LICENCE))
                {
                    servicioGeneral.Procesos.Add(new PROCESS
                    {
                        MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeVisitaActivaN4),
                        STEP = "PRE_GATE",
                        STEP_DATE = DateTime.Now,
                        RESPONSE = ""
                    });
                    return new RespuestaN4 { Mensaje = Mensajes.MensajeVisitaActivaN4, IdTransaccion = servicioGeneral.GrabarRegistroProceso(false) };
                }
                return servicioGeneral.SetearEstado(new EstadoGrabarPreGateEntregaVacio());
            }
            return servicioGeneral.SetearEstado(new EstadoValidarEntradaKiosco());
        }
    }

    internal class EstadoValidarEntradaKiosco : EstadoKioscoVacio
    {
        internal override RespuestaN4 Ejecutar(ServicioGeneral servicioGeneral)
        {
            if (servicioGeneral.PreGate.STATUS == "I")
            {
                if (servicioGeneral.PreGate.KIOSK_TRANSACTIONS.Any(kt => kt.IS_OK && !kt.KIOSK.IS_IN))
                {
                    servicioGeneral.Procesos.Add(new PROCESS
                    {
                        MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeYaEfectuoDespacho),
                        STEP = "PRE_GATE",
                        STEP_DATE = DateTime.Now,
                        RESPONSE = ""
                    });
                    return new RespuestaN4 { Mensaje = Mensajes.MensajeYaEfectuoDespacho, IdTransaccion = servicioGeneral.GrabarRegistroProceso(false) };
                }
                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajePreGateOk),
                    STEP = "PRE_GATE",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = "",
                    IS_OK = true
                });
                return servicioGeneral.SetearEstado(new EstadoMixtaVacio());
            }
            servicioGeneral.Procesos.Add(new PROCESS
            {
                MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeNoExisteEntradaQuiosco),
                STEP = "PRE_GATE",
                STEP_DATE = DateTime.Now,
                RESPONSE = ""
            });
            return new RespuestaN4 { Mensaje = Mensajes.MensajeNoExisteEntradaQuiosco, IdTransaccion = servicioGeneral.GrabarRegistroProceso(false) };
        }
    }

    internal class EstadoMixtaVacio : EstadoKioscoVacio
    {
        internal override RespuestaN4 Ejecutar(ServicioGeneral servicioGeneral)
        {
            string xml = string.Empty;
            string v_etiqueta = string.Empty;
            string v_contenedor = string.Empty;
            string[] separadas;

            if (servicioGeneral.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 10))
            {
                TOS_PROCCESS _tosProcessfiltrado = new TOS_PROCCESS();
                _tosProcessfiltrado = servicioGeneral.PreGate.TOS_PROCCESSES.Where(pgd => pgd.STEP == "SUBMIT").FirstOrDefault();

                if (_tosProcessfiltrado != null)                {
                    xml = _tosProcessfiltrado.RESPONSE.ToString();
                }else{
                    throw new Exception("_No existe info. Submit en Tos_Process, No se pudo verificar si el contener es FCL o MTY, Pregate : " + servicioGeneral.PreGate.PRE_GATE_ID.ToString());
                }
                
                try {
                    v_etiqueta = xml.Substring(xml.IndexOf("freight-kind=") + 14, 3);
                    
                    try { 
                        v_contenedor = xml.Substring(xml.IndexOf("eqid=") + 6, 11);
                        v_contenedor = v_contenedor.Substring(0, 11);
                        separadas = v_contenedor.Split('"');
                        v_contenedor = separadas[0];
                    }catch{v_contenedor = string.Empty;}
                }
                catch(Exception ex){
                    throw new Exception("Error al leer XML del TOS_Process atributo SUBMIT, No se pudo verificar si el contener es FCL o MTY" + ex.Message);
                }


                if (v_etiqueta == "MTY")
                {
                    return servicioGeneral.SetearEstado(new EstadoImdt());
                }
                else
                {
                    if (v_etiqueta == "FCL" || v_etiqueta == "LCL")
                    {
                        return servicioGeneral.SetearEstado(new EstadoIIE(v_contenedor));//IIE
                    }
                }
            }

            if (servicioGeneral.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 19))//transaccion expo full --> Devilery Depot 
            {
                foreach (var detalle in servicioGeneral.PreGate.PRE_GATE_DETAILS.Where( pgd => pgd.REFERENCE_ID == "E"))
                {
                    if (!servicioGeneral.RepositorioN4.EstaEnPatioContenedor(detalle.CONTAINERS.FirstOrDefault().NUMBER))
                        return new RespuestaN4 { Mensaje = Mensajes.MensajeContendorNoEstaEnPatio, IdTransaccion = servicioGeneral.GrabarRegistroProceso(false) };

                    v_contenedor = detalle.CONTAINERS.FirstOrDefault().NUMBER;
                }
                return servicioGeneral.SetearEstado(new EstadoIIE(v_contenedor));//IIE
                //return servicioGeneral.SetearEstado(new EstadoActualizarPreGate());
            }

            servicioGeneral.PreGateEntregaVacio = servicioGeneral.PreGate;
            return servicioGeneral.SetearEstado(new EstadoConsultaTransaccionActivaN4());///tran 11
        }
    }

    internal class EstadoImdt : EstadoKioscoVacio
    {
        internal override RespuestaN4 Ejecutar(ServicioGeneral servicioGeneral)
        {
            foreach (var detalle in servicioGeneral.PreGate.PRE_GATE_DETAILS.Where(pgd => pgd.REFERENCE_ID == "E"))
            {
                var xml = ArmarXml(servicioGeneral.Kiosco.NAME, servicioGeneral.RepositorioValidaAduana.ObtenerGKeyContenedor(detalle.CONTAINERS.FirstOrDefault().NUMBER));
                var servicioWeb = new ServicioAduana.n4ServiceSoapClient();
                var respuesta = servicioWeb.basicInvoke(ConfigurationManager.AppSettings["scopeCoordinateIds"], xml);
#if DEBUG
                if (respuesta.Contains("No esta disponible la informacion de N4 para esta unidad por favor intente en unos minutos"))
                {
                    servicioGeneral.Procesos.Add(new PROCESS
                    {
                        MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeImdtOk),
                        STEP = "IMDT",
                        STEP_DATE = DateTime.Now,
                        RESPONSE = $"Contenedor:{detalle.CONTAINERS.FirstOrDefault().NUMBER} {respuesta}",
                        IS_OK = true
                    });
                }
                else
                {
                    servicioGeneral.Procesos.Add(new PROCESS
                    {
                        MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeErrorImdt),
                        STEP = "IMDT",
                        STEP_DATE = DateTime.Now,
                        RESPONSE = $"Contenedor:{detalle.CONTAINERS.FirstOrDefault().NUMBER} {respuesta}"
                    });
                    return new RespuestaN4 { Mensaje = Mensajes.MensajeErrorImdt, IdTransaccion = servicioGeneral.GrabarRegistroProceso(false) };
                }
#else
                if (respuesta.Contains(@"Code>0</Code"))
                {
                    servicioGeneral.Procesos.Add(new PROCESS
                    {
                        MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeImdtOk),
                        STEP = "IMDT",
                        STEP_DATE = DateTime.Now,
                        RESPONSE = $"Contenedor:{detalle.CONTAINERS.FirstOrDefault().NUMBER} {respuesta}",
                        IS_OK = true
                    });
                }
                else
                {
                    servicioGeneral.Procesos.Add(new PROCESS
                    {
                        MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeErrorImdt),
                        STEP = "IMDT",
                        STEP_DATE = DateTime.Now,
                        RESPONSE = $"Contenedor:{detalle.CONTAINERS.FirstOrDefault().NUMBER} {respuesta}"
                    });
                    return new RespuestaN4 { Mensaje = Mensajes.MensajeErrorImdt, IdTransaccion = servicioGeneral.GrabarRegistroProceso(false) };
                }
#endif
            }
            return servicioGeneral.SetearEstado(new EstadoProcessTruckReceiveEmpty());
        }

        private string ArmarXml(string nombreKiosco, long gkey)
        {
            return $@"<imdt><tipo>E</tipo><usuario>{nombreKiosco}</usuario><validar>0</validar><parametros><gkey>{gkey}</gkey></parametros></imdt>";
        }
    }

    //<JGUSQUI 2019-04-26>
    //CREACION DE PROCESO IIE PARA TRANSACCION MIXTA - IN FULL + OUT EMPLY

    internal class EstadoIIE : EstadoKioscoVacio
    {
        private string _contenedor = string.Empty;
        private bool v_Error_IIE = false;

        public EstadoIIE(string conten)
        {
            _contenedor = conten;
        }

        internal override RespuestaN4 Ejecutar(ServicioGeneral servicioGeneral)
        {
            foreach (var detalle in servicioGeneral.PreGate.PRE_GATE_DETAILS.Where(pgd => pgd.REFERENCE_ID == "E"))
            {
                long _gkey = -1;
                try
                {
                    if (servicioGeneral.RepositorioValidaAduana is null)
                    {
                        throw new Exception(" - Error al obtener RepositorioValidaAduana referencia null");
                    }
                    _gkey = servicioGeneral.RepositorioValidaAduana.ObtenerGKeyContenedor(detalle.TRANSACTION_NUMBER);

                }
                catch (Exception ex)
                {
                    string _Message = ex.Message + " - No se pudo consultar GKey del contenedor "; //+ detalle.CONTAINERS is null? _contenedor : detalle.CONTAINERS.FirstOrDefault().NUMBER.ToString();
                    servicioGeneral.Procesos.Add(new PROCESS
                    {
                        MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeErrorN4),
                        STEP = "IIE",
                        STEP_DATE = DateTime.Now,
                        RESPONSE = $"Contenedor:{detalle.TRANSACTION_NUMBER} {_Message}"
                    });
                    
                    v_Error_IIE = true;
                }
                
                var xml = ArmarXml(servicioGeneral.Kiosco.NAME is null ? "KIOSCO 00" : servicioGeneral.Kiosco.NAME, _gkey);
                var servicioWeb = new ServicioAduana.n4ServiceSoapClient();
                var respuesta = servicioWeb.basicInvoke(ConfigurationManager.AppSettings["scopeCoordinateIds"], xml);


                if (respuesta.Contains(@"Code>0</Code"))
                {
                    servicioGeneral.Procesos.Add(new PROCESS
                    {
                        MESSAGE_ID = 13,
                        STEP = "IIE",
                        STEP_DATE = DateTime.Now,
                        RESPONSE = $"Contenedor:{detalle.TRANSACTION_NUMBER} {respuesta}",
                        IS_OK = true
                    });
                }
                else
                {
                    servicioGeneral.Procesos.Add(new PROCESS
                    {
                        MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeErrorN4),
                        STEP = "IIE",
                        STEP_DATE = DateTime.Now,
                        RESPONSE = $"Contenedor  :{detalle.TRANSACTION_NUMBER} {respuesta}",//$"Contenedor:{detalle.CONTAINERS.FirstOrDefault().NUMBER} {respuesta}"
                         IS_OK = false
                    });

                    v_Error_IIE = true;
                }               
            }

            var _respuesta = servicioGeneral.SetearEstado(new EstadoSalidaProcessTruckFull());

            if (v_Error_IIE)
            {
                _respuesta.Mensaje = "ERROR_IIE - " + _respuesta.Mensaje;
            }

            return _respuesta;
        }

        public class EstadoSalidaProcessTruckFull : EstadoKioscoVacio
        {
            internal override RespuestaN4 Ejecutar(ServicioGeneral servicioGeneral)
            {
                var xml = ArmarXml(servicioGeneral);
                var respuesta = servicioGeneral.Conector.Invocacion(xml);
                if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
                {
                    servicioGeneral.Procesos.Add(new PROCESS
                    {
                        MESSAGE_ID = 13,
                        STEP = "PROCESO_N4",
                        STEP_DATE = DateTime.Now,
                        IS_OK = true,
                        RESPONSE = respuesta.ResultadosConsultas.FirstOrDefault().Result
                    });
                    servicioGeneral.XmlRecepcion = respuesta.ResultadosConsultas.FirstOrDefault().Result;
                }
                else
                {
                    servicioGeneral.Procesos.Add(new PROCESS
                    {
                        MESSAGE_ID =Convert.ToInt32(Mensajes.MensajeErrorN4),
                        STEP = "PROCESO_N4",
                        STEP_DATE = DateTime.Now,
                        IS_OK = false,
                        RESPONSE =  $"N4 RETORNO ERROR EN PROCESS TRUCK :{respuesta.ToString()}" 
                    });

                    return new RespuestaN4 { Mensaje = Mensajes.MensajeErrorN4, IdTransaccion = servicioGeneral.GrabarRegistroProceso(false),FueOk = false,PreGate = servicioGeneral.PreGate, Xml = respuesta.ResultadosConsultas.FirstOrDefault().Result, IdPreGateRecepcion = servicioGeneral.Id.Value };
                }

                return servicioGeneral.SetearEstado(new EstadoActualizarPreGate());
            }

            public static string ArmarXml(ServicioGeneral datos)
            {
                var fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                return $@"<gate>
    	                <process-truck scan-status=""0"" no-content=""false"">
                            <gate-id>RECEPTIO</gate-id>
		                    <stage-id>outgate</stage-id>
                            <lane-id>{datos.Kiosco.NAME}</lane-id>
		                    <truck license-nbr=""{datos.PreGate.TRUCK_LICENCE}""/>
                            <driver license-nbr=""{datos.PreGate.DRIVER_ID}""/>
                            <truck-visit gos-tv-key=""{(datos.PreGate.PRE_GATE_ID * -1)}""/>
                            <timestamp>{fecha}</timestamp>
                        </process-truck>
                      </gate>";
            }
        }

        private string ArmarXml(string nombreKiosco, long gkey)
        {
            return $@"<iie><tipo>C</tipo><usuario>{nombreKiosco}</usuario><validar>1</validar><parametros><gkey>{gkey}</gkey><peso>1</peso></parametros></iie>";
        }

      
    }
    //</JGUSQUI 2019-04-26>

    internal class EstadoProcessTruckReceiveEmpty : EstadoKioscoVacio
    {
        internal override RespuestaN4 Ejecutar(ServicioGeneral servicioGeneral)
        {
            var xml = ArmarXml(servicioGeneral.Kiosco.NAME, servicioGeneral.PreGate);
            var respuesta = servicioGeneral.Conector.Invocacion(xml);
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
            {
                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeN4Ok),
                    STEP = "PROCESO N4",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = respuesta.ResultadosConsultas.FirstOrDefault().Result,
                    IS_OK = true
                });
                servicioGeneral.XmlRecepcion = respuesta.ResultadosConsultas.FirstOrDefault().Result;
                return servicioGeneral.SetearEstado(new EstadoActualizarPreGate());
            }
            else
            {
                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeErrorN4),
                    STEP = "PROCESO N4",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = $"N4 RETORNO ERROR EN PROCESS TRUCK :{respuesta.ToString()}"
                });
                return new RespuestaN4 { Mensaje = Mensajes.MensajeErrorN4, IdTransaccion = servicioGeneral.GrabarRegistroProceso(false) };
            }
        }

        private string ArmarXml(string nombreKiosco, PRE_GATE datos)
        {
#if DEBUG
            var fecha = DateTime.Now.ToString("yyyy-MM-ddTHH");
#else
            var fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
#endif
            return $@"<gate>
    	                <process-truck scan-status=""0"" no-content=""false"">
                            <gate-id>RECEPTIO</gate-id>
		                    <stage-id>outgate</stage-id>
                            <lane-id>{nombreKiosco}</lane-id>
		                    <truck license-nbr=""{datos.TRUCK_LICENCE}""/>
                            <driver license-nbr=""{datos.DRIVER_ID}""/>
                            <truck-visit gos-tv-key=""{(datos.PRE_GATE_ID * -1)}""/>
                            <timestamp>{fecha}</timestamp>
                        </process-truck>
                      </gate>";
        }
    }

    internal class EstadoActualizarPreGate : EstadoKioscoVacio
    {
        internal override RespuestaN4 Ejecutar(ServicioGeneral servicioGeneral)
        {
            using (var transaccion = new TransactionScope())
            {
                servicioGeneral.GrabarRegistroProceso(true);
                servicioGeneral.PreGate.STATUS = "O";       
                foreach (var item in servicioGeneral.PreGate.PRE_GATE_DETAILS)
                    item.STATUS = "P";
                servicioGeneral.RepositorioPreGate.Actualizar(servicioGeneral.PreGate);
                transaccion.Complete();
            }

            if (servicioGeneral.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 19))
            {
                return servicioGeneral.SetearEstado(new EstadoGrabarPreGateZAL());
            }

            return servicioGeneral.SetearEstado(new EstadoGrabarPreGateEntregaVacio());
        }
    }

    internal class EstadoGrabarPreGateEntregaVacio : EstadoKioscoVacio
    {
        internal override RespuestaN4 Ejecutar(ServicioGeneral servicioGeneral)
        {
            var detalles = new List<PRE_GATE_DETAIL>();
            short TRANSACCTYPE = 11;

            if (servicioGeneral.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 19))
            {
                TRANSACCTYPE = 19;
            }

            foreach (var detalle in servicioGeneral.PreGate.PRE_GATE_DETAILS.Where(pgd => pgd.REFERENCE_ID == "I"))
            {
                detalles.Add(new PRE_GATE_DETAIL
                {
                    DOCUMENT_ID = detalle.DOCUMENT_ID,
                    REFERENCE_ID = detalle.REFERENCE_ID,
                    STATUS = "N",
                    TRANSACTION_NUMBER = detalle.TRANSACTION_NUMBER,
                    TRANSACTION_TYPE_ID = TRANSACCTYPE
                });
            }
            servicioGeneral.PreGateEntregaVacio = new PRE_GATE
            {
                PRE_GATE_ID = servicioGeneral.RepositorioPreGate.ObtenerSecuenciaIdPreGate(),
                CREATION_DATE = DateTime.Now,
                DEVICE_ID = servicioGeneral.PreGate.DEVICE_ID,
                DRIVER_ID = servicioGeneral.PreGate.DRIVER_ID,
                PRE_GATE_DETAILS = detalles,
                STATUS = "C",
                TRUCK_LICENCE = servicioGeneral.PreGate.TRUCK_LICENCE,
                USER = servicioGeneral.Kiosco.NAME,
                PRE_GATE_ID_REF = servicioGeneral.PreGate.PRE_GATE_ID
            };
            servicioGeneral.RepositorioPreGate.Agregar(servicioGeneral.PreGateEntregaVacio);
            return servicioGeneral.SetearEstado(new EstadoConsultaTransaccionActivaN4());
        }
    }

    internal class EstadoGrabarPreGateZAL : EstadoKioscoVacio
    {
        internal override RespuestaN4 Ejecutar(ServicioGeneral servicioGeneral)
        {
            var detalles = new List<PRE_GATE_DETAIL>();
            foreach (var detalle in servicioGeneral.PreGate.PRE_GATE_DETAILS.Where(pgd => pgd.REFERENCE_ID == "I"))
            {
                detalles.Add(new PRE_GATE_DETAIL
                {
                    DOCUMENT_ID = detalle.DOCUMENT_ID,
                    REFERENCE_ID = detalle.REFERENCE_ID,
                    STATUS = "N",
                    TRANSACTION_NUMBER = detalle.TRANSACTION_NUMBER,
                    TRANSACTION_TYPE_ID = 17,//CISE
                    IS_RECYCLED = true
                });
            }
            servicioGeneral.PreGateEntregaVacio = new PRE_GATE
            {
                PRE_GATE_ID = servicioGeneral.RepositorioPreGate.ObtenerSecuenciaIdPreGate(),
                CREATION_DATE = DateTime.Now,
                DEVICE_ID = servicioGeneral.PreGate.DEVICE_ID,
                DRIVER_ID = servicioGeneral.PreGate.DRIVER_ID,
                PRE_GATE_DETAILS = detalles,
                STATUS = "C",
                TRUCK_LICENCE = servicioGeneral.PreGate.TRUCK_LICENCE,
                USER = servicioGeneral.Kiosco.NAME,
                PRE_GATE_ID_REF = servicioGeneral.PreGate.PRE_GATE_ID,
                IS_RECYCLED = true
            };
            //servicioGeneral.RepositorioPreGate.Agregar(servicioGeneral.PreGateEntregaVacio);
           
            servicioGeneral.Procesos.Add(new PROCESS
            {
                MESSAGE_ID = Convert.ToInt32(Mensajes.MensajePreGateOk),
                STEP = "PREGATE",
                STEP_DATE = DateTime.Now,
                RESPONSE = servicioGeneral.PreGate.PRE_GATE_ID.ToString(),
                IS_OK = true
            });

            servicioGeneral.RepositorioPreGate.Agregar(servicioGeneral.PreGateEntregaVacio);
            return servicioGeneral.SetearEstado(new EstadoConsultaTransaccionActivaN4());
        }
    }

    internal class EstadoConsultaTransaccionActivaN4 : EstadoKioscoVacio
    {
        internal override RespuestaN4 Ejecutar(ServicioGeneral servicioGeneral)
        {
            servicioGeneral.Procesos = new List<PROCESS>();
            var resultado = servicioGeneral.RepositorioN4.TieneTransaccionActiva(servicioGeneral.PreGateEntregaVacio.PRE_GATE_ID.ToString());
            if (resultado)
            {
                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeVisitaActivaN4),
                    STEP = "COMPROBAR_VISITA",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = ""
                });
                servicioGeneral.PreGateEntregaVacio.KIOSK_TRANSACTIONS.Add(new KIOSK_TRANSACTION { END_DATE = DateTime.Now, KIOSK_ID = servicioGeneral.Kiosco.KIOSK_ID, PROCESSES = servicioGeneral.Procesos, START_DATE = DateTime.Now });
                servicioGeneral.RepositorioPreGate.Actualizar(servicioGeneral.PreGateEntregaVacio);
                //cambio realizado para evitar que la transaccion se detenga
                return servicioGeneral.SetearEstado(new EstadoTruckVisit());
                //return new RespuestaN4 { Mensaje = Mensajes.MensajeVisitaActivaN4, IdTransaccion = servicioGeneral.PreGateEntregaVacio.KIOSK_TRANSACTIONS.LastOrDefault().TRANSACTION_ID };
            }
            else
            {
                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeN4Ok),
                    STEP = "COMPROBAR_VISITA",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = "",
                    IS_OK = true
                });
                if (servicioGeneral.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 19))
                {
                    return servicioGeneral.SetearEstado(new TruckVisitRecepcion());
                }
                return servicioGeneral.SetearEstado(new EstadoTruckVisit());
            }
        }
    }

    internal class EstadoTruckVisit : EstadoKioscoVacio
    {
        internal override RespuestaN4 Ejecutar(ServicioGeneral servicioGeneral)
        {
            servicioGeneral.Procesos = new List<PROCESS>();
            var xml = ArmarXml(servicioGeneral.PreGateEntregaVacio);
            var respuesta = servicioGeneral.Conector.Invocacion(xml);
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
            {
                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeN4Ok),
                    STEP = "TRUCK_VISIT",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = respuesta.ResultadosConsultas.FirstOrDefault().Result,
                    IS_OK = true
                });
                return servicioGeneral.SetearEstado(new EstadoGroovy());
            }
            else
            {
                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeErrorN4),
                    STEP = "TRUCK_VISIT",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = $"N4 RETORNO ERROR EN TRUCK_VISIT :{respuesta.ToString()}"
                });
                servicioGeneral.PreGateEntregaVacio.KIOSK_TRANSACTIONS.Add(new KIOSK_TRANSACTION { END_DATE = DateTime.Now, KIOSK_ID = servicioGeneral.Kiosco.KIOSK_ID, PROCESSES = servicioGeneral.Procesos, START_DATE = DateTime.Now });
                servicioGeneral.RepositorioPreGate.Actualizar(servicioGeneral.PreGateEntregaVacio);
                return new RespuestaN4 { Mensaje = Mensajes.MensajeErrorN4, IdTransaccion = servicioGeneral.PreGateEntregaVacio.KIOSK_TRANSACTIONS.LastOrDefault().TRANSACTION_ID };
            }
        }

        private string ArmarXml(PRE_GATE datos)
        {
#if DEBUG
            var fecha = DateTime.Now.ToString("yyyy-MM-ddTHH");
#else
            var fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
#endif
            return $@"<gate>
    	                <create-truck-visit>
		                    <gate-id>RECEPTIO_MTY</gate-id>
		                    <stage-id>mty_pre</stage-id>
		                    <truck license-nbr=""{datos.TRUCK_LICENCE}"" trucking-co-id=""{datos.PRE_GATE_DETAILS.FirstOrDefault().DOCUMENT_ID}""/>
                            <driver license-nbr=""{datos.DRIVER_ID}""/>
                            <truck-visit gos-tv-key=""{datos.PRE_GATE_ID * -1}""/>
                            <timestamp>{fecha}</timestamp>
                        </create-truck-visit>
                      </gate>";
        }
    }

    internal class EstadoGroovy : EstadoKioscoVacio
    {
        internal override RespuestaN4 Ejecutar(ServicioGeneral servicioGeneral)
        {
            foreach (var item in servicioGeneral.PreGateEntregaVacio.PRE_GATE_DETAILS)
            {
                var xml = ArmarXml(servicioGeneral.Kiosco.IP, item, servicioGeneral.PreGateEntregaVacio.PRE_GATE_ID);
                var respuesta = servicioGeneral.Conector.Invocacion(xml);
                if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("ERROR")))
                {
                    servicioGeneral.Procesos.Add(new PROCESS
                    {
                        MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeN4Ok),
                        STEP = "GROOVY",
                        STEP_DATE = DateTime.Now,
                        RESPONSE = respuesta.ResultadosConsultas.FirstOrDefault().Result,
                        IS_OK = true
                    });
                }
                else
                {
                    servicioGeneral.Procesos.Add(new PROCESS
                    {
                        MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeErrorN4),
                        STEP = "GROOVY",
                        STEP_DATE = DateTime.Now,
                        RESPONSE = $"N4 RETORNO ERROR EN GROOVY :{respuesta.ToString()}"
                    });
                    servicioGeneral.PreGateEntregaVacio.KIOSK_TRANSACTIONS.Add(new KIOSK_TRANSACTION { END_DATE = DateTime.Now, KIOSK_ID = servicioGeneral.Kiosco.KIOSK_ID, PROCESSES = servicioGeneral.Procesos, START_DATE = DateTime.Now });
                    servicioGeneral.RepositorioPreGate.Actualizar(servicioGeneral.PreGateEntregaVacio);
                    return new RespuestaN4 { Mensaje = Mensajes.MensajeErrorN4, IdTransaccion = servicioGeneral.PreGateEntregaVacio.KIOSK_TRANSACTIONS.LastOrDefault().TRANSACTION_ID };
                }
            }
            return servicioGeneral.SetearEstado(new EstadoStageDone1());
        }

        private string ArmarXml(string ip, PRE_GATE_DETAIL datos, long idPreGate)
        {
#if DEBUG
            var fecha = DateTime.Now.ToString("yyyy-MM-dd HH");
#else
            var fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
#endif
            var transaccion = datos.TRANSACTION_NUMBER.Split('-');
            return $@"<groovy class-name=""CGSAMTYGateTransactionInWS"" class-location=""code-extension"">
                          <parameters>
                            <parameter id=""gos-tv-key"" value=""{idPreGate * -1}""/>
                            <parameter id=""fecha"" value=""{fecha}""/>
                            <parameter id=""order-nbr"" value=""{transaccion[1]}""/>
                            <parameter id=""operator"" value=""{transaccion[1]}""/> 
                            <parameter id=""kiosco"" value=""{ip}""/>
                            <parameter id=""stage-id"" value=""mty_pre""/>
                            <parameter id=""nextstage-id"" value=""mty_in""/>
                            <parameter id=""iso-type"" value=""{transaccion[2]}""/>
                            <parameter id=""item-qty"" value=""1""/>
                            <parameter id=""numdocumento"" value=""{transaccion[0]}""/> 
                            <parameter id =""notes"" value="""" />
                          </parameters>
                      </groovy>";
        }
    }

    internal class EstadoStageDone1 : EstadoKioscoVacio
    {
        internal override RespuestaN4 Ejecutar(ServicioGeneral servicioGeneral)
        {
            var xml = ArmarXml(servicioGeneral.PreGateEntregaVacio);
            var respuesta = servicioGeneral.Conector.Invocacion(xml);
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
            {
                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeN4Ok),
                    STEP = "STAGE_DONE",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = respuesta.ResultadosConsultas.FirstOrDefault().Result,
                    IS_OK = true
                });
                return servicioGeneral.SetearEstado(new EstadoProcessTruckDeliveryEmptyIn());
            }
            else
            {
                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeErrorN4),
                    STEP = "STAGE_DONE",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = $"N4 RETORNO ERROR EN STAGE_DONE :{respuesta.ToString()}"
                });
                servicioGeneral.PreGateEntregaVacio.KIOSK_TRANSACTIONS.Add(new KIOSK_TRANSACTION { END_DATE = DateTime.Now, KIOSK_ID = servicioGeneral.Kiosco.KIOSK_ID, PROCESSES = servicioGeneral.Procesos, START_DATE = DateTime.Now });
                servicioGeneral.RepositorioPreGate.Actualizar(servicioGeneral.PreGateEntregaVacio);
                return new RespuestaN4 { Mensaje = Mensajes.MensajeErrorN4, IdTransaccion = servicioGeneral.PreGateEntregaVacio.KIOSK_TRANSACTIONS.LastOrDefault().TRANSACTION_ID };
            }
        }

        private string ArmarXml(PRE_GATE datos)
        {
            return $@"<gate>
    	                <stage-done>
		                    <gate-id>RECEPTIO_MTY</gate-id>
		                    <stage-id>mty_pre</stage-id>
                            <truck-visit gos-tv-key=""{datos.PRE_GATE_ID * -1}""/>
                        </stage-done>
                      </gate>";
        }
    }

    internal class EstadoProcessTruckDeliveryEmptyIn : EstadoKioscoVacio
    {
        internal override RespuestaN4 Ejecutar(ServicioGeneral servicioGeneral)
        {
            var xml = ArmarXml(servicioGeneral.Kiosco.NAME, servicioGeneral.PreGateEntregaVacio);
            var respuesta = servicioGeneral.Conector.Invocacion(xml);
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
            {
                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeN4Ok),
                    STEP = "PROCESS_TRUCK",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = respuesta.ResultadosConsultas.FirstOrDefault().Result,
                    IS_OK = true
                });
                return servicioGeneral.SetearEstado(new EstadoStageDone2());
            }
            else
            {
                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeErrorN4),
                    STEP = "PROCESS_TRUCK",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = $"N4 RETORNO ERROR EN PROCESS TRUCK :{respuesta.ToString()}"
                });
                servicioGeneral.PreGateEntregaVacio.KIOSK_TRANSACTIONS.Add(new KIOSK_TRANSACTION { END_DATE = DateTime.Now, KIOSK_ID = servicioGeneral.Kiosco.KIOSK_ID, PROCESSES = servicioGeneral.Procesos, START_DATE = DateTime.Now });
                servicioGeneral.RepositorioPreGate.Actualizar(servicioGeneral.PreGateEntregaVacio);
                return new RespuestaN4 { Mensaje = Mensajes.MensajeErrorN4, IdTransaccion = servicioGeneral.PreGateEntregaVacio.KIOSK_TRANSACTIONS.LastOrDefault().TRANSACTION_ID };
            }
        }

        private string ArmarXml(string nombreKiosco, PRE_GATE datos)
        {
#if DEBUG
            var fecha = DateTime.Now.ToString("yyyy-MM-ddTHH");
#else
            var fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
#endif
            return $@"<gate>
    	                <process-truck scan-status=""0"" no-content=""false"">
                            <gate-id>RECEPTIO_MTY</gate-id>
		                    <stage-id>mty_in</stage-id>
                            <lane-id>{nombreKiosco}</lane-id>
		                    <truck license-nbr=""{datos.TRUCK_LICENCE}""/>
                            <driver license-nbr=""{datos.DRIVER_ID}""/>
                            <truck-visit gos-tv-key=""{(datos.PRE_GATE_ID * -1)}""/>
                            <timestamp>{fecha}</timestamp>
                        </process-truck>
                      </gate>";
        }
    }

    internal class EstadoStageDone2 : EstadoKioscoVacio
    {
        internal override RespuestaN4 Ejecutar(ServicioGeneral servicioGeneral)
        {
            var xml = ArmarXml(servicioGeneral.Kiosco.NAME, servicioGeneral.PreGateEntregaVacio);
            var respuesta = servicioGeneral.Conector.Invocacion(xml);
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
            {
                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeN4Ok),
                    STEP = "STAGE_DONE_1",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = respuesta.ResultadosConsultas.FirstOrDefault().Result,
                    IS_OK = true
                });
                return servicioGeneral.SetearEstado(new EstadoStageDone3());
            }
            else
            {
                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeErrorN4),
                    STEP = "STAGE_DONE_1",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = $"N4 RETORNO ERROR EN STAGE DONE 1 :{respuesta.ToString()}"
                });
                servicioGeneral.PreGateEntregaVacio.KIOSK_TRANSACTIONS.Add(new KIOSK_TRANSACTION { END_DATE = DateTime.Now, KIOSK_ID = servicioGeneral.Kiosco.KIOSK_ID, PROCESSES = servicioGeneral.Procesos, START_DATE = DateTime.Now });
                servicioGeneral.RepositorioPreGate.Actualizar(servicioGeneral.PreGateEntregaVacio);
                return new RespuestaN4 { Mensaje = Mensajes.MensajeErrorN4, IdTransaccion = servicioGeneral.PreGateEntregaVacio.KIOSK_TRANSACTIONS.LastOrDefault().TRANSACTION_ID };
            }
        }

        private string ArmarXml(string nombreKiosco, PRE_GATE datos)
        {
            return $@"<gate>
    	                <stage-done>
		                    <gate-id>RECEPTIO_MTY</gate-id>
		                    <stage-id>mty_yard</stage-id>
                            <lane-id>{nombreKiosco}</lane-id>
                            <truck-visit gos-tv-key=""{datos.PRE_GATE_ID * -1}""/>
                        </stage-done>
                      </gate>";
        }
    }

    internal class EstadoStageDone3 : EstadoKioscoVacio
    {
        internal override RespuestaN4 Ejecutar(ServicioGeneral servicioGeneral)
        {
            var xml = ArmarXml(servicioGeneral.Kiosco.NAME, servicioGeneral.PreGateEntregaVacio);
            var respuesta = servicioGeneral.Conector.Invocacion(xml);
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
            {
                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeN4Ok),
                    STEP = "STAGE_DONE_2",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = respuesta.ResultadosConsultas.FirstOrDefault().Result,
                    IS_OK = true
                });
                servicioGeneral.Xml = respuesta.ResultadosConsultas.FirstOrDefault().Result;
                return servicioGeneral.SetearEstado(new EstadoSmdt());
            }
            else
            {
                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeErrorN4),
                    STEP = "STAGE_DONE_2",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = $"N4 RETORNO ERROR EN STAGE DONE 2 :{respuesta.ToString()}"
                });
                servicioGeneral.PreGateEntregaVacio.KIOSK_TRANSACTIONS.Add(new KIOSK_TRANSACTION { END_DATE = DateTime.Now, KIOSK_ID = servicioGeneral.Kiosco.KIOSK_ID, PROCESSES = servicioGeneral.Procesos, START_DATE = DateTime.Now });
                servicioGeneral.RepositorioPreGate.Actualizar(servicioGeneral.PreGateEntregaVacio);
                return new RespuestaN4 { Mensaje = Mensajes.MensajeErrorN4, IdTransaccion = servicioGeneral.PreGateEntregaVacio.KIOSK_TRANSACTIONS.LastOrDefault().TRANSACTION_ID };
            }
        }

        private string ArmarXml(string nombreKiosco, PRE_GATE datos)
        {
            return $@"<gate>
    	                <stage-done>
		                    <gate-id>RECEPTIO_MTY</gate-id>
		                    <stage-id>mty_yard2</stage-id>
                            <lane-id>{nombreKiosco}</lane-id>
                            <truck-visit gos-tv-key=""{datos.PRE_GATE_ID * -1}""/>
                        </stage-done>
                      </gate>";
        }
    }

    internal class EstadoSmdt : EstadoKioscoVacio
    {
        internal override RespuestaN4 Ejecutar(ServicioGeneral servicioGeneral)
        {
            servicioGeneral.ContenedoresAsignados = new List<string>();
            foreach (var item in BaseXml.ObtenerEtiquetas(servicioGeneral.Xml, "container"))
            {
                servicioGeneral.ContenedoresAsignados.Add(item.Attributes().FirstOrDefault(x => x.Name == "eqid").Value);
                var xml = ArmarXml(servicioGeneral.Kiosco.NAME, servicioGeneral.RepositorioValidaAduana.ObtenerGKeyContenedorVacio(item.Attributes().FirstOrDefault(x => x.Name == "eqid").Value));
                var servicioWeb = new ServicioAduana.n4ServiceSoapClient();
                for (var i = 0; i < 3; i++)
                {
                    var respuesta = servicioWeb.basicInvoke(ConfigurationManager.AppSettings["scopeCoordinateIds"], xml);
                    if (respuesta.Contains(@"Code>0</Code"))
                    {
                        servicioGeneral.Procesos.Add(new PROCESS
                        {
                            MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeSmdtOk),
                            STEP = "SMDT",
                            STEP_DATE = DateTime.Now,
                            RESPONSE = $"Contenedor:{item.Attributes().FirstOrDefault(x => x.Name == "eqid").Value} {respuesta}",
                            IS_OK = true
                        });
                        break;
                    }
                    else
                    {
                        servicioGeneral.Procesos.Add(new PROCESS
                        {
                            MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeErrorSmdt),
                            STEP = "SMDT",
                            STEP_DATE = DateTime.Now,
                            RESPONSE = $"{xml}////Contenedor:{item.Attributes().FirstOrDefault(x => x.Name == "eqid").Value} {respuesta}",
                        });
                    }
                }
            }
            var transaccionKiosco = new KIOSK_TRANSACTION { END_DATE = DateTime.Now, KIOSK_ID = servicioGeneral.Kiosco.KIOSK_ID, PROCESSES = servicioGeneral.Procesos, START_DATE = DateTime.Now, PRE_GATE_ID = servicioGeneral.PreGateEntregaVacio.PRE_GATE_ID };
            if (servicioGeneral.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 10))
                foreach (var item in servicioGeneral.PreGate.PRE_GATE_DETAILS)
                    item.STATUS = "O";
            using (var transaccion = new TransactionScope())
            {
                if (servicioGeneral.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 10))
                    servicioGeneral.RepositorioPreGate.Actualizar(servicioGeneral.PreGate);
                servicioGeneral.PreGateEntregaVacio.STATUS = "P";
                var contador = 0;
                foreach (var item in servicioGeneral.PreGateEntregaVacio.PRE_GATE_DETAILS)
                {
                    item.CONTAINERS.Add(new CONTAINER { NUMBER = servicioGeneral.ContenedoresAsignados[contador] } );
                    contador++;
                }
                servicioGeneral.RepositorioPreGate.Actualizar(servicioGeneral.PreGateEntregaVacio);
                servicioGeneral.RepositorioTransaccionQuiosco.Agregar(transaccionKiosco);
                transaccion.Complete();
            }
            return new RespuestaN4 { Mensaje = Mensajes.MensajeN4Ok, FueOk = true, Xml = servicioGeneral.Xml, IdTransaccion = transaccionKiosco.TRANSACTION_ID, PreGate = servicioGeneral.PreGateEntregaVacio, XmlRecepcion = ObtenerXmlRecepcion(servicioGeneral), IdPreGateRecepcion = servicioGeneral.Id.Value };
        }

        private string ArmarXml(string nombreKiosco, long gkey)
        {
            return $@"<smdt><tipo>C</tipo><usuario>{nombreKiosco}</usuario><validar>0</validar><parametros><gkey>{gkey}</gkey><peso>0</peso></parametros></smdt>";
        }

        private string ObtenerXmlRecepcion(ServicioGeneral servicioGeneral)
        {
            if (servicioGeneral.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 11))
                return null;
            else if (string.IsNullOrWhiteSpace(servicioGeneral.XmlRecepcion))
                return servicioGeneral.PreGate.KIOSK_TRANSACTIONS.Where(kt => kt.IS_OK).OrderBy(kt => kt.END_DATE).FirstOrDefault().PROCESSES.FirstOrDefault(p => p.STEP == "PROCESO_N4").RESPONSE;
            return servicioGeneral.XmlRecepcion;
        }
    }

    internal class TruckVisitRecepcion : EstadoKioscoVacio
    {
        internal override RespuestaN4 Ejecutar(ServicioGeneral servicioGeneral)
        {
            
            var xml = ArmarXmlTruckVisitRecepcion(servicioGeneral);
            var respuesta = servicioGeneral.Conector.Invocacion(xml);

            if (!respuesta.ResultadosConsultas.Any(c => c.Result.Contains(@"status=""OK""")))
            {
                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeErrorN4),
                    STEP = "PROCESO N4",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = $"N4 RETORNO ERROR EN TRUCK VISIT RECEPCION ZAL :{respuesta.ToString()}"
                });
                //var v_trans = servicioGeneral.RepositorioTransaccionQuiosco.ObtenerObjetos(new FiltroTransaccionPorId(servicioGeneral.IdTransaccion)).FirstOrDefault();
                //servicioGeneral.Procesos.AddRange(v_trans.PROCESSES);
                //v_trans.PROCESSES = servicioGeneral.Procesos;
                //servicioGeneral.RepositorioTransaccionQuiosco.Actualizar(v_trans);
                //return new RespuestaN4Depot { Mensaje = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeErrorN4))).FirstOrDefault().TROUBLE_DESK_MESSAGE, MensajeDetalle = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeErrorN4))).FirstOrDefault().DETAILS, IdTransaccion = servicioGeneral.IdTransaccion };
                servicioGeneral.PreGateEntregaVacio.KIOSK_TRANSACTIONS.Add(new KIOSK_TRANSACTION { END_DATE = DateTime.Now, KIOSK_ID = servicioGeneral.Kiosco.KIOSK_ID, PROCESSES = servicioGeneral.Procesos, START_DATE = DateTime.Now });
                servicioGeneral.RepositorioPreGate.Actualizar(servicioGeneral.PreGateEntregaVacio);
                return new RespuestaN4 { Mensaje = Mensajes.MensajeErrorN4, IdTransaccion = servicioGeneral.PreGateEntregaVacio.KIOSK_TRANSACTIONS.LastOrDefault().TRANSACTION_ID };
            }
            else
            {
                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeN4Ok),
                    STEP = "PROCESO N4",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = respuesta.ResultadosConsultas.FirstOrDefault().Result,
                    IS_OK = true
                });
                return servicioGeneral.SetearEstado(new EstadoStageDone1Depot());
            }
        }

        private string ArmarXmlTruckVisitRecepcion(ServicioGeneral servicioGeneral)
        {
            var Placa = servicioGeneral.PreGateEntregaVacio.TRUCK_LICENCE.ToString();
            var bat = Placa.Substring(1, Placa.Length - 1);
            var fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            return $@"<gate>
                        <create-truck-visit>
                        <gate-id>CISE</gate-id>
                        <stage-id>ingate_cise</stage-id>
                        <lane-id>KIOSCO 02</lane-id>
		                <truck license-nbr=""{servicioGeneral.PreGateEntregaVacio.TRUCK_LICENCE.ToUpper()}""/>
		                <driver license-nbr=""{servicioGeneral.PreGateEntregaVacio.DRIVER_ID}""/>
		                <truck-visit gos-tv-key=""{servicioGeneral.PreGateEntregaVacio.PRE_GATE_ID * -1}"" bat-nbr=""{bat.ToUpper()}""/>
		                <timestamp>{fecha}</timestamp>
	                    </create-truck-visit>
                       </gate>";
        }
    }

    internal class EstadoStageDone1Depot : EstadoKioscoVacio
    {
        internal override RespuestaN4 Ejecutar(ServicioGeneral servicioGeneral)
        {
            //var ServicioAnunciante = new ServicioAnuncianteProblema.ServicioAnuncianteProblemaClient();
            var xml = ArmarXml(servicioGeneral.PreGateEntregaVacio);
            var respuesta = servicioGeneral.Conector.Invocacion(xml);
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
            {
                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeN4Ok),
                    STEP = "STAGE_DONE",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = respuesta.ResultadosConsultas.FirstOrDefault().Result,
                    IS_OK = true
                });
                servicioGeneral.ActualizarEntradaDepot(servicioGeneral.PreGateEntregaVacio.PRE_GATE_ID, "N");
                //servicioGeneral.XmlRecepcion = respuesta.ResultadosConsultas.FirstOrDefault().Result;

                var transaccionKiosco = new KIOSK_TRANSACTION { END_DATE = DateTime.Now, KIOSK_ID = servicioGeneral.Kiosco.KIOSK_ID, PROCESSES = servicioGeneral.Procesos, START_DATE = DateTime.Now, PRE_GATE_ID = servicioGeneral.PreGateEntregaVacio.PRE_GATE_ID };
                
                using (var transaccion = new TransactionScope())
                {
                    servicioGeneral.PreGateEntregaVacio.STATUS = "N";
                    servicioGeneral.RepositorioPreGate.Actualizar(servicioGeneral.PreGateEntregaVacio);
                    servicioGeneral.RepositorioTransaccionQuiosco.Agregar(transaccionKiosco);
                    transaccion.Complete();
                }
                return new RespuestaN4 { Mensaje = Mensajes.MensajeN4Ok, FueOk = true, Xml = servicioGeneral.Xml, IdTransaccion = transaccionKiosco.TRANSACTION_ID, PreGate = servicioGeneral.PreGateEntregaVacio, XmlRecepcion = servicioGeneral.XmlRecepcion, IdPreGateRecepcion = servicioGeneral.Id.Value };

            }
            else
            {
                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeErrorN4),
                    STEP = "STAGE_DONE",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = $"N4 RETORNO ERROR EN STAGE_DONE :{respuesta.ToString()}"
                });/*
                var v_trans = servicioGeneral.RepositorioTransaccionQuiosco.ObtenerObjetos(new FiltroTransaccionPorId(servicioGeneral.IdTransaccion)).FirstOrDefault();
                v_trans.PRE_GATE_ID = servicioGeneral.PreGate.PRE_GATE_ID;
                servicioGeneral.Procesos.AddRange(v_trans.PROCESSES);
                v_trans.PROCESSES = servicioGeneral.Procesos;
                servicioGeneral.RepositorioTransaccionQuiosco.Actualizar(v_trans);
                //ServicioAnunciante.AnunciarProblema(int.Parse(servicioGeneral.IdTransaccion.ToString()));
                return new RespuestaN4Depot { Mensaje = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeErrorN4))).FirstOrDefault().TROUBLE_DESK_MESSAGE, MensajeDetalle = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeErrorN4))).FirstOrDefault().DETAILS, IdTransaccion = servicioGeneral.IdTransaccion };
           */
                
                servicioGeneral.PreGateEntregaVacio.KIOSK_TRANSACTIONS.Add(new KIOSK_TRANSACTION { END_DATE = DateTime.Now, KIOSK_ID = servicioGeneral.Kiosco.KIOSK_ID, PROCESSES = servicioGeneral.Procesos, START_DATE = DateTime.Now });
                servicioGeneral.RepositorioPreGate.Actualizar(servicioGeneral.PreGateEntregaVacio);
                return new RespuestaN4 { Mensaje = Mensajes.MensajeErrorN4, IdTransaccion = servicioGeneral.PreGateEntregaVacio.KIOSK_TRANSACTIONS.LastOrDefault().TRANSACTION_ID };
            }
        }

        private string ArmarXml(PRE_GATE datos)
        {
            return $@"<gate>
        	                <stage-done>
    		                    <gate-id>CISE</gate-id>
    		                    <stage-id>ingate_cise</stage-id>
                                <lane-id>KIOSCO 02</lane-id>
                                <truck-visit gos-tv-key=""{datos.PRE_GATE_ID * -1}""/>
                            </stage-done>
                          </gate>";
        }
    }



    
}
