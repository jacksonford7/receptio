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
                            Nombre = reader["Chofer"].ToString(),
                            Empresa = reader["Empresa"].ToString(),
                            Patente = reader["Patente"].ToString()
                        };
                    }
                }
            }
            return info;
        }
    }
}
