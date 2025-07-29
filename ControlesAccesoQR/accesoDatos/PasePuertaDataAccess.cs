using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ControlesAccesoQR.accesoDatos
{
    public class PasePuertaInfo
    {
        public string Nombre { get; set; }
        public string Empresa { get; set; }
        public string Patente { get; set; }
    }

    public class PasePuertaDataAccess
    {
        private readonly string _connectionString;

        public PasePuertaDataAccess()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["midle"].ConnectionString;
        }

        public PasePuertaInfo ObtenerChoferEmpresaPorPase(string numeroPase)
        {
            PasePuertaInfo info = null;
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("[vhs].[obtener_chofer_empresa_por_pase]", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@NumeroPase", numeroPase);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        info = new PasePuertaInfo
                        {
                            Nombre = reader["ChoferID"].ToString(),
                            Empresa = reader["EmpresaTransporteID"].ToString(),
                           
                        };
                    }
                }
            }
            return info;
        }

        public class ActualizarFechaLlegadaResult
        {
            public int PasePuertaID { get; set; }
            public DateTime FechaHoraLlegada { get; set; }
        }

        public ActualizarFechaLlegadaResult ActualizarFechaLlegada(string numeroPase)
        {
            ActualizarFechaLlegadaResult result = null;
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("[vhs].[actualizar_fecha_llegada]", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@NumeroPase", numeroPase);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result = new ActualizarFechaLlegadaResult
                        {
                            PasePuertaID = Convert.ToInt32(reader["PasePuertaID"]),
                            FechaHoraLlegada = Convert.ToDateTime(reader["FechaHoraLlegada"])
                        };
                    }
                }
            }
            return result;
        }

        public class ActualizarFechaSalidaResult
        {
            public int PasePuertaID { get; set; }
            public DateTime FechaHoraSalida { get; set; }
        }

        public ActualizarFechaSalidaResult ActualizarFechaSalida(string numeroPase)
        {
            ActualizarFechaSalidaResult result = null;
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("[vhs].[actualizar_fecha_salida]", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@NumeroPase", numeroPase);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result = new ActualizarFechaSalidaResult
                        {
                            PasePuertaID = Convert.ToInt32(reader["PasePuertaID"]),
                            FechaHoraSalida = Convert.ToDateTime(reader["FechaHoraSalida"])
                        };
                    }
                }
            }
            return result;
        }
    }
}
