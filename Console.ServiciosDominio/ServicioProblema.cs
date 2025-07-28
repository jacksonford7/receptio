using RECEPTIO.CapaAplicacion.Console.Aplicacion.Interfaces;
using RECEPTIO.CapaDominio.Console.Dominio.InterfacesRepositorios;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaDominio.Nucleo.ServiciosDominio.Otros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RECEPTIO.CapaDominio.Console.ServiciosDominio
{
    public class ServicioProblema : AsignacionProblema, IProblema
    {
        internal readonly IRepositorioProcess RepositorioProcess;
        internal readonly IRepositorioUserSession RepositorioUserSession;
        internal readonly IRepositorioTroubleTicket RepositorioTroubleTicket;
        internal readonly IRepositorioError RepositorioError;
        private EstadoRegistroProblema _estado;
        internal int IdTransaccionQuiosco;
        internal PROCESS Proceso;
        internal List<USER_SESSION> SesionesUsuarios;
        internal int IdError;
        internal short IdZona;
        internal int IdAplicacion;
        internal ERROR Error;
        internal string MensajeError;
        internal long IdTosProceso;
        internal long? IdPreGate;

        public ServicioProblema(IRepositorioProcess repositorioProcess, IRepositorioUserSession repositorioUserSession, IRepositorioTroubleTicket repositorioTroubleTicket, IRepositorioError repositorioError)
        {
            RepositorioProcess = repositorioProcess;
            RepositorioUserSession = repositorioUserSession;
            RepositorioTroubleTicket = repositorioTroubleTicket;
            RepositorioError = repositorioError;
        }

        public Tuple<bool, string, string> RegistrarProblema(int idTransaccionQuiosco)
        {
            IdTransaccionQuiosco = idTransaccionQuiosco;
            _estado = new EstadoObtenerProceso();
            return _estado.Ejecutar(this);
        }

        public Tuple<bool, string, string> RegistrarProblemaMobile(long idTosProcess, short idZona)
        {
            IdTosProceso = idTosProcess;
            IdZona = idZona;
            _estado = new EstadoObtenerSesionesUsuariosParaMobileTicket();
            return _estado.Ejecutar(this);
        }

        public Tuple<bool, string, string> RegistrarProblemaGenericoMobile(string mensajeError, short idZona)
        {
            MensajeError = mensajeError;
            IdZona = idZona;
            _estado = new EstadoObtenerSesionesUsuariosParaMobileTicket();
            return _estado.Ejecutar(this);
        }

        public Tuple<bool, string, string> AnunciarProblemaTransaccionPendiente(long idPreGate, short idZona)
        {
            IdPreGate = idPreGate;
            IdZona = idZona;
            MensajeError = "Existe una transacción pendiente el proceso no continuará.-En RECEPTIO existe una transacción pendiente por completar-Verifique en N4 si existe una transacción activa de día usando los datos de cédula chofer, camión. Si existe entonces el auxiliar intento ingresar otra vez la transacción, comunique con él y expliquele el error, luego cierre el ticket. Si no existe es porque se registro en RECEPTIO pero nunca pasó por el kiosco, haga clic derecho en el ticket y cancele la transacción en RECEPTIO, luego comuniquese con el auxiliar para que vuelva a ingresar la transacción, finalmemte cierre el ticket.";
            _estado = new EstadoObtenerSesionesUsuariosParaMobileTicket();
            return _estado.Ejecutar(this);
        }

        public Tuple<bool, string, string> RegistrarProblemaClienteAppTransaction(int idError, short idZona)
        {
            IdError = idError;
            IdZona = idZona;
            _estado = new EstadoObtenerError();
            return _estado.Ejecutar(this);
        }

        public Tuple<bool, string, string> RegistrarProblemaServicioWebTransaction(string error, short idZona, int idAplicacion)
        {
            MensajeError = error;
            IdZona = idZona;
            IdAplicacion = idAplicacion;
            _estado = new EstadoObtenerSesionesUsuariosClienteAppTransaction();
            return _estado.Ejecutar(this);
        }

        public void LiberarRecursos()
        {
            RepositorioProcess.LiberarRecursos();
            RepositorioUserSession.LiberarRecursos();
            RepositorioTroubleTicket.LiberarRecursos();
            RepositorioError.LiberarRecursos();
        }

        internal Tuple<bool, string, string> SetearEstado(EstadoRegistroProblema estado)
        {
            _estado = estado;
            return _estado.Ejecutar(this);
        }

        internal USER_SESSION ObtenerSesionUsuarioAlgoritmo()
        {
            try
            {
                //SE OBTIENE LA SESION DEL USUARIO TD AL QUE SE LE VA ASIGNAR EL TICKET
                //EL TERCER VALOR DEL RESULTADO INDICA SI YA EXISTE UN TICKET ABIERTO (EVITA REPETICION DE TICKET) - VALIDO 0/ REPETIDO 1
                var v_Result = RepositorioError.ObtenerValidacionesGenerales("OBTIENE_SESION_TD", String.Empty, 0, 0);
                if (!string.IsNullOrEmpty(v_Result))
                {
                    var oResult = v_Result.Split('-');
                    string v_Strvalor = $@"<PARAMETROS OPERATION_NAME=""ServicioProblema""
                                                ACTION_NAME=""OBTIENE_SESION_TD""
                                                METHOD_NAME=""ObtenerSesionUsuarioAlgoritmo""
                                                LOG_SECUENCE_ID=""{oResult[0].ToString()}""
                                                OPERATION_NOTES=""{"Result: " + v_Result }""
                                                RESULTA=""1""
                                                RESULT_MESSAGE=""{oResult[1]}""/>";
                    var v_resultado = RepositorioError.ObtenerValidacionesGenerales("REGISTRA_LOG", v_Strvalor, 0, 0);
                    return new USER_SESSION { ID = long.Parse(oResult[0]), DEVICE = new DEVICE { IP = oResult[1] } };
                }
                return ObtenerSesionUsuarioAlgoritmo(SesionesUsuarios);
            }
            catch
            {
                return ObtenerSesionUsuarioAlgoritmo(SesionesUsuarios);
            }
        }

        internal string validaTicketRepetido(long idProcess)
        {
            string v_resultado = string.Empty;
            try
            {
                var v_Result = RepositorioError.ObtenerValidacionesGenerales("VALIDA_TICKET_REPETIDO", String.Empty, 0, idProcess);
                v_resultado = v_Result;
            }
            catch
            {
                v_resultado = string.Empty;
            }
            return v_resultado;
        }

    }

    internal abstract class EstadoRegistroProblema
    {
        internal abstract Tuple<bool, string, string> Ejecutar(ServicioProblema servicio);
    }

    internal class EstadoObtenerProceso : EstadoRegistroProblema
    {
        internal override Tuple<bool, string, string> Ejecutar(ServicioProblema servicio)
        {
            servicio.Proceso = servicio.RepositorioProcess.ObtenerProcesoConMensaje(new FiltroProcesoPorIdTransaccion(servicio.IdTransaccionQuiosco)).OrderBy(p => p.PROCESS_ID).LastOrDefault();
            if (servicio.Proceso == null)
                throw new Exception($"No existe proceso cuyo id de transacción es : {servicio.IdTransaccionQuiosco}");
            return servicio.SetearEstado(new EstadoObtenerSesionesUsuarios());
        }
    }

    public class FiltroProcesoPorIdTransaccion : IFiltros<PROCESS>
    {
        private readonly int _idTransaccionQuiosco;

        public FiltroProcesoPorIdTransaccion(int idTransaccionQuiosco)
        {
            _idTransaccionQuiosco = idTransaccionQuiosco;
        }

        public Expression<Func<PROCESS, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<PROCESS>(p => p.TRANSACTION_ID == _idTransaccionQuiosco);
            return filtro.SastifechoPor();
        }
    }

    internal class EstadoObtenerSesionesUsuarios : EstadoRegistroProblema
    {
        internal override Tuple<bool, string, string> Ejecutar(ServicioProblema servicio)
        {
            servicio.SesionesUsuarios = servicio.RepositorioUserSession.ObtenerSesionUsuarioConDispositivo(new FiltroSesionUsuarioAbiertas(servicio.Proceso.KIOSK_TRANSACTION.KIOSK.ZONE_ID)).GroupBy(us => us.DEVICE_ID).Select(us => us.FirstOrDefault()).ToList();
            return servicio.SetearEstado(new EstadoRegistrarProblema());
        }
    }

    public class FiltroSesionUsuarioAbiertas : IFiltros<USER_SESSION>
    {
        private readonly short _idZona;

        public FiltroSesionUsuarioAbiertas(short idZona)
        {
            _idZona = idZona;
        }

        public Expression<Func<USER_SESSION, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<USER_SESSION>(us => !us.FINISH_SESSION_DATE.HasValue && us.DEVICE.ZONE_ID == _idZona && us.TROUBLE_DESK_USER.IS_TD && us.BREAKS.All(b => b.FINISH_BREAK_DATE.HasValue));
            return filtro.SastifechoPor();
        }
    }

    internal class EstadoRegistrarProblema : EstadoRegistroProblema
    {
        internal override Tuple<bool, string, string> Ejecutar(ServicioProblema servicio)
        {
            if (servicio.SesionesUsuarios.Count == 0)
                return RegistrarSoloProblema(servicio);
            return RegistrarProblemaConSesionUsuario(servicio);
        }

        private Tuple<bool, string, string> RegistrarSoloProblema(ServicioProblema servicio)
        {
            servicio.RepositorioTroubleTicket.CrearTicketProceso(new PROCESS_TROUBLE_TICKET
            {
                CREATION_DATE = DateTime.Now
            }, servicio.Proceso.PROCESS_ID);
            return new Tuple<bool, string, string>(false, "", "");
        }

        private Tuple<bool, string, string> RegistrarProblemaConSesionUsuario(ServicioProblema servicio)
        {
            var sesionUsuario = servicio.ObtenerSesionUsuarioAlgoritmo();

            try
            {
                var vResultado = servicio.validaTicketRepetido(servicio.Proceso.PROCESS_ID);
                if (!string.IsNullOrEmpty(vResultado))
                {
                    servicio.RepositorioTroubleTicket.CrearTicketProceso(new PROCESS_TROUBLE_TICKET
                    {
                        IS_CANCEL = true,
                        ACCEPTANCE_DATE = DateTime.Now,
                        FINISH_DATE = DateTime.Now,
                        NOTES = vResultado + " - CANCELACIÓN AUTOMATICA",
                        MOTIVE_ID = 52,
                        SUBMOTIVE_ID = 68,
                        ASSIGNMENT_DATE = DateTime.Now,
                        CREATION_DATE = DateTime.Now,
                    }, servicio.Proceso.PROCESS_ID);
                    return new Tuple<bool, string, string>(true, vResultado, sesionUsuario.DEVICE.IP);
                }
            }
            catch { }

            servicio.RepositorioTroubleTicket.CrearTicketProceso(new PROCESS_TROUBLE_TICKET
            {
                ASSIGNMENT_DATE = DateTime.Now,
                CREATION_DATE = DateTime.Now,
                USER_SESSION_ID = sesionUsuario.ID
            }, servicio.Proceso.PROCESS_ID);
            return new Tuple<bool, string, string>(true, servicio.Proceso.MESSAGE.TROUBLE_DESK_MESSAGE, sesionUsuario.DEVICE.IP);
        }
    }

    internal class EstadoObtenerError : EstadoRegistroProblema
    {
        internal override Tuple<bool, string, string> Ejecutar(ServicioProblema servicio)
        {
            servicio.Error = servicio.RepositorioError.ObtenerObjetos(new FiltroErrorPorId(servicio.IdError)).FirstOrDefault();
            if (servicio.Error == null)
                throw new Exception($"No existe error cuyo id es : {servicio.IdError}");
            return servicio.SetearEstado(new EstadoObtenerSesionesUsuariosClienteAppTransaction());
        }
    }

    public class FiltroErrorPorId : IFiltros<ERROR>
    {
        private readonly int _id;

        public FiltroErrorPorId(int id)
        {
            _id = id;
        }

        public Expression<Func<ERROR, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<ERROR>(e => e.ERROR_ID == _id);
            return filtro.SastifechoPor();
        }
    }

    internal class EstadoObtenerSesionesUsuariosClienteAppTransaction : EstadoRegistroProblema
    {
        internal override Tuple<bool, string, string> Ejecutar(ServicioProblema servicio)
        {
            servicio.SesionesUsuarios = servicio.RepositorioUserSession.ObtenerSesionUsuarioConDispositivo(new FiltroSesionUsuarioAbiertas(servicio.IdZona)).GroupBy(us => us.DEVICE_ID).Select(us => us.First()).ToList();
            return servicio.SetearEstado(new EstadoRegistrarProblemaClienteAppTransaction());
        }
    }

    internal class EstadoRegistrarProblemaClienteAppTransaction : EstadoRegistroProblema
    {
        internal override Tuple<bool, string, string> Ejecutar(ServicioProblema servicio)
        {
            if (servicio.SesionesUsuarios.Count == 0)
                return RegistrarSoloProblema(servicio);
            return RegistrarProblemaConSesionUsuario(servicio);
        }

        private Tuple<bool, string, string> RegistrarSoloProblema(ServicioProblema servicio)
        {
            if (servicio.IdError == 0)
                servicio.RepositorioTroubleTicket.Agregar(new CLIENT_APP_TRANSACTION_TROUBLE_TICKET
                {
                    CREATION_DATE = DateTime.Now,
                    ERROR = new ERROR { THROW_ON = DateTime.Now, MESSAGE = servicio.MensajeError.Substring(0, servicio.MensajeError.Length > 255 ? 254 : servicio.MensajeError.Length - 1), DETAILS = servicio.MensajeError, APPLICATION_ID = servicio.IdAplicacion, TYPE_ERROR_ID = 1 },
                    ZONE_ID = servicio.IdZona

                });
            else
                servicio.RepositorioTroubleTicket.CrearTicketAppCliente(new CLIENT_APP_TRANSACTION_TROUBLE_TICKET
                {
                    CREATION_DATE = DateTime.Now,
                    ZONE_ID = servicio.IdZona
                }, servicio.IdError);
            return new Tuple<bool, string, string>(false, "", "");
        }

        private Tuple<bool, string, string> RegistrarProblemaConSesionUsuario(ServicioProblema servicio)
        {
            var sesionUsuario = servicio.ObtenerSesionUsuarioAlgoritmo();
            if (servicio.IdError == 0)
                servicio.RepositorioTroubleTicket.Agregar(new CLIENT_APP_TRANSACTION_TROUBLE_TICKET
                {
                    ASSIGNMENT_DATE = DateTime.Now,
                    CREATION_DATE = DateTime.Now,
                    ERROR = new ERROR { THROW_ON = DateTime.Now, MESSAGE = servicio.MensajeError.Substring(0, servicio.MensajeError.Length > 255 ? 254 : servicio.MensajeError.Length - 1), DETAILS = servicio.MensajeError, APPLICATION_ID = servicio.IdAplicacion, TYPE_ERROR_ID = 1 },
                    USER_SESSION_ID = sesionUsuario.ID,
                    ZONE_ID = servicio.IdZona
                });
            else
                servicio.RepositorioTroubleTicket.CrearTicketAppCliente(new CLIENT_APP_TRANSACTION_TROUBLE_TICKET
                {
                    ASSIGNMENT_DATE = DateTime.Now,
                    CREATION_DATE = DateTime.Now,
                    USER_SESSION_ID = sesionUsuario.ID,
                    ZONE_ID = servicio.IdZona
                }, servicio.IdError);
            return new Tuple<bool, string, string>(true, servicio.IdError == 0 ? servicio.MensajeError : servicio.Error.MESSAGE, sesionUsuario.DEVICE.IP);
        }
    }

    internal class EstadoObtenerSesionesUsuariosParaMobileTicket : EstadoRegistroProblema
    {
        internal override Tuple<bool, string, string> Ejecutar(ServicioProblema servicio)
        {
            servicio.SesionesUsuarios = servicio.RepositorioUserSession.ObtenerSesionUsuarioConDispositivo(new FiltroSesionUsuarioAbiertas(servicio.IdZona)).GroupBy(us => us.DEVICE_ID).Select(us => us.FirstOrDefault()).ToList();
            return servicio.SetearEstado(new EstadoRegistrarProblemaMobile());
        }
    }

    internal class EstadoRegistrarProblemaMobile : EstadoRegistroProblema
    {
        internal override Tuple<bool, string, string> Ejecutar(ServicioProblema servicio)
        {
            if (servicio.SesionesUsuarios.Count == 0)
                return RegistrarSoloProblema(servicio);
            return RegistrarProblemaConSesionUsuario(servicio);
        }

        private Tuple<bool, string, string> RegistrarSoloProblema(ServicioProblema servicio)
        {
            if (servicio.MensajeError == null)
                servicio.RepositorioTroubleTicket.CrearMobileProceso(new MOBILE_TROUBLE_TICKET
                {
                    CREATION_DATE = DateTime.Now,
                    MESSAGE = "Error en Proceso N4",
                    ZONE_ID = servicio.IdZona,
                }, servicio.IdTosProceso);
            else
                servicio.RepositorioTroubleTicket.Agregar(new MOBILE_TROUBLE_TICKET
                {
                    CREATION_DATE = DateTime.Now,
                    MESSAGE = servicio.MensajeError,
                    ZONE_ID = servicio.IdZona,
                    PRE_GATE_ID = servicio.IdPreGate
                });
            return new Tuple<bool, string, string>(false, "", "");
        }

        private Tuple<bool, string, string> RegistrarProblemaConSesionUsuario(ServicioProblema servicio)
        {
            var sesionUsuario = servicio.ObtenerSesionUsuarioAlgoritmo();
            if (servicio.MensajeError == null)
                servicio.RepositorioTroubleTicket.CrearMobileProceso(new MOBILE_TROUBLE_TICKET
                {
                    ASSIGNMENT_DATE = DateTime.Now,
                    CREATION_DATE = DateTime.Now,
                    USER_SESSION_ID = sesionUsuario.ID,
                    MESSAGE = "Error en Proceso N4",
                    ZONE_ID = servicio.IdZona,
                }, servicio.IdTosProceso);
            else
                servicio.RepositorioTroubleTicket.Agregar(new MOBILE_TROUBLE_TICKET
                {
                    ASSIGNMENT_DATE = DateTime.Now,
                    CREATION_DATE = DateTime.Now,
                    USER_SESSION_ID = sesionUsuario.ID,
                    MESSAGE = servicio.MensajeError,
                    ZONE_ID = servicio.IdZona,
                    PRE_GATE_ID = servicio.IdPreGate
                });
            return new Tuple<bool, string, string>(true, servicio.MensajeError ?? "Error en Proceso N4", sesionUsuario.DEVICE.IP);
        }
    }
}
