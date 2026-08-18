using System.Data;
using Microsoft.Data.SqlClient;

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
                    string.IsNullOrWhiteSpace(
                        parameter.Value.ToString()))
                {
                    parameter.Value = DBNull.Value;
                }
            }
        }
        public async Task<int> ExecuteNonQueryAsync(
        string storedProcedureName,
        params SqlParameter[] parameters)
        {
            using SqlConnection connection =
                new SqlConnection(_connectionString);

            await connection.OpenAsync();

            using SqlCommand command =
                new SqlCommand(storedProcedureName, connection);

            command.CommandType = CommandType.StoredProcedure;

            if (parameters != null && parameters.Length > 0)
            {
                ChangeToDBNull(parameters);
                command.Parameters.AddRange(parameters);
            }
            return await command.ExecuteNonQueryAsync();
        }
        public async Task<string?> ExecuteScalarAsync(string storedProcedureName,params SqlParameter[] parameters)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);

            await conn.OpenAsync();

            using SqlCommand command = new SqlCommand(storedProcedureName, conn);

            command.CommandType = CommandType.StoredProcedure;

            if(parameters != null && parameters.Length > 0)
            {
                ChangeToDBNull(parameters);
                command.Parameters.AddRange(parameters);
            }
            object? result = await command.ExecuteScalarAsync();

            return result == null || result == DBNull.Value ? null : result.ToString();
        }
    }
}