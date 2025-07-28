using RECEPTIO.CapaAplicacion.TransactionDepot.Aplicacion.Contratos;
using RECEPTIO.CapaAplicacion.TransactionDepot.Aplicacion.Interfaces;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaDominio.Nucleo.ServiciosDominio.N4;
using RECEPTIO.CapaDominio.Transaction.ServiciosDominio;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;


namespace RECEPTIO.CapaDominio.TransactionDepot.ServicioDominio
{
    public class ServicioGeneral : IGeneralDepot
    {
        internal readonly IRepositorioPreGate RepositorioPreGate;
        internal readonly IRepositorioTransaccionQuiosco RepositorioTransaccionQuiosco;
        internal readonly IRepositorioValidaAduana RepositorioValidaAduana;
        internal readonly IConector Conector;
        internal readonly IRepositorioN4 RepositorioN4;
        internal readonly IRepositorioMensaje RepositorioMensaje;
        internal string Token;
        internal long IdTransaccion;
        internal long? Id;
        internal long? Turno;
        internal string Cedula;
        internal string Placa;
        internal string Contenedor;
        internal string Booking;
        internal string DatosBooking;
        internal string Usuario;
        internal long Clave;
        internal long Gkey;
        private EstadoKioscoZAL _estado;
        internal PRE_GATE PreGate;
        internal List<PROCESS> Procesos;
        internal TOS_PROCCESS TosProcess;
        internal string XmlRecepcion;

        public ServicioGeneral(IRepositorioPreGate repositorioPreGate, IRepositorioTransaccionQuiosco repositorioTransaccionQuiosco, IRepositorioValidaAduana repositorioValidaAduana, IConector conector, IRepositorioN4 repositorioN4, IRepositorioMensaje repositorioMensaje)
        {
            RepositorioPreGate = repositorioPreGate;
            RepositorioTransaccionQuiosco = repositorioTransaccionQuiosco;
            RepositorioValidaAduana = repositorioValidaAduana;
            Conector = conector;
            RepositorioN4 = repositorioN4;
            RepositorioMensaje = repositorioMensaje;
        }

        public RespuestaN4Depot AutentificacionWS(string usuario, long clave)
        {
            if (usuario == string.Empty || clave == 0)
            {
                return new RespuestaN4Depot { IdTransaccion = 0, Mensaje = "Faltan datos por ingresar", FueOk = false, MensajeDetalle = "Faltan datos por ingresar" };
            }

            string v_Strvalor = $@"<PARAMETROS OPERATION_NAME=""ServicioTransactionDepot""
                                                ACTION_NAME=""Valida Credenciales""
                                                METHOD_NAME=""AutentificacionWS""
                                                LOG_SECUENCE_ID=""0""
                                                OPERATION_NOTES=""{"Usuario: " + usuario + " Password:" + clave}""
                                                RESULTA=""1""
                                                RESULT_MESSAGE=""solicitud de token""/>";

            var v_resultado = RepositorioPreGate.ObtenerValidacionesGenerales("REGISTRA_LOG", v_Strvalor, 0, 0);

            Usuario = usuario;
            Clave= clave;
            Procesos = new List<PROCESS>();
            _estado = new EstadoAutentificar();
            return _estado.Ejecutar(this);
        }

        public RespuestaN4Depot GeneraVisita(long idTransaccion, string token, string cedula, string placa, long turno)
        {
            if ( idTransaccion == 0  || token == string.Empty || cedula == string.Empty || placa == string.Empty || turno == 0 )
            {
                return new RespuestaN4Depot { IdTransaccion = 0, Mensaje = "Faltan datos por ingresar", FueOk = false, MensajeDetalle = "Faltan datos por ingresar" };
            }


            string v_Strvalor = $@"<PARAMETROS OPERATION_NAME=""ServicioTransactionDepot""
                                                ACTION_NAME=""Genera Visita N4""
                                                METHOD_NAME=""GeneraVisita""
                                                LOG_SECUENCE_ID=""{idTransaccion}""
                                                OPERATION_NOTES=""{"Transaccion: " + idTransaccion + " Token:" + token + " Cedula:" + cedula + " Placa:" + placa + " Turno:" + turno }""
                                                RESULTA=""1""
                                                RESULT_MESSAGE=""{placa}""/>";

            var v_resultado = RepositorioPreGate.ObtenerValidacionesGenerales("REGISTRA_LOG", v_Strvalor, 0, 0);

            IdTransaccion = idTransaccion;
            Token = token;
            Cedula = cedula;
            Placa = placa;
            Turno = turno;
            Procesos = new List<PROCESS>();
            _estado = new EstadoValidaToken(1);
            TosProcess = new TOS_PROCCESS();
            return _estado.Ejecutar(this);
        }

        public RespuestaN4Depot Procesar(long idTransaccion,string token, long preGate , string placa,  string contenedor, string booking)
        {
            if (idTransaccion == 0 || token == string.Empty || preGate == 0 || placa == string.Empty || contenedor== string.Empty || booking == string.Empty)
            {
                return new RespuestaN4Depot { IdTransaccion = 0, Mensaje = "Faltan datos por ingresar", FueOk = false, MensajeDetalle = "Faltan datos por ingresar" };
            }

            string v_Strvalor = $@"<PARAMETROS OPERATION_NAME=""ServicioTransactionDepot""
                                                ACTION_NAME=""Asigna Contenedor""
                                                METHOD_NAME=""Procesar""
                                                LOG_SECUENCE_ID=""{idTransaccion}""
                                                OPERATION_NOTES=""{"Pregate: " + preGate + " Transaccion: " + idTransaccion + " Token:" + token + " Contenedor:" + contenedor + " Placa:" + placa + " Booking:" + booking }""
                                                RESULTA=""1""
                                                RESULT_MESSAGE=""{placa}""/>";

            var v_resultado = RepositorioPreGate.ObtenerValidacionesGenerales("REGISTRA_LOG", v_Strvalor, 0, 0);

            

            //VALIDACION DE CONTENEDOR - TRUE SI ESTA EN LA TERMINAL - FALSE SI YA SE FUE
            var v_validaCamion = RepositorioPreGate.ObtenerValidacionesGenerales("VALIDA_CONTENEDOR_CISE", contenedor, 0, 0);

            if (v_validaCamion != string.Empty)
            {
                return new RespuestaN4Depot { IdTransaccion = 0, Mensaje = v_validaCamion, FueOk = false, MensajeDetalle = "Contenedor no se puede usar por que ya salió " };
            }

            //ACTUALIZA 
            RepositorioPreGate.ObtenerValidacionesGenerales("UPDATE_PREGATE_DATAIL", contenedor, 0,preGate);
            


            IdTransaccion = idTransaccion;
            Token = token;
            Id = preGate;
            Placa = placa;
            Contenedor = contenedor;
            Booking = booking;
            Procesos = new List<PROCESS>();
            _estado = new EstadoValidaToken(2); 
            TosProcess = new TOS_PROCCESS();
            return _estado.Ejecutar(this);
        }

        public RespuestaN4Depot GeneraEventoFacturaN4(long idTransaccion,string token,  string contenedor)
        {
            if (idTransaccion == 0 || token == string.Empty || contenedor == string.Empty )
            {
                return new RespuestaN4Depot { IdTransaccion = 0, Mensaje = "Faltan datos por ingresar", FueOk = false, MensajeDetalle = "Faltan datos por ingresar" };
            }

            string v_Strvalor = $@"<PARAMETROS OPERATION_NAME=""ServicioTransactionDepot""
                                                ACTION_NAME=""Carga Evento N4""
                                                METHOD_NAME=""EventoFactura""
                                                LOG_SECUENCE_ID=""{idTransaccion}""
                                                OPERATION_NOTES=""{"Transaccion: " + idTransaccion + " Token:" + token + " Contenedor:" + contenedor }""
                                                RESULTA=""1""
                                                RESULT_MESSAGE=""{contenedor}""/>";

            var v_resultado = RepositorioPreGate.ObtenerValidacionesGenerales("REGISTRA_LOG", v_Strvalor, 0, 0);

            IdTransaccion = idTransaccion;
            Token = token;
            Contenedor = contenedor;
            Procesos = new List<PROCESS>();
            _estado = new EstadoValidaToken(3);
            TosProcess = new TOS_PROCCESS();
            return _estado.Ejecutar(this);
        }

