using RECEPTIO.CapaAplicacion.Transaction.Aplicacion.Contratos;
using RECEPTIO.CapaAplicacion.Transaction.Aplicacion.Interfaces;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaDominio.Transaction.Dominio.InterfacesRepositorios;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using System.Xml;

namespace RECEPTIO.CapaDominio.Transaction.ServiciosDominio
{
    public class ServicioPreGate : IPreGate
    {
        private EstadoPreGate _estado;
        private EstadoPreGateSalida _estadoSalida;
        private Dictionary<short, bool> _valoresSensores;
        internal short IdQuiosco;
        private readonly IRepositorioTransaccionQuiosco _repositorioTransaccion;
        internal readonly IRepositorioPreGate RepositorioPreGate;
        internal readonly IRepositorioValidaAduana RepositorioValidaAduana;
        internal readonly IRepositorioValidacionesN4 RepositorioValidacionesN4;
        internal readonly IRepositorioQuiosco RepositorioQuiosco;
        internal string Cedula;
        internal IEnumerable<PRE_GATE> PreGates;
        internal PRE_GATE PreGate;
        internal long IdPreGateDigitado;
        internal KIOSK Quiosco;
        internal int Peso;

        public ServicioPreGate(IRepositorioPreGate repositorioPreGate, IRepositorioTransaccionQuiosco repositorioTransaccion, IRepositorioValidaAduana repositorioValidaAduana, IRepositorioValidacionesN4 repositorioValidacionesN4, IRepositorioQuiosco repositorioQuiosco)
        {
            RepositorioPreGate = repositorioPreGate;
            _repositorioTransaccion = repositorioTransaccion;
            RepositorioValidaAduana = repositorioValidaAduana;
            RepositorioValidacionesN4 = repositorioValidacionesN4;
            RepositorioQuiosco = repositorioQuiosco;
        }

        public DatosPreGate ValidarPreGate(string cedula, short idQuiosco, Dictionary<short, bool> valoresSensores)
        {
            Cedula = cedula;
            IdQuiosco = idQuiosco;
            _valoresSensores = valoresSensores;
            _estado = new EstadoValidarExistenciaPreGate();
            return _estado.Validar(this);
        }

        internal int RegistrarTransaccion(int idMensaje, bool fueOk, string repuesta = "")
        {
            var fecha = DateTime.Now;
            var transaccion = new KIOSK_TRANSACTION
            {
                KIOSK_ID = IdQuiosco,
                PRE_GATE_ID = PreGate == null || PreGate.PRE_GATE_ID == 0 ? new long?(): PreGate.PRE_GATE_ID,
                START_DATE = fecha,
                END_DATE = fecha,
                PROCESSES = new List<PROCESS>
                {
                    new PROCESS
                    {
                        IS_OK = fueOk,
                        MESSAGE_ID = idMensaje,
                        RESPONSE = repuesta,
                        STEP = "PRE_GATE",
                        STEP_DATE = fecha
                    }
                },
                SENSOR_KIOSK_TRANSACTIONS = SensoresKiosco()
            };
            _repositorioTransaccion.Agregar(transaccion);
            return transaccion.TRANSACTION_ID;
        }

        private ICollection<SENSOR_KIOSK_TRANSACTION> SensoresKiosco()
        {
            var resultado = new List<SENSOR_KIOSK_TRANSACTION>();
            foreach (var valorSensor in _valoresSensores)
                resultado.Add(new SENSOR_KIOSK_TRANSACTION { SENSOR_ID = valorSensor.Key, VALUE = valorSensor.Value});
            return resultado;
        }

        public DatosPreGateSalida ValidarEntradaQuiosco(long idPreGate, short idQuiosco, Dictionary<short, bool> valoresSensores)
        {
            IdPreGateDigitado = idPreGate;
            IdQuiosco = idQuiosco;
            _valoresSensores = valoresSensores;
            _estadoSalida = new EstadoValidarExistenciaPreGateParaSalida();
            return _estadoSalida.Validar(this);
        }

        internal DatosPreGate SetearEstado(EstadoPreGate estado)
        {
            _estado = estado;
            return _estado.Validar(this);
        }

        internal DatosPreGateSalida SetearEstadoSalida(EstadoPreGateSalida estadoSalida)
        {
            _estadoSalida = estadoSalida;
            return _estadoSalida.Validar(this);
        }

