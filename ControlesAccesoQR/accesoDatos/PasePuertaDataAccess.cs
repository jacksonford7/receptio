using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ControlesAccesoQR.accesoDatos
{
    public class PasePuertaInfo
    {
        public string ChoferID { get; set; }
        public string EmpresaTransporteID { get; set; }
        public string ChoferNombre { get; set; }
        public string EmpresaNombre { get; set; }
        public string Patente { get; set; }
    }

    public class PasePuertaDataAccess
    {
        private readonly string _connectionString;
        private readonly string _extendedConnectionString;

        public PasePuertaDataAccess()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["midle"].ConnectionString;
            _extendedConnectionString = ConfigurationManager.ConnectionStrings["bill"].ConnectionString;
        }

        public PasePuertaInfo ObtenerChoferEmpresaPorPase(string numeroPase)
        {
            PasePuertaInfo info = null;
            string choferId = null;
            string empresaId = null;

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
                        choferId = reader["ChoferID"].ToString();
                        empresaId = reader["EmpresaTransporteID"].ToString();

                        info = new PasePuertaInfo
                        {
                            ChoferID = choferId,
                            EmpresaTransporteID = empresaId,
                           
                        };
                    }
                }
            }

            if (info == null)
                return null;

            using (var connection = new SqlConnection(_extendedConnectionString))
            using (var command = new SqlCommand("[Bill].[compania_lista]", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@pista", empresaId);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        info.EmpresaNombre = reader["razon_social"].ToString();
                    }
                }
            }

            using (var connection = new SqlConnection(_extendedConnectionString))
            using (var command = new SqlCommand("[Bill].[choferes_empresa_lista]", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@empresa_id", empresaId);
                command.Parameters.AddWithValue("@pista", choferId);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        info.ChoferNombre = reader["nombres"].ToString();
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
            public string ChoferNombre { get; set; }
            public string EmpresaNombre { get; set; }
            public string Patente { get; set; }
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

            if (result != null)
            {
                var info = ObtenerChoferEmpresaPorPase(numeroPase);
                if (info != null)
                {
                    result.ChoferNombre = info.ChoferNombre;
                    result.EmpresaNombre = info.EmpresaNombre;
                    result.Patente = info.Patente;
                }
            }

            return result;
        }
    }
}
