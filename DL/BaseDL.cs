using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CKM_ManagementSystem.DL
{
    public class BaseDL
    {
        private readonly string _connectionString;

        public BaseDL(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "DefaultConnection was not found.");
        }

        public string InsertUpdateDeleteData(
            string storedProcedureName,
            params SqlParameter[] parameters)
        {
            using SqlConnection connection =
                new SqlConnection(_connectionString);

            connection.Open();

            using SqlTransaction transaction =
                connection.BeginTransaction();

            using SqlCommand command = new SqlCommand(
                storedProcedureName,
                connection,
                transaction);

            command.CommandType = CommandType.StoredProcedure;

            if (parameters != null && parameters.Length > 0)
            {
                ChangeToDBNull(parameters);
                command.Parameters.AddRange(parameters);
            }

            try
            {
                command.ExecuteNonQuery();
                transaction.Commit();

                return "true";
            }
            catch
            {
                transaction.Rollback();

                return "false";
            }
        }

        public int ExecuteScalar(
            string storedProcedureName,
            params SqlParameter[] parameters)
        {
            using SqlConnection connection =
                new SqlConnection(_connectionString);

            connection.Open();

            using SqlCommand command =
                new SqlCommand(storedProcedureName, connection);

            command.CommandType = CommandType.StoredProcedure;

            if (parameters != null && parameters.Length > 0)
            {
                ChangeToDBNull(parameters);
                command.Parameters.AddRange(parameters);
            }

            object? result = command.ExecuteScalar();

            return Convert.ToInt32(result);
        }

        private static void ChangeToDBNull(
            SqlParameter[] parameters)
        {
            foreach (SqlParameter parameter in parameters)
            {
                if (parameter.Value == null ||
                    string.IsNullOrWhiteSpace(parameter.Value.ToString()))
                {
                    parameter.Value = DBNull.Value;
                }
            }
        }

        public object? ExecuteScalarObject(string storedProcedureName, params SqlParameter[] parameters)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();
            using SqlCommand command = new SqlCommand(storedProcedureName, connection);
            command.CommandType = CommandType.StoredProcedure;
            if(parameters != null && parameters.Length > 0)
            {
                ChangeToDBNull(parameters);
                command.Parameters.AddRange(parameters);
            }
            object? result = command.ExecuteScalar();
            return (result == DBNull.Value) ? null : result;
        }

        public DataTable ExecuteDataTable(string storedProcedure, params SqlParameter[] parameters)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(storedProcedure, connection);

            command.CommandType= CommandType.StoredProcedure;
            

            if(parameters != null && parameters.Length > 0)
            {
                ChangeToDBNull(parameters);
                command.Parameters.AddRange(parameters);
            }
            using SqlDataAdapter adapter= new SqlDataAdapter(command);
            DataTable dataTable = new DataTable();

            adapter.Fill(dataTable);

            return dataTable;
        }

    }
}