        public RespuestaN4Depot GeneraSalida(long idTransaccion, long _preGate, string token)
        {
            if (_preGate == 0 || token == string.Empty)
            {
                return new RespuestaN4Depot { IdTransaccion = 0, Mensaje = "Faltan datos por ingresar", FueOk = false, MensajeDetalle = "Faltan datos por ingresar" };
            }

            string v_Strvalor = $@"<PARAMETROS OPERATION_NAME=""ServicioTransactionDepot""
                                                ACTION_NAME=""Genera Salida N4""
                                                METHOD_NAME=""GeneraSalida""
                                                LOG_SECUENCE_ID=""{idTransaccion}""
                                                OPERATION_NOTES=""{"Transaccion: " + _preGate + " Token:" + token }""
                                                RESULTA=""1""
                                                RESULT_MESSAGE=""{_preGate}""/>";

            var v_resultado = RepositorioPreGate.ObtenerValidacionesGenerales("REGISTRA_LOG", v_Strvalor, 0, 0);

            IdTransaccion = idTransaccion;
            PreGate = new PRE_GATE();
            PreGate.PRE_GATE_ID = _preGate;
            Token = token;
            Procesos = new List<PROCESS>();
            _estado = new EstadoValidaToken(4);
            TosProcess = new TOS_PROCCESS();
            return _estado.Ejecutar(this);
        }

        internal void ActualizarEntrada(long idPreGate, string status)
        {
            var preGate = RepositorioPreGate.ObtenerObjetos(new FiltroPreGatePorId(idPreGate)).FirstOrDefault();
            preGate.STATUS = status;
            RepositorioPreGate.Actualizar(preGate);
        }

        internal RespuestaN4Depot GrabarRegistroProcesoSalida(bool fueOk, int idMensaje, string respuesta, ServicioGeneral servicioGeneral)
        {
            var servicioTransaccionQuiosco = new ServicioTransaccionQuiosco(RepositorioTransaccionQuiosco, RepositorioPreGate);
            servicioTransaccionQuiosco.RegistrarProceso(new KIOSK_TRANSACTION
            {
                IS_OK = fueOk,
                TRANSACTION_ID = Convert.ToInt32(servicioGeneral.IdTransaccion),
                PRE_GATE_ID = servicioGeneral.PreGate.PRE_GATE_ID,
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
            string mensajeTD = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(idMensaje)).FirstOrDefault().TROUBLE_DESK_MESSAGE;
            string mensajeDet = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(idMensaje)).FirstOrDefault().DETAILS;
            return new RespuestaN4Depot { FueOk = fueOk, Mensaje = mensajeTD, MensajeDetalle = mensajeDet, IdTransaccion = servicioGeneral.IdTransaccion, Token = servicioGeneral.Token, IdPreGateRecepcion = servicioGeneral.PreGate.PRE_GATE_ID };
        }

        internal RespuestaN4Depot SetearEstado(EstadoKioscoZAL estado)
        {
            _estado = estado;
            return _estado.Ejecutar(this);
        }

        public void LiberarRecursos()
        {
            RepositorioPreGate.LiberarRecursos();
            RepositorioTransaccionQuiosco.LiberarRecursos();
            RepositorioMensaje.LiberarRecursos();
        }
    }

    internal abstract class EstadoKioscoZAL
    {
        internal abstract RespuestaN4Depot Ejecutar(ServicioGeneral servicioGeneral);
    }

