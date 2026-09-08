using System.Data;
using Microsoft.Data.SqlClient;

namespace CKM_ManagementSystem.DL
{
    public class BaseDL
    {
        private readonly string _connectionString;
        private readonly int _commandTimeout;

        public BaseDL(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "DefaultConnection was not found.");

            _commandTimeout = 30;
        }

        public string InsertUpdateDeleteData(
            string storedProcedureName,
            params SqlParameter[] parameters)
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlTransaction transaction = connection.BeginTransaction();
            using SqlCommand command = new(
                storedProcedureName,
                connection,
                transaction);

            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = _commandTimeout;

            if (parameters.Length > 0)
            {
                command.Parameters.AddRange(NormalizeParameters(parameters));
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

        public async Task<bool> ExecuteAsync(
            string storedProcedure,
            params SqlParameter[] parameters)
        {
            await using SqlConnection connection = new(_connectionString);
            await using SqlCommand command = new(
                storedProcedure,
                connection);

            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = _commandTimeout;

            if (parameters.Length > 0)
            {
                command.Parameters.AddRange(NormalizeParameters(parameters));
            }

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return true;
        }

        public int ExecuteScalar(
            string storedProcedureName,
            params SqlParameter[] parameters)
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand command = new(
                storedProcedureName,
                connection);

            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = _commandTimeout;

            if (parameters.Length > 0)
            {
                command.Parameters.AddRange(NormalizeParameters(parameters));
            }

            object? result = command.ExecuteScalar();

            return result is null || result == DBNull.Value
                ? 0
                : Convert.ToInt32(result);
        }

        public async Task<int> ExecuteNonQueryWithErrorCodeAsync(
            string storedProcedureName,
            params SqlParameter[] parameters)
        {
            await using SqlConnection connection = new(_connectionString);
            await using SqlCommand command = new(
                storedProcedureName,
                connection);

            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = _commandTimeout;

            if (parameters.Length > 0)
            {
                command.Parameters.AddRange(NormalizeParameters(parameters));
            }

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            foreach (SqlParameter parameter in parameters)
            {
                if (string.Equals(
                    parameter.ParameterName,
                    "@ErrorCode",
                    StringComparison.OrdinalIgnoreCase))
                {
                    if (parameter.Value is not null &&
                        parameter.Value != DBNull.Value)
                    {
                        return Convert.ToInt32(parameter.Value);
                    }

                    return 0;
                }
            }
            return 0;
        }

        public async Task<DataTable> SelectDataTableAsync(
            string storedProcedure,
            params SqlParameter[] parameters)
        {
            await using SqlConnection connection = new(_connectionString);
            await using SqlCommand command = new(
                storedProcedure,
                connection);

            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = _commandTimeout;

            if (parameters.Length > 0)
            {
                command.Parameters.AddRange(NormalizeParameters(parameters));
            }

            await connection.OpenAsync();

            await using SqlDataReader reader = await command.ExecuteReaderAsync();

            DataTable table = new();
            table.Load(reader);

            return table;
        }

        public async Task<List<T>> ExecuteReaderAsync<T>(
            string storedProcedure,
            Func<SqlDataReader, T> map,
            params SqlParameter[] parameters)
        {
            List<T> results = new();

            await using SqlConnection connection = new(_connectionString);
            await using SqlCommand command = new(
                storedProcedure,
                connection);

            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = _commandTimeout;

            if (parameters.Length > 0)
            {
                command.Parameters.AddRange(NormalizeParameters(parameters));
            }

            await connection.OpenAsync();

            await using SqlDataReader reader =
                await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(map(reader));
            }

            return results;
        }

        private static SqlParameter[] NormalizeParameters(
            SqlParameter[] parameters)
        {
            foreach (SqlParameter parameter in parameters)
            {
                if (parameter.Value is null ||
                    string.IsNullOrWhiteSpace(parameter.Value.ToString()))
                {
                    parameter.Value = DBNull.Value;
                }
            }

            return parameters;
        }
    }
}