        public void LiberarRecursos()
        {
            RepositorioPreGate.LiberarRecursos();
            _repositorioTransaccion.LiberarRecursos();
            RepositorioQuiosco.LiberarRecursos();
        }
    }

    internal abstract class EstadoPreGate
    {
        internal abstract DatosPreGate Validar(ServicioPreGate servicioPreGate);
    }

    internal class EstadoValidarExistenciaPreGate : EstadoPreGate
    {
        internal override DatosPreGate Validar(ServicioPreGate servicioPreGate)
        {
            var preGates = servicioPreGate.RepositorioPreGate.ObtenerPreGateConDetalle(new FiltroPreGatePorCedula(servicioPreGate.Cedula, servicioPreGate.IdQuiosco));
            if(preGates.Count() == 0)
                return new DatosPreGate { Mensaje = Mensajes.MensajeNoExistePreGate, IdTransaccion = servicioPreGate.RegistrarTransaccion(Convert.ToInt32(Mensajes.MensajeNoExistePreGate), false) };
            servicioPreGate.PreGates = preGates;
            return servicioPreGate.SetearEstado(new EstadoValidarUnicoRegistro());
        }
    }

    internal class EstadoValidarUnicoRegistro : EstadoPreGate
    {
        internal override DatosPreGate Validar(ServicioPreGate servicioPreGate)
        {
            if (servicioPreGate.PreGates.Count() != 1)
                return new DatosPreGate { Mensaje = Mensajes.MensajeMultiplesPreGates, IdTransaccion = servicioPreGate.RegistrarTransaccion(Convert.ToInt32(Mensajes.MensajeMultiplesPreGates), false) };
            servicioPreGate.PreGate = servicioPreGate.PreGates.FirstOrDefault();
            return servicioPreGate.SetearEstado(new SwitcheadorEntrada126911());
        }
    }