    internal class EstadoAutentificar : EstadoKioscoZAL
    {
        internal override RespuestaN4Depot Ejecutar(ServicioGeneral servicioGeneral)
        {            
            var resultado = servicioGeneral.RepositorioPreGate.ObtenerValidacionesGenerales("AUTENTIFICAR", servicioGeneral.Usuario, 0, servicioGeneral.Clave);
            if (resultado == string.Empty)
            {
                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeErrorAutentificacion),
                    STEP = "AUTENTIFICAR",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeErrorAutentificacion))).FirstOrDefault().TROUBLE_DESK_MESSAGE
                });

                servicioGeneral.RepositorioTransaccionQuiosco.Agregar(new KIOSK_TRANSACTION { PRE_GATE = null, PRE_GATE_ID = null, END_DATE = DateTime.Now, KIOSK_ID = 2, PROCESSES = servicioGeneral.Procesos, START_DATE = DateTime.Now });
                return new RespuestaN4Depot {IdTransaccion = servicioGeneral.Procesos.FirstOrDefault().TRANSACTION_ID, Mensaje = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeErrorAutentificacion))).FirstOrDefault().TROUBLE_DESK_MESSAGE, MensajeDetalle = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeErrorAutentificacion))).FirstOrDefault().DETAILS, FueOk = false , Token = string.Empty};                
            }
            else
            {
                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeAutentificacionOk),
                    STEP = "AUTENTIFICAR",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = resultado,
                    IS_OK = true
                });
                
                servicioGeneral.RepositorioTransaccionQuiosco.Agregar(new KIOSK_TRANSACTION { PRE_GATE = null, PRE_GATE_ID = null, END_DATE = DateTime.Now, KIOSK_ID = 2, PROCESSES = servicioGeneral.Procesos, START_DATE = DateTime.Now, IS_OK = true  });
                return new RespuestaN4Depot { IdTransaccion = servicioGeneral.Procesos.FirstOrDefault().TRANSACTION_ID, Mensaje = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeAutentificacionOk))).FirstOrDefault().TROUBLE_DESK_MESSAGE, MensajeDetalle = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeAutentificacionOk))).FirstOrDefault().DETAILS, FueOk = true, Token = resultado };
            }
        }
    }

    internal class EstadoValidaToken : EstadoKioscoZAL
    {
        private int tipoTransaccion;
       
        public EstadoValidaToken(int _tipoTransaccion)
        {
            tipoTransaccion = _tipoTransaccion;
        }

        internal override RespuestaN4Depot Ejecutar(ServicioGeneral servicioGeneral)
        {
            var resultado = servicioGeneral.RepositorioPreGate.ObtenerValidacionesGenerales("VALIDA_TOKEN", servicioGeneral.Token, 0, servicioGeneral.IdTransaccion);
            if (resultado != string.Empty)
            {
                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeTokenInválido),
                    STEP = "TOKEN",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = servicioGeneral.Token + " - " + resultado,
                });

                var v_trans = servicioGeneral.RepositorioTransaccionQuiosco.ObtenerObjetos(new FiltroTransaccionPorId(servicioGeneral.IdTransaccion)).FirstOrDefault();
                servicioGeneral.Procesos.AddRange(v_trans.PROCESSES);
                v_trans.PROCESSES = servicioGeneral.Procesos;
                servicioGeneral.RepositorioTransaccionQuiosco.Actualizar(v_trans);
                return new RespuestaN4Depot { IdTransaccion = servicioGeneral.IdTransaccion, Mensaje = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeTokenInválido))).FirstOrDefault().TROUBLE_DESK_MESSAGE, MensajeDetalle = resultado + " - " + servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeTokenInválido))).FirstOrDefault().DETAILS, FueOk = false, Token = servicioGeneral.Token  };
            }
            else
            {
                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeTokenOk),
                    STEP = "TOKEN",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = servicioGeneral.Token,
                    IS_OK = true
                });
                var v_trans = servicioGeneral.RepositorioTransaccionQuiosco.ObtenerObjetos(new FiltroTransaccionPorId(servicioGeneral.IdTransaccion)).FirstOrDefault();
                v_trans.PROCESSES = servicioGeneral.Procesos;
                servicioGeneral.RepositorioTransaccionQuiosco.Actualizar(v_trans);

                return servicioGeneral.SetearEstado(new EstadoConsultaTransaccionActivaN4(tipoTransaccion));
            }
        }
    }

    internal class EstadoConsultaTransaccionActivaN4 : EstadoKioscoZAL
    {
        private int tipoTrans;

        public  EstadoConsultaTransaccionActivaN4(int _tipoTrans)
        {
            tipoTrans = _tipoTrans;
        }

        internal override RespuestaN4Depot Ejecutar(ServicioGeneral servicioGeneral)
        {
            var ServicioAnunciante = new ServicioAnuncianteProblema.ServicioAnuncianteProblemaClient();
            servicioGeneral.Procesos = new List<PROCESS>();
            RespuestaN4Depot v_respuesta = null;

            if (tipoTrans == 1 || tipoTrans == 2 || tipoTrans == 4)
            {
                var resultado = servicioGeneral.RepositorioN4.TieneTransaccionActivaDepotPorPlaca(servicioGeneral.Placa);

                if (resultado){
                    var strPregate = servicioGeneral.RepositorioPreGate.ObtenerValidacionesGenerales("INFO_PRE_GATE_CISE",servicioGeneral.Placa, 0, servicioGeneral.IdTransaccion);
                    servicioGeneral.PreGate = servicioGeneral.RepositorioPreGate .ObtenerPreGateConDetalle (new FiltroPreGatePorId(long.Parse(strPregate))).FirstOrDefault();
                }
                
                if (resultado && tipoTrans == 1) //crea visita
                {
                    string strPreGate = servicioGeneral.PreGate?.PRE_GATE_ID.ToString();
                    if (string.IsNullOrEmpty(strPreGate) || strPreGate == "0")
                    {
                        servicioGeneral.Procesos.Add(new PROCESS
                        {
                            MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeVisitaActivaN4),
                            STEP = "COMPROBAR_VISITA",
                            STEP_DATE = DateTime.Now,
                            RESPONSE = servicioGeneral.Token + " - " + servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeVisitaActivaN4))).FirstOrDefault().TROUBLE_DESK_MESSAGE
                        });

                        var v_trans = servicioGeneral.RepositorioTransaccionQuiosco.ObtenerObjetos(new FiltroTransaccionPorId(servicioGeneral.IdTransaccion)).FirstOrDefault();
                        servicioGeneral.Procesos.AddRange(v_trans.PROCESSES);
                        v_trans.PROCESSES = servicioGeneral.Procesos;
                        servicioGeneral.RepositorioTransaccionQuiosco.Actualizar(v_trans);
                        ServicioAnunciante.AnunciarProblema(int.Parse(servicioGeneral.IdTransaccion.ToString()));
                        v_respuesta = new RespuestaN4Depot { IdTransaccion = servicioGeneral.IdTransaccion, Mensaje = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeVisitaActivaN4))).FirstOrDefault().TROUBLE_DESK_MESSAGE, MensajeDetalle = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeVisitaActivaN4))).FirstOrDefault().DETAILS };
                    }
                    else
                    {
                        servicioGeneral.Procesos.Add(new PROCESS
                        {
                            MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeN4Ok),
                            STEP = "COMPROBAR_VISITA",
                            STEP_DATE = DateTime.Now,
                            RESPONSE = servicioGeneral.PreGate?.PRE_GATE_ID.ToString(),
                            IS_OK = true
                        });

                        var v_trans = servicioGeneral.RepositorioTransaccionQuiosco.ObtenerObjetos(new FiltroTransaccionPorId(servicioGeneral.IdTransaccion)).FirstOrDefault();
                        servicioGeneral.Procesos.AddRange(v_trans.PROCESSES);
                        v_trans.PROCESSES = servicioGeneral.Procesos;
                        servicioGeneral.RepositorioTransaccionQuiosco.Actualizar(v_trans);

                        return new RespuestaN4Depot { Token = servicioGeneral.Token, IdTransaccion = servicioGeneral.IdTransaccion, Mensaje = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeN4Ok))).FirstOrDefault().TROUBLE_DESK_MESSAGE, MensajeDetalle = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeN4Ok))).FirstOrDefault().DETAILS, FueOk = true, IdPreGateRecepcion = servicioGeneral.PreGate.PRE_GATE_ID };
                    }
                }
                else
                {
                    if (!resultado && tipoTrans == 1)
                    {
                        servicioGeneral.Procesos.Add(new PROCESS
                        {
                            MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeN4Ok),
                            STEP = "COMPROBAR_VISITA",
                            STEP_DATE = DateTime.Now,
                            RESPONSE = servicioGeneral.Token,
                            IS_OK = true
                        });
                        return servicioGeneral.SetearEstado(new EstadoValidaTurno());
                    } 
                }

                if (resultado && tipoTrans == 2) // Gate Transaction - Procesar
                {
                    servicioGeneral.Procesos.Add(new PROCESS
                    {
                        MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeN4Ok),
                        STEP = "COMPROBAR_VISITA",
                        STEP_DATE = DateTime.Now,
                        RESPONSE = servicioGeneral.Token,
                        IS_OK = true
                    });
                    return servicioGeneral.SetearEstado(new EstadoTransaccionActivaPregate());
                }
                else
                {
                    if (!resultado && tipoTrans == 2)
                    {
                        servicioGeneral.Procesos.Add(new PROCESS
                        {
                            MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeNoExisteEntradaQuiosco),
                            STEP = "COMPROBAR_VISITA",
                            STEP_DATE = DateTime.Now,
                            RESPONSE = servicioGeneral.Token + " - NO EXISTE VISITA"
                        });
                        var v_trans = servicioGeneral.RepositorioTransaccionQuiosco.ObtenerObjetos(new FiltroTransaccionPorId(servicioGeneral.IdTransaccion)).FirstOrDefault();
                        servicioGeneral.Procesos.AddRange(v_trans.PROCESSES);
                        v_trans.PROCESSES = servicioGeneral.Procesos;
                        servicioGeneral.RepositorioTransaccionQuiosco.Actualizar(v_trans);
                        ServicioAnunciante.AnunciarProblema(int.Parse(servicioGeneral.IdTransaccion.ToString()));
                        v_respuesta = new RespuestaN4Depot { IdTransaccion = servicioGeneral.IdTransaccion, Mensaje = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeNoExisteEntradaQuiosco))).FirstOrDefault().TROUBLE_DESK_MESSAGE, MensajeDetalle = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeNoExisteEntradaQuiosco))).FirstOrDefault().DETAILS };
                    }
                }

                if (tipoTrans == 4)
                {
                    string strPreGate = servicioGeneral.PreGate?.PRE_GATE_ID.ToString();
                    servicioGeneral.PreGate = servicioGeneral.RepositorioPreGate.ObtenerPreGateConDetalle(new FiltroPreGatePorId(long.Parse(strPreGate))).FirstOrDefault();
                    if (!(servicioGeneral.PreGate is null)) { resultado = true; }
                    if (resultado) // Gate Transaction - SALIDA
                    {
                        if ((string.IsNullOrEmpty(strPreGate) || strPreGate == "0") || servicioGeneral.PreGate is null)
                        {
                            servicioGeneral.Procesos.Add(new PROCESS
                            {
                                MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeNoExisteEntradaQuiosco),
                                STEP = "COMPROBAR_VISITA",
                                STEP_DATE = DateTime.Now,
                                RESPONSE = servicioGeneral.Token + " - NO EXISTE VISITA PREGATE " + strPreGate
                            });

                            var v_trans = servicioGeneral.RepositorioTransaccionQuiosco.ObtenerObjetos(new FiltroTransaccionPorId(servicioGeneral.IdTransaccion)).FirstOrDefault();
                            servicioGeneral.Procesos.AddRange(v_trans.PROCESSES);
                            v_trans.PROCESSES = servicioGeneral.Procesos;
                            servicioGeneral.RepositorioTransaccionQuiosco.Actualizar(v_trans);
                            ServicioAnunciante.AnunciarProblema(int.Parse(servicioGeneral.IdTransaccion.ToString()));
                            v_respuesta = new RespuestaN4Depot {Token = servicioGeneral.Token, IdTransaccion = servicioGeneral.IdTransaccion, Mensaje = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeNoExisteEntradaQuiosco))).FirstOrDefault().TROUBLE_DESK_MESSAGE, MensajeDetalle = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeNoExisteEntradaQuiosco))).FirstOrDefault().DETAILS };
                        }
                        else
                        {
                            servicioGeneral.Procesos.Add(new PROCESS
                            {
                                MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeN4Ok),
                                STEP = "COMPROBAR_VISITA",
                                STEP_DATE = DateTime.Now,
                                RESPONSE = servicioGeneral.PreGate?.PRE_GATE_ID.ToString(),
                                IS_OK = true
                            });
                            var v_trans = servicioGeneral.RepositorioTransaccionQuiosco.ObtenerObjetos(new FiltroTransaccionPorId(servicioGeneral.IdTransaccion)).FirstOrDefault();
                            servicioGeneral.Procesos.AddRange(v_trans.PROCESSES);
                            v_trans.PROCESSES = servicioGeneral.Procesos;
                            servicioGeneral.RepositorioTransaccionQuiosco.Actualizar(v_trans);
                            return servicioGeneral.SetearEstado(new EstadoSalidaProcessTruckDeliveryEmpty());
                        }
                    }
                    else
                    {
                        if (!resultado)
                        {
                            servicioGeneral.Procesos.Add(new PROCESS
                            {
                                MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeNoExisteEntradaQuiosco),
                                STEP = "COMPROBAR_VISITA",
                                STEP_DATE = DateTime.Now,
                                RESPONSE = servicioGeneral.Token + " - NO EXISTE VISITA"
                            });
                            var v_trans = servicioGeneral.RepositorioTransaccionQuiosco.ObtenerObjetos(new FiltroTransaccionPorId(servicioGeneral.IdTransaccion)).FirstOrDefault();
                            servicioGeneral.Procesos.AddRange(v_trans.PROCESSES);
                            v_trans.PROCESSES = servicioGeneral.Procesos;
                            servicioGeneral.RepositorioTransaccionQuiosco.Actualizar(v_trans);
                            ServicioAnunciante.AnunciarProblema(int.Parse(servicioGeneral.IdTransaccion.ToString()));
                            v_respuesta = new RespuestaN4Depot { IdTransaccion = servicioGeneral.IdTransaccion, Mensaje = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeNoExisteEntradaQuiosco))).FirstOrDefault().TROUBLE_DESK_MESSAGE, MensajeDetalle = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeNoExisteEntradaQuiosco))).FirstOrDefault().DETAILS };
                        }
                    }
                }
            }

            if (tipoTrans == 3) // Carga Evento a Facturar N4
            {
                if (servicioGeneral.Contenedor != string.Empty)
                {
                    v_respuesta = servicioGeneral.SetearEstado(new EstadoEventInvoiceN4());
                }
                else
                {
                    servicioGeneral.Procesos.Add(new PROCESS
                    {
                        MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeVisitaActivaN4),
                        STEP = "PROCESO N4",
                        STEP_DATE = DateTime.Now,
                        RESPONSE = ""
                    });

                    var v_trans = servicioGeneral.RepositorioTransaccionQuiosco.ObtenerObjetos(new FiltroTransaccionPorId(servicioGeneral.IdTransaccion)).FirstOrDefault();
                    servicioGeneral.Procesos.AddRange(v_trans.PROCESSES);
                    v_trans.PROCESSES = servicioGeneral.Procesos;
                    servicioGeneral.RepositorioTransaccionQuiosco.Actualizar(v_trans);
                    ServicioAnunciante.AnunciarProblema(int.Parse(servicioGeneral.IdTransaccion.ToString()));
                    v_respuesta = new RespuestaN4Depot { IdTransaccion = servicioGeneral.IdTransaccion, Mensaje = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeNoExistePregate))).FirstOrDefault().TROUBLE_DESK_MESSAGE, MensajeDetalle = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeNoExistePregate))).FirstOrDefault().DETAILS };
                }
            }
            return v_respuesta;
        }
    }

    internal class EstadoTransaccionActivaPregate : EstadoKioscoZAL
    {
        RespuestaN4Depot v_respuesta = null;
        internal override RespuestaN4Depot Ejecutar(ServicioGeneral servicioGeneral)
        {
            var ServicioAnunciante = new ServicioAnuncianteProblema.ServicioAnuncianteProblemaClient();
            if (servicioGeneral.Id > 0)
            {
                servicioGeneral.PreGate = servicioGeneral.RepositorioPreGate.ObtenerPreGateConDetalle(new FiltroPreGatePorId(servicioGeneral.Id.Value)).FirstOrDefault();
            }

            if (servicioGeneral.PreGate == null)
            {
                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeNoExistePregate),
                    STEP = "PREGATE",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = "PREGATE NO EXISTE"
                });
                var v_trans = servicioGeneral.RepositorioTransaccionQuiosco.ObtenerObjetos(new FiltroTransaccionPorId(servicioGeneral.IdTransaccion)).FirstOrDefault();
                servicioGeneral.Procesos.AddRange(v_trans.PROCESSES);
                v_trans.PROCESSES = servicioGeneral.Procesos;
                servicioGeneral.RepositorioTransaccionQuiosco.Actualizar(v_trans);
                ServicioAnunciante.AnunciarProblema(int.Parse(servicioGeneral.IdTransaccion.ToString()));
                v_respuesta = new RespuestaN4Depot { IdTransaccion = servicioGeneral.IdTransaccion, Mensaje = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeNoExistePregate))).FirstOrDefault().TROUBLE_DESK_MESSAGE, MensajeDetalle = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeNoExistePregate))).FirstOrDefault().DETAILS };
            }
            else
            {
                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajePreGateOk),
                    STEP = "PREGATE",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = servicioGeneral.Token,
                    IS_OK = true                    
                });
                v_respuesta = servicioGeneral.SetearEstado(new EstadoConsultaInfoBooking());
            }
            return v_respuesta;
        }
    }

    internal class EstadoConsultaInfoBooking : EstadoKioscoZAL
    {
        RespuestaN4Depot v_respuesta = null;
        internal override RespuestaN4Depot Ejecutar(ServicioGeneral servicioGeneral)
        {
            var ServicioAnunciante = new ServicioAnuncianteProblema.ServicioAnuncianteProblemaClient();
            servicioGeneral.DatosBooking = servicioGeneral.RepositorioPreGate.ObtenerValidacionesGenerales("INFO_BOOKING", servicioGeneral.Booking, 0, servicioGeneral.IdTransaccion);
            if (servicioGeneral.DatosBooking ==null  || servicioGeneral.DatosBooking == string.Empty)
            {
                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeNoExisteCodigo),
                    STEP = "BOOKING",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = servicioGeneral.Booking + " - BOOKING NO EXISTE"
                });

                var v_trans = servicioGeneral.RepositorioTransaccionQuiosco.ObtenerObjetos(new FiltroTransaccionPorId(servicioGeneral.IdTransaccion)).FirstOrDefault();
                servicioGeneral.Procesos.AddRange(v_trans.PROCESSES);
                v_trans.PROCESSES = servicioGeneral.Procesos;
                servicioGeneral.RepositorioTransaccionQuiosco.Actualizar(v_trans);
                ServicioAnunciante.AnunciarProblema(int.Parse(servicioGeneral.IdTransaccion.ToString()));
                v_respuesta = new RespuestaN4Depot { IdTransaccion = servicioGeneral.IdTransaccion, Mensaje = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeNoExisteCodigo))).FirstOrDefault().TROUBLE_DESK_MESSAGE, MensajeDetalle = servicioGeneral.DatosBooking + " - " + servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeNoExisteCodigo))).FirstOrDefault().DETAILS, FueOk = false, Token = string.Empty };
            }
            else
            {
                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeTokenOk),
                    STEP = "BOOKING",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = servicioGeneral.Booking + " - " + servicioGeneral.DatosBooking,
                    IS_OK = true
                });
               
                v_respuesta = servicioGeneral.SetearEstado(new EstadoTransaction());
            }
            return v_respuesta;
        }
    }

    internal class EstadoValidaTurno : EstadoKioscoZAL
    {
        RespuestaN4Depot v_respuesta = null;
        internal override RespuestaN4Depot Ejecutar(ServicioGeneral servicioGeneral)
        {
            var ServicioAnunciante = new ServicioAnuncianteProblema.ServicioAnuncianteProblemaClient();

            string v_Strvalor = $@"<PARAMETROS TURNO=""{servicioGeneral.Turno}""
                                                CEDULA=""{servicioGeneral.Cedula}""
                                                PLACA=""{servicioGeneral.Placa}"">
                                            </PARAMETROS>";

            var v_resultado = servicioGeneral.RepositorioPreGate.ObtenerValidacionesGenerales("VALIDA_TURNO", v_Strvalor, 0, servicioGeneral.IdTransaccion);
            if (v_resultado != string.Empty)
                {
                    servicioGeneral.Procesos.Add(new PROCESS
                    {
                        MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeTurnoInvalido),
                        STEP = "COMPROBAR_TURNO",
                        STEP_DATE = DateTime.Now,
                        RESPONSE = servicioGeneral.Token + " - Turno:" + servicioGeneral.Turno + " - " + v_resultado,
                    });

                    var v_trans = servicioGeneral.RepositorioTransaccionQuiosco.ObtenerObjetos(new FiltroTransaccionPorId(servicioGeneral.IdTransaccion)).FirstOrDefault();
                    servicioGeneral.Procesos.AddRange(v_trans.PROCESSES);
                    v_trans.PROCESSES = servicioGeneral.Procesos;
                    servicioGeneral.RepositorioTransaccionQuiosco.Actualizar(v_trans);
                    ServicioAnunciante.AnunciarProblema(int.Parse(servicioGeneral.IdTransaccion.ToString()));
                    v_respuesta = new RespuestaN4Depot { IdTransaccion = servicioGeneral.IdTransaccion, Mensaje = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeTurnoInvalido))).FirstOrDefault().TROUBLE_DESK_MESSAGE, MensajeDetalle = v_resultado, FueOk = false, Token = servicioGeneral.Token };
                }
                else
                {
                    servicioGeneral.Procesos.Add(new PROCESS
                    {
                        MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeTurnoOk),
                        STEP = "COMPROBAR_TURNO",
                        STEP_DATE = DateTime.Now,
                        RESPONSE = servicioGeneral.Token + " - Turno:" + servicioGeneral.Turno,
                        IS_OK = true
                    });
                    v_respuesta = servicioGeneral.SetearEstado(new EstadoGrabarPreGateZAL());
                }
            return v_respuesta;
        }
    }

    internal class EstadoGrabarPreGateZAL : EstadoKioscoZAL
    {
        internal override RespuestaN4Depot Ejecutar(ServicioGeneral servicioGeneral)
        {
            var detalles = new List<PRE_GATE_DETAIL>();

            detalles.Add(new PRE_GATE_DETAIL
            {
                STATUS = "N",
                TRANSACTION_NUMBER = servicioGeneral.IdTransaccion.ToString() +"-"+ servicioGeneral.Turno.ToString(),
                TRANSACTION_TYPE_ID = 17,
                REFERENCE_ID = "I",
                IS_RECYCLED = true
            });

            servicioGeneral.PreGate = new PRE_GATE
            {
                PRE_GATE_ID = servicioGeneral.RepositorioPreGate.ObtenerSecuenciaIdPreGate(),
                CREATION_DATE = DateTime.Now,
                DEVICE_ID = 1,//tablet
                DRIVER_ID = servicioGeneral.Cedula,
                PRE_GATE_DETAILS = detalles,
                STATUS = "C",
                TRUCK_LICENCE = servicioGeneral.Placa,
                USER = "OPACIFIC",
                IS_RECYCLED = true
            };

            servicioGeneral.Procesos.Add(new PROCESS
            {
                MESSAGE_ID = Convert.ToInt32(Mensajes.MensajePreGateOk),
                STEP = "PREGATE",
                STEP_DATE = DateTime.Now,
                RESPONSE = servicioGeneral.Token,
                IS_OK = true
            });

            servicioGeneral.RepositorioPreGate.Agregar(servicioGeneral.PreGate);          
            return servicioGeneral.SetearEstado(new TruckVisitRecepcion());
        }
    }

    internal class TruckVisitRecepcion : EstadoKioscoZAL
    {
        internal override RespuestaN4Depot Ejecutar(ServicioGeneral servicioGeneral)
        {
            var ServicioAnunciante = new ServicioAnuncianteProblema.ServicioAnuncianteProblemaClient();
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
                var v_trans = servicioGeneral.RepositorioTransaccionQuiosco.ObtenerObjetos(new FiltroTransaccionPorId(servicioGeneral.IdTransaccion)).FirstOrDefault();
                servicioGeneral.Procesos.AddRange(v_trans.PROCESSES);
                v_trans.PROCESSES = servicioGeneral.Procesos;
                servicioGeneral.RepositorioTransaccionQuiosco.Actualizar(v_trans);
                ServicioAnunciante.AnunciarProblema(int.Parse(servicioGeneral.IdTransaccion.ToString()));
                return new RespuestaN4Depot { Mensaje = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeErrorN4))).FirstOrDefault().TROUBLE_DESK_MESSAGE, MensajeDetalle = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeErrorN4))).FirstOrDefault().DETAILS, IdTransaccion = servicioGeneral.IdTransaccion };
            }else
            {
                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeN4Ok),
                    STEP = "PROCESO N4",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = respuesta.ResultadosConsultas.FirstOrDefault().Result,
                    IS_OK = true
                });
                return servicioGeneral.SetearEstado(new EstadoStageDone1()); 
            }
        }

        private string ArmarXmlTruckVisitRecepcion(ServicioGeneral servicioGeneral)
        {
            var bat = servicioGeneral.Placa.Substring(1, servicioGeneral.Placa.Length - 1);
            var fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            return $@"<gate>
                        <create-truck-visit>
                        <gate-id>CISE</gate-id>
                        <stage-id>ingate_cise</stage-id>
                        <lane-id>KIOSCO 02</lane-id>
		                <truck license-nbr=""{servicioGeneral.PreGate.TRUCK_LICENCE.ToUpper()}""/>
		                <driver license-nbr=""{servicioGeneral.PreGate.DRIVER_ID}""/>
		                <truck-visit gos-tv-key=""{servicioGeneral.PreGate.PRE_GATE_ID * -1}"" bat-nbr=""{bat.ToUpper()}""/>
		                <timestamp>{fecha}</timestamp>
	                    </create-truck-visit>
                       </gate>";
        }
    }

    internal class EstadoStageDone1 : EstadoKioscoZAL
    {
        internal override RespuestaN4Depot Ejecutar(ServicioGeneral servicioGeneral)
        {
            var ServicioAnunciante = new ServicioAnuncianteProblema.ServicioAnuncianteProblemaClient();
            var xml = ArmarXml(servicioGeneral.PreGate);
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
                servicioGeneral.ActualizarEntrada(servicioGeneral.PreGate.PRE_GATE_ID, "N");
                servicioGeneral.XmlRecepcion = respuesta.ResultadosConsultas.FirstOrDefault().Result;

                var v_trans = servicioGeneral.RepositorioTransaccionQuiosco.ObtenerObjetos(new FiltroTransaccionPorId(servicioGeneral.IdTransaccion)).FirstOrDefault();
                v_trans.PRE_GATE_ID = servicioGeneral.PreGate.PRE_GATE_ID;
                servicioGeneral.Procesos.AddRange(v_trans.PROCESSES);
                v_trans.PROCESSES = servicioGeneral.Procesos;
                servicioGeneral.RepositorioTransaccionQuiosco.Actualizar(v_trans);
                return new RespuestaN4Depot {Token = servicioGeneral.Token, IdTransaccion = servicioGeneral.IdTransaccion , Mensaje = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeN4Ok))).FirstOrDefault().TROUBLE_DESK_MESSAGE, MensajeDetalle = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeN4Ok))).FirstOrDefault().DETAILS, FueOk = true, IdPreGateRecepcion = servicioGeneral.PreGate.PRE_GATE_ID };
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
                var v_trans = servicioGeneral.RepositorioTransaccionQuiosco.ObtenerObjetos(new FiltroTransaccionPorId(servicioGeneral.IdTransaccion)).FirstOrDefault();
                v_trans.PRE_GATE_ID = servicioGeneral.PreGate.PRE_GATE_ID;
                servicioGeneral.Procesos.AddRange(v_trans.PROCESSES);
                v_trans.PROCESSES = servicioGeneral.Procesos;
                servicioGeneral.RepositorioTransaccionQuiosco.Actualizar(v_trans);
                ServicioAnunciante.AnunciarProblema(int.Parse(servicioGeneral.IdTransaccion.ToString()));
                return new RespuestaN4Depot { Mensaje = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeErrorN4))).FirstOrDefault().TROUBLE_DESK_MESSAGE, MensajeDetalle = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeErrorN4))).FirstOrDefault().DETAILS, IdTransaccion = servicioGeneral.IdTransaccion };
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

    internal class EstadoTransaction : EstadoKioscoZAL
    {
        internal override RespuestaN4Depot Ejecutar(ServicioGeneral servicioGeneral)
        {
            servicioGeneral.Gkey = long.Parse(servicioGeneral.RepositorioPreGate.ObtenerValidacionesGenerales("VALIDA_CONTENEDOR", servicioGeneral.Contenedor, 0, servicioGeneral.IdTransaccion));
            var ServicioAnunciante = new ServicioAnuncianteProblema.ServicioAnuncianteProblemaClient();
            var xml = ArmarXmlGateTransaction(servicioGeneral);
            var respuesta = servicioGeneral.Conector.Invocacion(xml);

            if (!respuesta.ResultadosConsultas.Any(c => c.Result.Contains(@"status=""OK""")))
            {
                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeErrorN4),
                    STEP = "PROCESO N4",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = $"N4 RETORNO ERROR EN GATE SUBMIT TRANSACTION ZAL :{respuesta.ToString()}"
                });
                var v_trans = servicioGeneral.RepositorioTransaccionQuiosco.ObtenerObjetos(new FiltroTransaccionPorId(servicioGeneral.IdTransaccion)).FirstOrDefault();
                servicioGeneral.Procesos.AddRange(v_trans.PROCESSES);
                v_trans.PROCESSES = servicioGeneral.Procesos;
                servicioGeneral.RepositorioTransaccionQuiosco.Actualizar(v_trans);
                ServicioAnunciante.AnunciarProblema(int.Parse(servicioGeneral.IdTransaccion.ToString()));
                return new RespuestaN4Depot { Mensaje = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeErrorN4))).FirstOrDefault().TROUBLE_DESK_MESSAGE, MensajeDetalle = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeErrorN4))).FirstOrDefault().DETAILS, IdTransaccion = servicioGeneral.IdTransaccion};
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
                return servicioGeneral.SetearEstado(new EstadoStageDone2());
            }
        }

        private string ArmarXmlGateTransaction(ServicioGeneral servicioGeneral)
        {
            var bat = servicioGeneral.Placa.Substring(1, servicioGeneral.Placa.Length - 1);
            var fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

            string[] v_datosBoking = servicioGeneral.DatosBooking.Split(',');

            return $@"<gate>
                              <submit-transaction>
                                <gate-id>CISE</gate-id>
                                <stage-id>yardzal_cise</stage-id>
                                <truck-visit gos-tv-key=""{servicioGeneral.PreGate.PRE_GATE_ID * -1}""/>
                                <truck-transaction tran-type=""DM"" order-nbr=""{v_datosBoking[3]}"" notes="""">
                                <eq-order order-nbr=""{v_datosBoking[0]}"" order-type=""BOOK"" line-id=""{v_datosBoking[3]}"" freight-kind=""MTY"">
                                                                                         <eq-order-items>
                                                                                                       <eq-order-item type=""{v_datosBoking[4]}"" eq-length=""{v_datosBoking[6]}"" eq-height=""{v_datosBoking[5]}"" eq-iso-group=""{v_datosBoking[2]}""/>
                                                                                         </eq-order-items>
                                                                                         </eq-order>
                                                                          <container eqid=""{servicioGeneral.Contenedor}""/>
                                                           </truck-transaction>
                                </submit-transaction>
                        </gate>";
        }
    }

    internal class EstadoStageDone2 : EstadoKioscoZAL
    {
        internal override RespuestaN4Depot Ejecutar(ServicioGeneral servicioGeneral)
        {
            var ServicioAnunciante = new ServicioAnuncianteProblema.ServicioAnuncianteProblemaClient();
            var xml = ArmarXml(servicioGeneral.PreGate);
            var respuesta = servicioGeneral.Conector.Invocacion(xml);
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
            {
                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeN4Ok),
                    STEP = "STAGE_DONE2",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = respuesta.ResultadosConsultas.FirstOrDefault().Result,
                    IS_OK = true
                });
                return servicioGeneral.SetearEstado(new EstadoSmdt());
            }
            else
            {
                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeErrorN4),
                    STEP = "STAGE_DONE2",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = $"N4 RETORNO ERROR EN STAGE_DONE :{respuesta.ToString()}"
                });
                var v_trans = servicioGeneral.RepositorioTransaccionQuiosco.ObtenerObjetos(new FiltroTransaccionPorId(servicioGeneral.IdTransaccion)).FirstOrDefault();
                servicioGeneral.Procesos.AddRange(v_trans.PROCESSES);
                v_trans.PROCESSES = servicioGeneral.Procesos;
                servicioGeneral.RepositorioTransaccionQuiosco.Actualizar(v_trans);
                ServicioAnunciante.AnunciarProblema(int.Parse(servicioGeneral.IdTransaccion.ToString()));
                return new RespuestaN4Depot { Mensaje = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeErrorN4))).FirstOrDefault().TROUBLE_DESK_MESSAGE, MensajeDetalle = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeErrorN4))).FirstOrDefault().DETAILS, IdTransaccion = servicioGeneral.IdTransaccion };
            }
        }

        private string ArmarXml(PRE_GATE datos)
        {
            return $@"<gate>
                            <stage-done>
                                <gate-id>CISE</gate-id>
                                <stage-id>yardzal_cise</stage-id>
                                <truck-visit gos-tv-key=""{datos.PRE_GATE_ID * -1}""/>
                            </stage-done>
                     </gate >";
        }
    }

    internal class EstadoSmdt : EstadoKioscoZAL
    {
        internal override RespuestaN4Depot Ejecutar(ServicioGeneral servicioGeneral)
        {
            var ServicioAnunciante = new ServicioAnuncianteProblema.ServicioAnuncianteProblemaClient();
            bool v_error = false;

            var xml = ArmarXml("KIOSCO 02", servicioGeneral.Gkey);
            var servicioWeb = new ServicioAduana.n4ServiceSoapClient();
                //se cometa este codigo porque el deposito esta fuera del IP por ende no se genera SMDT
                /*for (var i = 0; i < 3; i++)
                {
                    var respuesta = servicioWeb.basicInvoke(ConfigurationManager.AppSettings["scopeCoordinateIds"], xml);
                    if (respuesta.Contains(@"Code>0</Code"))
                    {
                        servicioGeneral.Procesos.Add(new PROCESS
                        {
                            MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeSmdtOk),
                            STEP = "SMDT",
                            STEP_DATE = DateTime.Now,
                            RESPONSE = $"Contenedor:{servicioGeneral.Contenedor} {respuesta}",
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
                            RESPONSE = $"{xml}////Contenedor:{servicioGeneral.Contenedor} {respuesta}",
                        });
                        v_error = true;
                    }
                }*/

            if (servicioGeneral.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 17))
                foreach (var item in servicioGeneral.PreGate.PRE_GATE_DETAILS)
                {
                    item.STATUS = "O";
                    item.DOCUMENT_ID = servicioGeneral.Contenedor;
                }
                    
            using (var transaccion = new TransactionScope())
            {
                if (servicioGeneral.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 17))
                    servicioGeneral.RepositorioPreGate.Actualizar(servicioGeneral.PreGate);
                servicioGeneral.PreGate.STATUS = "P";
                var contador = 0;
                foreach (var item in servicioGeneral.PreGate.PRE_GATE_DETAILS)
                {
                    item.CONTAINERS.Add(new CONTAINER { NUMBER = servicioGeneral.Contenedor });
                    contador++;
                }
                transaccion.Complete();
            }
            servicioGeneral.ActualizarEntrada(servicioGeneral.PreGate.PRE_GATE_ID, "I");

            var v_trans = servicioGeneral.RepositorioTransaccionQuiosco.ObtenerObjetos(new FiltroTransaccionPorId(servicioGeneral.IdTransaccion)).FirstOrDefault();
            servicioGeneral.Procesos.AddRange(v_trans.PROCESSES);
            v_trans.PROCESSES = servicioGeneral.Procesos;
            servicioGeneral.RepositorioTransaccionQuiosco.Actualizar(v_trans);

            if (v_error)
            {
                ServicioAnunciante.AnunciarProblema(int.Parse(servicioGeneral.IdTransaccion.ToString()));
                return new RespuestaN4Depot { Mensaje = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeErrorSmdt))).FirstOrDefault().TROUBLE_DESK_MESSAGE, MensajeDetalle = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeErrorSmdt))).FirstOrDefault().DETAILS, FueOk = false, IdTransaccion = servicioGeneral.IdTransaccion, IdPreGateRecepcion = servicioGeneral.Id.Value , Token = servicioGeneral.Token};
            }
            else
            {
                return new RespuestaN4Depot { Mensaje = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeN4Ok))).FirstOrDefault().TROUBLE_DESK_MESSAGE, MensajeDetalle = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeN4Ok))).FirstOrDefault().DETAILS, FueOk = true, IdTransaccion = servicioGeneral.IdTransaccion, IdPreGateRecepcion = servicioGeneral.Id.Value , Token = servicioGeneral.Token };
            }
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

    internal class EstadoEventInvoiceN4 : EstadoKioscoZAL
    {
        internal override RespuestaN4Depot Ejecutar(ServicioGeneral servicioGeneral)
        {
            var ServicioAnunciante = new ServicioAnuncianteProblema.ServicioAnuncianteProblemaClient();
            var xml = ArmarXmlFacturar(servicioGeneral);
            var respuesta = servicioGeneral.Conector.Invocacion(xml);

            if (!respuesta.Estado.Equals("Successful"))
            {

                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeErrorN4),
                    STEP = "PROCESO N4",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = $"N4 RETORNO ERROR EN CARGA DE EVENTO A FACTURAR ZAL :{respuesta.ToString()}"
                });
                var v_trans = servicioGeneral.RepositorioTransaccionQuiosco.ObtenerObjetos(new FiltroTransaccionPorId(servicioGeneral.IdTransaccion)).FirstOrDefault();
                servicioGeneral.Procesos.AddRange(v_trans.PROCESSES);
                v_trans.PROCESSES = servicioGeneral.Procesos;
                servicioGeneral.RepositorioTransaccionQuiosco.Actualizar(v_trans);
                ServicioAnunciante.AnunciarProblema(int.Parse(servicioGeneral.IdTransaccion.ToString()));
                return new RespuestaN4Depot { Mensaje = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeErrorN4))).FirstOrDefault().TROUBLE_DESK_MESSAGE, MensajeDetalle = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse("51"))).FirstOrDefault().DETAILS, IdTransaccion =servicioGeneral.IdTransaccion };
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

                var v_trans = servicioGeneral.RepositorioTransaccionQuiosco.ObtenerObjetos(new FiltroTransaccionPorId(servicioGeneral.IdTransaccion)).FirstOrDefault();
                servicioGeneral.Procesos.AddRange(v_trans.PROCESSES);
                v_trans.PROCESSES = servicioGeneral.Procesos;
                servicioGeneral.RepositorioTransaccionQuiosco.Actualizar(v_trans);
                return new RespuestaN4Depot { Mensaje = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeN4Ok))).FirstOrDefault().TROUBLE_DESK_MESSAGE, MensajeDetalle = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeN4Ok))).FirstOrDefault().DETAILS, FueOk = true, IdTransaccion = servicioGeneral.IdTransaccion, Token= servicioGeneral.Token };
            }
        }

        private string ArmarXmlFacturar(ServicioGeneral servicioGeneral)
        {
            var fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            string evento = string.Empty;
            try
            {
                evento =  System.Configuration.ConfigurationManager.AppSettings["EVENTO_FACTURA_OPCACIFIC"];
            }
            catch
            {
                evento = "TRASLADO_CNTRS_CISE";
            }

            string xml = string.Format("<icu><units><unit-identity id=\"{0}\" type=\"CONTAINERIZED\"/>" +
                               "</units><properties><property tag=\"UnitRemark\" value=\"{4} P.C.\"/>" +
                               "</properties><event id=\"{3}\" note=\"{4} P.C.\" time-event-applied=\"{2}\" " +
                               "user-id=\"{1}\" /></icu>", servicioGeneral.Contenedor,"OPACIFIC", DateTime.Now.ToString("yyyy-MM-dd HH:mm"), evento, fecha);
            return xml;
        }
    }

    //GENERA SALIDA
    internal class EstadoSalidaProcessTruckDeliveryEmpty : EstadoKioscoZAL
    {
        internal override RespuestaN4Depot Ejecutar(ServicioGeneral servicioGeneral)
        {
            var ServicioAnunciante = new ServicioAnuncianteProblema.ServicioAnuncianteProblemaClient();
            var xml = ArmarXml(servicioGeneral);
            var respuesta = servicioGeneral.Conector.Invocacion(xml);
            servicioGeneral.ActualizarEntrada(servicioGeneral.PreGate.PRE_GATE_ID, "L");
            if (respuesta.RecepcionXmlOk && respuesta.IdEstado < 2 && !respuesta.ResultadosConsultas.Any(c => c.Result.Contains("TROUBLE")))
            {
                return servicioGeneral.GrabarRegistroProcesoSalida(true, 13, respuesta.ResultadosConsultas.FirstOrDefault().Result, servicioGeneral);
            }
            else
            {
                ServicioAnunciante.AnunciarProblema(int.Parse(servicioGeneral.IdTransaccion.ToString()));
                return servicioGeneral.GrabarRegistroProcesoSalida(false, 14, $"N4 RETORNO ERROR EN PROCESS TRUCK OUT :{respuesta.ToString()}", servicioGeneral);
            }

            /*
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

                var v_trans = servicioGeneral.RepositorioTransaccionQuiosco.ObtenerObjetos(new FiltroTransaccionPorId(servicioGeneral.IdTransaccion)).FirstOrDefault();
                servicioGeneral.Procesos.AddRange(v_trans.PROCESSES);
                v_trans.PROCESSES = servicioGeneral.Procesos;
                servicioGeneral.RepositorioTransaccionQuiosco.Actualizar(v_trans);
                return new RespuestaN4Depot { Mensaje = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeN4Ok))).FirstOrDefault().TROUBLE_DESK_MESSAGE, MensajeDetalle = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeN4Ok))).FirstOrDefault().DETAILS, FueOk = true, IdTransaccion = servicioGeneral.IdTransaccion, Token = servicioGeneral.Token };
            }
            else
            {
                servicioGeneral.Procesos.Add(new PROCESS
                {
                    MESSAGE_ID = Convert.ToInt32(Mensajes.MensajeErrorN4),
                    STEP = "PROCESO N4",
                    STEP_DATE = DateTime.Now,
                    RESPONSE = $"N4 RETORNO ERROR EN PROCESS TRUCK salida_cise :{respuesta.ToString()}"
                });
                var v_trans = servicioGeneral.RepositorioTransaccionQuiosco.ObtenerObjetos(new FiltroTransaccionPorId(servicioGeneral.IdTransaccion)).FirstOrDefault();
                servicioGeneral.Procesos.AddRange(v_trans.PROCESSES);
                v_trans.PROCESSES = servicioGeneral.Procesos;
                servicioGeneral.RepositorioTransaccionQuiosco.Actualizar(v_trans);
                ServicioAnunciante.AnunciarProblema(int.Parse(servicioGeneral.IdTransaccion.ToString()));
                return new RespuestaN4Depot { Mensaje = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse(Mensajes.MensajeErrorN4))).FirstOrDefault().TROUBLE_DESK_MESSAGE, MensajeDetalle = servicioGeneral.RepositorioMensaje.ObtenerObjetos(new FiltroMensajePorId(int.Parse("51"))).FirstOrDefault().DETAILS, IdTransaccion = servicioGeneral.IdTransaccion };
            }*/
        }

        public static string ArmarXml(ServicioGeneral datos)
        {
            var fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            return $@"<gate>
    	                <process-truck scan-status=""0"" no-content=""false"">
                            <gate-id>CISE</gate-id>
		                    <stage-id>salida_cise</stage-id>
                            <lane-id>KIOSCO 00</lane-id>
		                    <truck license-nbr=""{datos.PreGate.TRUCK_LICENCE}""/>
                            <driver license-nbr=""{datos.PreGate.DRIVER_ID}""/>
                            <truck-visit gos-tv-key=""{(datos.PreGate.PRE_GATE_ID * -1)}""/>
                            <timestamp>{fecha}</timestamp>
                        </process-truck>
                      </gate>";
        }

    
        //public static RespuestaN4Depot RegistrarProceso(KIOSK_TRANSACTION transaccion, ServicioGeneral servicioGeneral)
        //{
        //    using (var transaccionScope = new TransactionScope())
        //    {
        //        var transaccionQuiosco = servicioGeneral.RepositorioTransaccionQuiosco.ObtenerObjetos(new FiltroTransaccionPorId(servicioGeneral.IdTransaccion)).FirstOrDefault();
        //        if (transaccionQuiosco == null)
        //            return new RespuestaN4Depot { Mensaje = $"No existe transacción de quiosco # {transaccion.TRANSACTION_ID}" };
        //        var fecha = DateTime.Now;
        //        var proceso = transaccion.PROCESSES.FirstOrDefault();
        //        if (proceso == null)
        //            return new RespuestaN4Depot { Mensaje = "No existe proceso para la transacción." };
        //        transaccionQuiosco.END_DATE = fecha;
        //        transaccionQuiosco.IS_OK = transaccion.IS_OK;
        //        transaccionQuiosco.PROCESSES.Add(new PROCESS
        //        {
        //            IS_OK = proceso.IS_OK,
        //            MESSAGE_ID = proceso.MESSAGE_ID,
        //            RESPONSE = proceso.RESPONSE,
        //            STEP = proceso.STEP,
        //            STEP_DATE = fecha
        //        });
        //        servicioGeneral.RepositorioTransaccionQuiosco.Actualizar(transaccionQuiosco);
        //        if (transaccion.KIOSK.IS_IN)
        //            ActualizarPreGateEntrada(transaccion.PRE_GATE_ID.Value, proceso.IS_OK, transaccion.IS_OK, servicioGeneral);
        //        else
        //            ActualizarPreGateSalida(transaccion.PRE_GATE_ID.Value, proceso.IS_OK, transaccion.IS_OK, servicioGeneral);
        //        transaccionScope.Complete();
        //        return new RespuestaN4Depot { FueOk = true, Mensaje = "Registro Ok." };
        //    }
        //}

        //private void ActualizarPreGateEntrada(long idPreGate, bool fueOk, bool procesoFinalizado, ServicioGeneral servicioGeneral)
        //{
        //    var preGate = servicioGeneral.RepositorioPreGate.ObtenerObjetos(new FiltroPreGatePorId(idPreGate)).FirstOrDefault();
        //    if (fueOk && preGate.STATUS == "N")
        //        preGate.STATUS = "P";
        //    else if (!fueOk && preGate.STATUS == "P")
        //        preGate.STATUS = "N";
        //    else if (procesoFinalizado)
        //        preGate.STATUS = "I";
        //    else
        //        return;
        //    servicioGeneral.RepositorioPreGate.Actualizar(preGate);
        //}
        //private void ActualizarPreGateSalida(long idPreGate, bool fueOk, bool procesoFinalizado, ServicioGeneral servicioGeneral)
        //{
            
        //    var preGate = servicioGeneral.RepositorioPreGate.ObtenerObjetos(new FiltroPreGatePorId(idPreGate)).FirstOrDefault();
        //    if (fueOk && preGate.STATUS == "I")
        //        preGate.STATUS = "L";
        //    else if (!fueOk && preGate.STATUS == "L")
        //        preGate.STATUS = "I";
        //    else if (procesoFinalizado)
        //        preGate.STATUS = "O";
        //    else
        //        return;
        //    servicioGeneral.RepositorioPreGate.Actualizar(preGate);
        //}
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

    public class FiltroMensajePorId : Filtros<MESSAGE>
    {
        private readonly int _id;

        internal FiltroMensajePorId(int id)
        {
            _id = id;
        }

        public override Expression<Func<MESSAGE, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<MESSAGE>(p => p.MESSAGE_ID == _id);
            return filtro.SastifechoPor();
        }
    }

    public class FiltroTransaccionPorId : Filtros<KIOSK_TRANSACTION>
    {
        private readonly long _id;

        internal FiltroTransaccionPorId(long id)
        {
            _id = id;
        }

        public override Expression<Func<KIOSK_TRANSACTION, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<KIOSK_TRANSACTION>(p => p.TRANSACTION_ID == _id);
            return filtro.SastifechoPor();
        }
    }
}
