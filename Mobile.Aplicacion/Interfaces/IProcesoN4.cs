using RECEPTIO.CapaAplicacion.Mobile.Aplicacion.Contratos;
using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Interfaces;
using System.ServiceModel;
using System.Collections.Generic;

namespace RECEPTIO.CapaAplicacion.Mobile.Aplicacion.Interfaces
{
    [ServiceContract]
    public interface IProcesoN4 : IBase
    {
        [OperationContract]
        RespuestaProceso EjecutarProcesosDeliveryImportFull(DatosN4 datos); //1,6

        [OperationContract]
        RespuestaProceso EjecutarProcesosDeliveryImportBrBkCfs(DatosDeliveryImportBrbkCfs datos);//2

        [OperationContract]
        RespuestaProceso EjecutarProcesosReceiveExport(DatosReceiveExport datos);//3,7

        [OperationContract]
        RespuestaProceso EjecutarProcesosReceiveExportBrBk(DatosReceiveExportBrBk datos);//4,5

        [OperationContract]
        RespuestaProceso EjecutarProcesosReceiveExportBanano(DatosReceiveExportBanano datos);//8,9

        [OperationContract]
        RespuestaProceso LiberarHold(DatosN4 datos);

        [OperationContract]
        RespuestaProceso CambiarHold(DatosN4 datos);

        [OperationContract]
        RespuestaProceso EjecutarProcesosDeliveryImportMTYBooking(DatosN4 datos);

        [OperationContract]
        RespuestaProceso EjecutarProcesosDeliveryImportP2D(DatosDeliveryImportP2D datos);//18
    }
}
