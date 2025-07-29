using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.EntityClient;

namespace ControlesAccesoQR.accesoDatos
{
    public class PasePuertaInfo
    {
        public string Nombre { get; set; }
        public string Empresa { get; set; }
        public string Patente { get; set; }

        public string ChoferNombre { get; set; }
        public string EmpresaNombre { get; set; }
    }

    public class PasePuertaDataAccess
    {
        private readonly string _connectionString;
        private readonly string _connectionStringExtended;

        public PasePuertaDataAccess()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["midle"].ConnectionString;
            _connectionStringExtended = GetExtendedConnection();
        }

        private string GetExtendedConnection()
        {
            var cnn = ConfigurationManager.ConnectionStrings["n4catalog"]?.ConnectionString;
            if (string.IsNullOrEmpty(cnn))
            {
                var entity = ConfigurationManager.ConnectionStrings["ModeloReceptioContainer"]?.ConnectionString;
                if (!string.IsNullOrEmpty(entity))
                {
                    var builder = new EntityConnectionStringBuilder(entity);
                    cnn = builder.ProviderConnectionString;
                }
            }
            return cnn;
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

            if (info != null && !string.IsNullOrEmpty(_connectionStringExtended))
            {
                using (var connection = new SqlConnection(_connectionStringExtended))
                {
                    connection.Open();

                    using (var command = new SqlCommand("[Bill].[compania_lista]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@EmpresaTransporteID", info.Empresa);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                info.EmpresaNombre = reader["razon_social"].ToString();
                            }
                        }
                    }

                    using (var command = new SqlCommand("[Bill].[choferes_empresa_lista]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@EmpresaTransporteID", info.Empresa);
                        command.Parameters.AddWithValue("@ChoferID", info.Nombre);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                info.ChoferNombre = reader["nombres"].ToString();
                            }
                        }
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
