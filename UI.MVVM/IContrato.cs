using RECEPTIO.CapaDominio.Nucleo.Entidades;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace RECEPTIO.CapaPresentacion.UI.MVVM
{
    [ServiceContract()]
    public interface IContrato
    {
        [OperationContract]
        Tuple<List<string>, List<string>> EstadoImpresora();

        [OperationContract]
        Tuple<bool, string> EstadoAntena();

        [OperationContract]
        Tuple<bool, string> EstadoBarrera();

        [OperationContract]
        void AbrirBarreraLider();

        [OperationContract]
        Tuple<bool, string> AbrirBarreraTroubleDesk();

        [OperationContract]
        Tuple<bool, string> IrHomeQuiosco();

        [OperationContract]
        Tuple<bool, string> ReimprimirTicket();

        [OperationContract]
        Tuple<bool, string> ReimprimirCualquierTicket(PRE_GATE preGate, string xml);
    }
}
