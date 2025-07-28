using System.Linq;

namespace Console.ServicioConsole
{
    public partial class PRE_GATE
    {
        public string TipoTransaccion
        {
            get
            {
                return PRE_GATE_DETAILS.FirstOrDefault().TRANSACTION_TYPE.DESCRIPTION;
            }
        }

        public string Estado
        {
            get
            {
                switch (STATUS)
                {
                    case "N":
                        return "Nuevo";
                    case "C":
                        return "Cancelado";
                    case "I":
                        return "Ingresado";
                    case "P":
                        return "En Proceso";
                    case "L":
                        return "Saliendo";
                    case "O":
                        return "Completado";
                    default:
                        return "";
                };
            }
        }

        public string Zona
        {
            get
            {
                if (KIOSK_TRANSACTIONS == null || KIOSK_TRANSACTIONS.Count() == 0)
                    return DEVICE.ZONE.NAME;
                else
                    return KIOSK_TRANSACTIONS.FirstOrDefault().KIOSK.ZONE.NAME;
            }
        }
    }
}