    internal class SwitcheadorEntrada126911 : EstadoPreGate
    {
        internal override DatosPreGate Validar(ServicioPreGate servicioPreGate)
        {
            if (servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 1) || servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 2) || servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 6) || servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 9) || servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 11) || servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 17) || servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 18))
                return servicioPreGate.SetearEstado(new EstadoIngresoTransaccion());
            else
                return servicioPreGate.SetearEstado(new SwitcheadorEntrada37());
        }
    }

    internal class SwitcheadorEntrada37 : EstadoPreGate
    {
        internal override DatosPreGate Validar(ServicioPreGate servicioPreGate)
        {
            if (servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 3) || servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 7) || servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 19))
                return servicioPreGate.SetearEstado(new EstadoVerificaExistenciaContenedor());
            else
                return servicioPreGate.SetearEstado(new SwitcheadorEntrada45Proveedores());
        }
    }

    internal class SwitcheadorEntrada45Proveedores : EstadoPreGate
    {
        internal override DatosPreGate Validar(ServicioPreGate servicioPreGate)
        {
            if (servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 4) || servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 5) || servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 12) || servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 13) || servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 14) || servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 15) || servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 16))
                return servicioPreGate.SetearEstado(new EstadoConsultaQuiosco());
            else
                return servicioPreGate.SetearEstado(new SwitcheadorEntrada8());
        }
    }

    internal class SwitcheadorEntrada8 : EstadoPreGate
    {
        internal override DatosPreGate Validar(ServicioPreGate servicioPreGate)
        {
            if (servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 8))
                return servicioPreGate.SetearEstado(new EstadoConsultarPesoBanano());
            else
                return servicioPreGate.SetearEstado(new SwitcheadorEntrada10());
        }
    }

    internal class SwitcheadorEntrada10 : EstadoPreGate
    {
        internal override DatosPreGate Validar(ServicioPreGate servicioPreGate)
        {
            if (servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 10))
                return servicioPreGate.SetearEstado(new EstadoVerificaExistenciaContenedorMixta());
            else
                return new DatosPreGate { Mensaje = Mensajes.MensajeTipoTransaccionesNoContempladas, IdTransaccion = servicioPreGate.RegistrarTransaccion(Convert.ToInt32(Mensajes.MensajeTipoTransaccionesNoContempladas), false) };
        }
    }

    internal class EstadoVerificaExistenciaContenedorMixta : EstadoPreGate
    {
        internal override DatosPreGate Validar(ServicioPreGate servicioPreGate)
        {
            if (servicioPreGate.PreGate.PRE_GATE_DETAILS.Where(pgd => pgd.REFERENCE_ID == "E").All(pgd => pgd.CONTAINERS.Count() == 1))
                return servicioPreGate.SetearEstado(new EstadoIngresoTransaccion());
            return new DatosPreGate { Mensaje = Mensajes.MensajeNoExistenciaContenedor, IdTransaccion = servicioPreGate.RegistrarTransaccion(Convert.ToInt32(Mensajes.MensajeNoExistenciaContenedor), false) };
        }
    }

    internal class EstadoConsultarPesoBanano : EstadoPreGate
    {
        internal override DatosPreGate Validar(ServicioPreGate servicioPreGate)
        {
            servicioPreGate.Peso = Convert.ToInt32(servicioPreGate.PreGate.PRE_GATE_DETAILS.FirstOrDefault().REFERENCE_ID) * Convert.ToInt32(ConfigurationManager.AppSettings["CantidaPesoReferencial"]);
            return servicioPreGate.SetearEstado(new EstadoIngresoTransaccion());
        }
    }

    internal class EstadoVerificaExistenciaContenedor : EstadoPreGate
    {
        internal override DatosPreGate Validar(ServicioPreGate servicioPreGate)
        {
            if (servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 19))
            {
                if (servicioPreGate.PreGate.PRE_GATE_DETAILS.Where(pgd => pgd.REFERENCE_ID == "E").All(pgd => pgd.CONTAINERS.Count() == 1))
                    return servicioPreGate.SetearEstado(new EstadoIngresoTransaccion());
                return new DatosPreGate { Mensaje = Mensajes.MensajeNoExistenciaContenedor, IdTransaccion = servicioPreGate.RegistrarTransaccion(Convert.ToInt32(Mensajes.MensajeNoExistenciaContenedor), false) };
            }
            else
            {
                if (servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.CONTAINERS.Count() == 1))
                    return servicioPreGate.SetearEstado(new EstadoIngresoTransaccion());
                return new DatosPreGate { Mensaje = Mensajes.MensajeNoExistenciaContenedor, IdTransaccion = servicioPreGate.RegistrarTransaccion(Convert.ToInt32(Mensajes.MensajeNoExistenciaContenedor), false) };
            }
        }
    }

    internal class EstadoConsultaQuiosco : EstadoPreGate
    {
        internal override DatosPreGate Validar(ServicioPreGate servicioPreGate)
        {
            servicioPreGate.Quiosco = servicioPreGate.RepositorioQuiosco.ObtenerObjetos(new FiltroQuioscoPorId(servicioPreGate.IdQuiosco)).FirstOrDefault();
            return servicioPreGate.SetearEstado(new EstadoConsultarPeso());
        }
    }

    internal class EstadoConsultarPeso : EstadoPreGate
    {
        internal override DatosPreGate Validar(ServicioPreGate servicioPreGate)
        {
            //if (servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 4))
            //{
            //    servicioPreGate.Peso = 0;
            //    return servicioPreGate.SetearEstado(new EstadoIngresoTransaccion());
            //}

            var xml = ArmarXml(servicioPreGate);
            var servicioWeb = new ServicioPesaje.n4ServiceSoapClient();
            var respuesta = servicioWeb.basicInvoke(ConfigurationManager.AppSettings["scopeCoordinateIds"], xml);
            if (respuesta.Contains(@"message>OK</message"))
            {
                servicioPreGate.Peso = ObtenerPeso(respuesta);
                return servicioPreGate.SetearEstado(new EstadoIngresoTransaccion());
            }
            return new DatosPreGate { Mensaje = Mensajes.MensajePesaje, IdTransaccion = servicioPreGate.RegistrarTransaccion(Convert.ToInt32(Mensajes.MensajePesaje), false) };
        }

        private int ObtenerPeso(string respuesta)
        {
            var xml =new XmlDocument();
            xml.LoadXml($"<documento>{respuesta}</documento>");
            return Convert.ToInt32(xml.FirstChild.ChildNodes[2].InnerText);
        }

        private static string ArmarXml(ServicioPreGate servicioPreGate)
        {
            return $@"<?xml  version='1.0' encoding='utf-8'?><documento><transaction>{servicioPreGate.PreGate.PRE_GATE_ID}</transaction><User>{servicioPreGate.Quiosco.NAME}</User><Date>{DateTime.Now}</Date><Console>{servicioPreGate.Quiosco.IP}</Console><Event>PESAJE</Event><Details><Cedula>{servicioPreGate.PreGate.DRIVER_ID}</Cedula></Details></documento>";
        }
    }

    internal class EstadoIngresoTransaccion : EstadoPreGate
    {
        internal override DatosPreGate Validar(ServicioPreGate servicioPreGate)
        {
            int idTransaccion;
            using (var transaccion = new TransactionScope())
            {
                try
                {
                    ActualizarPreGate(servicioPreGate);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    servicioPreGate.RegistrarTransaccion(Convert.ToInt32(Mensajes.MensajeConcurrencia), false, $"Mensaje : {ex.Message}///Excepción Interna : {ex.InnerException}///Pila de Seguimiento : {ex.StackTrace}///Fuente : {ex.Source}///Link : {ex.HelpLink}");
                    return new DatosPreGate { Mensaje = Mensajes.MensajeConcurrencia };
                }
                idTransaccion = servicioPreGate.RegistrarTransaccion(Convert.ToInt32(Mensajes.MensajePreGateOk), true);
                transaccion.Complete();
            }
            return new DatosPreGate
            {
                FueOk = true,
                IdTransaccion = idTransaccion,
                Mensaje = Mensajes.MensajePreGateOk,
                PreGate = servicioPreGate.PreGate,
                Peso = servicioPreGate.Peso
            };
        }

        private void ActualizarPreGate(ServicioPreGate servicioPreGate)
        {
            servicioPreGate.PreGate.STATUS = "P";
            servicioPreGate.RepositorioPreGate.Actualizar(servicioPreGate.PreGate);
        }
    }

    internal abstract class EstadoPreGateSalida
    {
        internal abstract DatosPreGateSalida Validar(ServicioPreGate servicioPreGate);
    }

    internal class EstadoValidarExistenciaPreGateParaSalida : EstadoPreGateSalida
    {
        internal override DatosPreGateSalida Validar(ServicioPreGate servicioPreGate)
        {
            var preGate = servicioPreGate.RepositorioPreGate.ObtenerPreGateConDetalle(new FiltroPreGatePorId(servicioPreGate.IdPreGateDigitado)).FirstOrDefault();
            if (preGate == null)
                return new DatosPreGateSalida { Mensaje = Mensajes.MensajeNoExisteCodigo , IdTransaccion = servicioPreGate.RegistrarTransaccion(Convert.ToInt32(Mensajes.MensajeNoExisteCodigo), false).ToString() };
            servicioPreGate.PreGate = preGate;
            return servicioPreGate.SetearEstadoSalida(new EstadoValidarSalidaTerminal());
        }
    }

    internal class EstadoValidarSalidaTerminal : EstadoPreGateSalida
    {
        internal override DatosPreGateSalida Validar(ServicioPreGate servicioPreGate)
        {
            if(servicioPreGate.PreGate.STATUS == "O")
                return new DatosPreGateSalida { Mensaje = Mensajes.MensajeSalidaExistente, IdTransaccion = servicioPreGate.RegistrarTransaccion(Convert.ToInt32(Mensajes.MensajeSalidaExistente), false).ToString() };
            return servicioPreGate.SetearEstadoSalida(new EstadoValidarEntradaTerminal());
        }
    }

    internal class EstadoValidarEntradaTerminal : EstadoPreGateSalida
    {
        internal override DatosPreGateSalida Validar(ServicioPreGate servicioPreGate)
        {
            if (servicioPreGate.PreGate.STATUS == "I" || (servicioPreGate.PreGate.STATUS == "L" && servicioPreGate.PreGate.KIOSK_TRANSACTIONS.Any(kt => kt.KIOSK_ID == servicioPreGate.IdQuiosco)))
                return servicioPreGate.SetearEstadoSalida(new SwitcheadorSalida16());
            return new DatosPreGateSalida { Mensaje = Mensajes.MensajeNoExisteEntradaQuiosco, IdTransaccion = servicioPreGate.RegistrarTransaccion(Convert.ToInt32(Mensajes.MensajeNoExisteEntradaQuiosco), false).ToString() };
        }
    }

    internal class SwitcheadorSalida16 : EstadoPreGateSalida
    {
        internal override DatosPreGateSalida Validar(ServicioPreGate servicioPreGate)
        {
            if (servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 1) || servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 6))
                return servicioPreGate.SetearEstadoSalida(new EstadoVerificarExistenciaContenedor());
            return servicioPreGate.SetearEstadoSalida(new SwitcheadorSalida37());
        }
    }

    internal class SwitcheadorSalida37 : EstadoPreGateSalida
    {
        internal override DatosPreGateSalida Validar(ServicioPreGate servicioPreGate)
        {
            if (servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 3) || servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 7) || servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 19))
                return servicioPreGate.SetearEstadoSalida(new EstadoVerificaContenedoresEnPatio());
            return servicioPreGate.SetearEstadoSalida(new SwitcheadorSalida45Proveedor());
        }
    }

    internal class SwitcheadorSalida45Proveedor : EstadoPreGateSalida
    {
        internal override DatosPreGateSalida Validar(ServicioPreGate servicioPreGate)
        {
            if (servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 4) || servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 5) || servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 12) || servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 13) || servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 14) || servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 15) || servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 16))
                return servicioPreGate.SetearEstadoSalida(new EstadoConsultaQuioscoSalida());
            else
                return servicioPreGate.SetearEstadoSalida(new SwitcheadorSalida289());
        }
    }

    internal class SwitcheadorSalida289 : EstadoPreGateSalida
    {
        internal override DatosPreGateSalida Validar(ServicioPreGate servicioPreGate)
        {
            if (servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 2) /*|| servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 4)*/ || servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 8) || servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 9) || servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 18))
                return servicioPreGate.SetearEstadoSalida(new EstadoIngresoTransaccionSalida());
            else
                return servicioPreGate.SetearEstadoSalida(new SwitcheadorSalida10());
        }
    }

    internal class SwitcheadorSalida10 : EstadoPreGateSalida
    {
        internal override DatosPreGateSalida Validar(ServicioPreGate servicioPreGate)
        {
            if (servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 10))
                    return servicioPreGate.SetearEstadoSalida(new EstadoVerificaContenedoresEnPatioMixta());
            else
                return servicioPreGate.SetearEstadoSalida(new SwitcheadorSalida1117());
        }
    }

    internal class SwitcheadorSalida1117 : EstadoPreGateSalida
    {
        internal override DatosPreGateSalida Validar(ServicioPreGate servicioPreGate)
        {
            if ((servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 11)) || (servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 17)))
                return servicioPreGate.SetearEstadoSalida(new EstadoValidaSmdt());
            else
                return new DatosPreGateSalida { Mensaje = Mensajes.MensajeTipoTransaccionesNoContempladas, IdTransaccion = servicioPreGate.RegistrarTransaccion(Convert.ToInt32(Mensajes.MensajeTipoTransaccionesNoContempladas), false).ToString() };
        }
    }

    internal class EstadoConsultaQuioscoSalida : EstadoPreGateSalida
    {
        internal override DatosPreGateSalida Validar(ServicioPreGate servicioPreGate)
        {
            servicioPreGate.Quiosco = servicioPreGate.RepositorioQuiosco.ObtenerObjetos(new FiltroQuioscoPorId(servicioPreGate.IdQuiosco)).FirstOrDefault();
            return servicioPreGate.SetearEstadoSalida(new EstadoConsultarPesoSalida());
        }
    }

    internal class EstadoConsultarPesoSalida : EstadoPreGateSalida
    {
        internal override DatosPreGateSalida Validar(ServicioPreGate servicioPreGate)
        {
            var xml = ArmarXml(servicioPreGate);
            var servicioWeb = new ServicioPesaje.n4ServiceSoapClient();
            var respuesta = servicioWeb.basicInvoke(ConfigurationManager.AppSettings["scopeCoordinateIds"], xml);
            if (respuesta.Contains(@"message>OK</message"))
            {
                servicioPreGate.Peso = ObtenerPeso(respuesta);
                return servicioPreGate.SetearEstadoSalida(new EstadoIngresoTransaccionSalida());
            }
            return new DatosPreGateSalida { Mensaje = Mensajes.MensajePesaje, IdTransaccion = servicioPreGate.RegistrarTransaccion(Convert.ToInt32(Mensajes.MensajePesaje), false).ToString() };
        }

        private int ObtenerPeso(string respuesta)
        {
            var xml = new XmlDocument();
            xml.LoadXml($"<documento>{respuesta}</documento>");
            return Convert.ToInt32(xml.FirstChild.ChildNodes[2].InnerText);
        }

        private static string ArmarXml(ServicioPreGate servicioPreGate)
        {
            return $@"<?xml  version='1.0' encoding='utf-8'?><documento><transaction>{servicioPreGate.PreGate.PRE_GATE_ID}</transaction><User>{servicioPreGate.Quiosco.NAME}</User><Date>{DateTime.Now}</Date><Console>{servicioPreGate.Quiosco.IP}</Console><Event>PESAJE</Event><Details><Cedula>{servicioPreGate.PreGate.DRIVER_ID}</Cedula></Details></documento>";
        }
    }

    internal class EstadoVerificarExistenciaContenedor : EstadoPreGateSalida
    {
        internal override DatosPreGateSalida Validar(ServicioPreGate servicioPreGate)
        {
            if (!servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.CONTAINERS.Count() == 1))
                return new DatosPreGateSalida { Mensaje = Mensajes.MensajeNoExistenciaContenedor, IdTransaccion = servicioPreGate.RegistrarTransaccion(Convert.ToInt32(Mensajes.MensajeNoExistenciaContenedor), false).ToString() };
            return servicioPreGate.SetearEstadoSalida(new EstadoIngresoTransaccionSalida());
        }
    }

    internal class EstadoVerificaContenedoresEnPatio : EstadoPreGateSalida
    {
        internal override DatosPreGateSalida Validar(ServicioPreGate servicioPreGate)
        {
            foreach (var detalle in servicioPreGate.PreGate.PRE_GATE_DETAILS)
            {
                if (!servicioPreGate.RepositorioValidacionesN4.EstaEnPatioContenedor(detalle.CONTAINERS.FirstOrDefault().NUMBER))
                    return new DatosPreGateSalida { Mensaje = Mensajes.MensajeContendorNoEstaEnPatio, IdTransaccion = servicioPreGate.RegistrarTransaccion(Convert.ToInt32(Mensajes.MensajeContendorNoEstaEnPatio), false, detalle.CONTAINERS.FirstOrDefault().NUMBER).ToString() };
            }
            return servicioPreGate.SetearEstadoSalida(new EstadoIngresoTransaccionSalida());
        }
    }

    internal class EstadoVerificaContenedoresEnPatioMixta : EstadoPreGateSalida
    {
        internal override DatosPreGateSalida Validar(ServicioPreGate servicioPreGate)
        {
            foreach (var detalle in servicioPreGate.PreGate.PRE_GATE_DETAILS.Where(pgd => pgd.REFERENCE_ID == "E"))
            {
                if (!servicioPreGate.RepositorioValidacionesN4.EstaEnPatioContenedor(detalle.CONTAINERS.FirstOrDefault().NUMBER))
                    return new DatosPreGateSalida { Mensaje = Mensajes.MensajeContendorNoEstaEnPatio, IdTransaccion = servicioPreGate.RegistrarTransaccion(Convert.ToInt32(Mensajes.MensajeContendorNoEstaEnPatio), false, detalle.CONTAINERS.FirstOrDefault().NUMBER).ToString() };
            }
            return servicioPreGate.SetearEstadoSalida(new EstadoIngresoTransaccionSalida());
        }
    }

    internal class EstadoValidaSmdt : EstadoPreGateSalida
    {
        internal override DatosPreGateSalida Validar(ServicioPreGate servicioPreGate)
        {
            if (ValidaSmdt(servicioPreGate))
                return servicioPreGate.SetearEstadoSalida(new EstadoIngresoTransaccionSalida());
            else
                return new DatosPreGateSalida { Mensaje = Mensajes.MensajeAduana, IdTransaccion = servicioPreGate.RegistrarTransaccion(Convert.ToInt32(Mensajes.MensajeAduana), false).ToString() };
        }

        private bool ValidaSmdt(ServicioPreGate servicioPreGate)
        {
            if (!servicioPreGate.RepositorioValidaAduana.ValidaSmdt((servicioPreGate.PreGate.PRE_GATE_ID * -1).ToString()))
                return false;
            return true;
        }
    }

    internal class EstadoIngresoTransaccionSalida : EstadoPreGateSalida
    {
        internal override DatosPreGateSalida Validar(ServicioPreGate servicioPreGate)
        {
            int idTransaccion;
            using (var transaccion = new TransactionScope())
            {
                if (servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 1) || servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 2) || servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 18))
                {
                    if (servicioPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 18))
                    {
                        if (!ValidaSmdtP2D(servicioPreGate))
                        {
                            idTransaccion = servicioPreGate.RegistrarTransaccion(Convert.ToInt32(Mensajes.MensajeAduana), false);
                            transaccion.Complete();
                            return new DatosPreGateSalida
                            {
                                IdTransaccion = idTransaccion.ToString(),
                                Mensaje = Mensajes.MensajeAduana,
                                PreGate = servicioPreGate.PreGate
                            };
                        }
                    }
                    else
                    {
                        if (!ValidaSmdt(servicioPreGate))
                        {
                            idTransaccion = servicioPreGate.RegistrarTransaccion(Convert.ToInt32(Mensajes.MensajeAduana), false);
                            transaccion.Complete();
                            return new DatosPreGateSalida
                            {
                                IdTransaccion = idTransaccion.ToString(),
                                Mensaje = Mensajes.MensajeAduana,
                                PreGate = servicioPreGate.PreGate
                            };
                        }
                    }
                }
                try
                {
                    ActualizarPreGate(servicioPreGate);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return new DatosPreGateSalida { Mensaje = Mensajes.MensajeConcurrencia, IdTransaccion = servicioPreGate.RegistrarTransaccion(Convert.ToInt32(Mensajes.MensajeConcurrencia), false, $"Mensaje : {ex.Message}///Excepción Interna : {ex.InnerException}///Pila de Seguimiento : {ex.StackTrace}///Fuente : {ex.Source}///Link : {ex.HelpLink}").ToString() };
                }
                idTransaccion = servicioPreGate.RegistrarTransaccion(Convert.ToInt32(Mensajes.MensajePreGateOk), true);
                transaccion.Complete();
            }
            return new DatosPreGateSalida
            {
                FueOk = true,
                IdTransaccion = idTransaccion.ToString(),
                Mensaje = Mensajes.MensajePreGateOk,
                PreGate = servicioPreGate.PreGate,
                Peso = servicioPreGate.Peso
            };
        }

        private void ActualizarPreGate(ServicioPreGate servicioPreGate)
        {
            servicioPreGate.PreGate.STATUS = "L";
            servicioPreGate.RepositorioPreGate.Actualizar(servicioPreGate.PreGate);
        }

        private bool ValidaSmdt(ServicioPreGate servicioPreGate)
        {
            foreach (var detalle in servicioPreGate.PreGate.PRE_GATE_DETAILS)
            {
                if (!servicioPreGate.RepositorioValidaAduana.ValidaSmdt(detalle.TRANSACTION_NUMBER))
                    return false;
            }
            return true;
        }

        private bool ValidaSmdtP2D(ServicioPreGate servicioPreGate)
        {
            string[] v_pasePuerta;
            foreach (var detalle in servicioPreGate.PreGate.PRE_GATE_DETAILS)
            {
                try
                {
                    v_pasePuerta = detalle.TRANSACTION_NUMBER.Split('-');

                    if (!servicioPreGate.RepositorioValidaAduana.ValidaSmdt(v_pasePuerta[0]))
                    {
                        return false;
                    }
                }catch
                {
                    return false;
                }
                
            }
            return true;
        }
    }

    public class FiltroPreGatePorCedula : Filtros<PRE_GATE>
    {
        private readonly string _cedula;
        private readonly short _idQuiosco;

        internal FiltroPreGatePorCedula(string cedula, short idQuiosco)
        {
            _cedula = cedula;
            _idQuiosco = idQuiosco;
        }

        public override Expression<Func<PRE_GATE, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<PRE_GATE>(p => p.DRIVER_ID == _cedula && (p.STATUS == "N" || (p.STATUS == "P" && p.KIOSK_TRANSACTIONS.Any(kt => kt.KIOSK_ID == _idQuiosco))));
            return filtro.SastifechoPor();
        }
    }

    public class FiltroPreGatePorId : Filtros<PRE_GATE>
    {
        private readonly long _id;

        public FiltroPreGatePorId(long id)
        {
            _id = id;
        }

        public override Expression<Func<PRE_GATE, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<PRE_GATE>(pg => pg.PRE_GATE_ID == _id);
            return filtro.SastifechoPor();
        }
    }

    public class FiltroQuioscoPorId : IFiltros<KIOSK>
    {
        private readonly short _idQuiosco;

        public FiltroQuioscoPorId(short idQuiosco)
        {
            _idQuiosco = idQuiosco;
        }

        public Expression<Func<KIOSK, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<KIOSK>(k => k.KIOSK_ID == _idQuiosco);
            return filtro.SastifechoPor();
        }
    }
}
