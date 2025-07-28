using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Contratos;
using RECEPTIO.CapaAplicacion.Transaction.Aplicacion.Interfaces;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaDominio.Nucleo.ServiciosDominio.Filtros;
using System;
using System.Linq;
using System.Transactions;

namespace RECEPTIO.CapaDominio.Transaction.ServiciosDominio
{
    public class ServicioTransaccionQuiosco : ITransaccionQuiosco
    {
        private readonly IRepositorioTransaccionQuiosco _repositorio;
        private readonly IRepositorioPreGate _repositorioPreGate;

        public ServicioTransaccionQuiosco(IRepositorioTransaccionQuiosco repositorio, IRepositorioPreGate repositorioPreGate)
        {
            _repositorio = repositorio;
            _repositorioPreGate = repositorioPreGate;
        }

        public Respuesta RegistrarProceso(KIOSK_TRANSACTION transaccion)
        {
            using (var transaccionScope = new TransactionScope())
            {
                var transaccionQuiosco = _repositorio.ObtenerObjetos(new FiltroTransaccionQuioscoPorId(transaccion.TRANSACTION_ID)).FirstOrDefault();
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
                _repositorio.Actualizar(transaccionQuiosco);
                if(transaccion.KIOSK.IS_IN)
                    ActualizarPreGateEntrada(transaccion.PRE_GATE_ID.Value, proceso.IS_OK, transaccion.IS_OK);
                else
                    ActualizarPreGateSalida(transaccion.PRE_GATE_ID.Value, proceso.IS_OK, transaccion.IS_OK);
                transaccionScope.Complete();
                return new Respuesta { FueOk = true, Mensaje = "Registro Ok." };
            }
        }

        private void ActualizarPreGateEntrada(long idPreGate, bool fueOk, bool procesoFinalizado)
        {
            var preGate = _repositorioPreGate.ObtenerObjetos(new FiltroPreGatePorId(idPreGate)).FirstOrDefault();
            if (fueOk && preGate.STATUS == "N")
                preGate.STATUS = "P";
            else if (!fueOk && preGate.STATUS == "P")
                preGate.STATUS = "N";
            else if (procesoFinalizado)
                preGate.STATUS = "I";
            else
                return;
            _repositorioPreGate.Actualizar(preGate);
        }

        private void ActualizarPreGateSalida(long idPreGate, bool fueOk, bool procesoFinalizado)
        {
            var preGate = _repositorioPreGate.ObtenerObjetos(new FiltroPreGatePorId(idPreGate)).FirstOrDefault();
            if (fueOk && preGate.STATUS == "I")
                preGate.STATUS = "L";
            else if (!fueOk && preGate.STATUS == "L")
                preGate.STATUS = "I";
            else if (procesoFinalizado)
                preGate.STATUS = "O";
            else
                return;
            _repositorioPreGate.Actualizar(preGate);
        }

        public void LiberarRecursos()
        {
            _repositorio.LiberarRecursos();
            _repositorioPreGate.LiberarRecursos();
        }
    }
}
