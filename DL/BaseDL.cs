using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

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
            catch (Exception ex)
            {
                transaction.Rollback();

                return "Error: " + ex.Message;
            }
        }

        public int ExecuteScalar(
            string storedProcedureName,
            params SqlParameter[] parameters)
        {
            object? value = ExecuteScalarObject(storedProcedureName, parameters);
            return value == null || value == DBNull.Value ? 0 : Convert.ToInt32(value);
        }

        public object? ExecuteScalarObject(
            string storedProcedureName,
            params SqlParameter[] parameters)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            using SqlCommand command = CreateCommand(connection, storedProcedureName, parameters);
            object? result = command.ExecuteScalar();
            return result == DBNull.Value ? null : result;
        }

        public DataTable SelectData(
            string storedProcedureName,
            params SqlParameter[] parameters)
            => ExecuteDataTable(storedProcedureName, parameters);

        public DataTable SelectDataTable(
            string storedProcedureName,
            params SqlParameter[] parameters)
            => ExecuteDataTable(storedProcedureName, parameters);

        public DataTable ExecuteDataTable(
            string storedProcedureName,
            params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();

            using SqlConnection connection =
                new SqlConnection(_connectionString);
            connection.Open();

            using SqlCommand command =
                CreateCommand(connection, storedProcedureName, parameters);

            using SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dt);

            return dt;
        }

        public DataSet SelectDataSet(
            string storedProcedureName,
            params SqlParameter[] parameters)
        {
            DataSet dataSet = new DataSet();

            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();
            using SqlCommand command = CreateCommand(connection, storedProcedureName, parameters);
            using SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dataSet);

            return dataSet;
        }

        public async Task<DataTable> SelectDataTableAsync(
            string storedProcedureName,
            params SqlParameter[] parameters)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using SqlCommand command = CreateCommand(connection, storedProcedureName, parameters);
            using SqlDataReader reader = await command.ExecuteReaderAsync();

            DataTable table = new DataTable();
            table.Load(reader);
            return table;
        }

        public async Task<int> ExecuteNonQueryWithErrorCodeAsync(
            string storedProcedureName,
            params SqlParameter[] parameters)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using SqlCommand command = CreateCommand(connection, storedProcedureName, parameters);
            await command.ExecuteNonQueryAsync();

            SqlParameter? errorParameter = command.Parameters.Cast<SqlParameter>()
                .FirstOrDefault(parameter =>
                    string.Equals(parameter.ParameterName, "@ErrorCode", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(parameter.ParameterName, "@Error_Code", StringComparison.OrdinalIgnoreCase));

            return errorParameter?.Value == null || errorParameter.Value == DBNull.Value
                ? 0
                : Convert.ToInt32(errorParameter.Value);
        }

        public async Task<int> ExecuteAsync(
            string storedProcedureName,
            params SqlParameter[] parameters)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using SqlCommand command = CreateCommand(connection, storedProcedureName, parameters);
            return await command.ExecuteNonQueryAsync();
        }

        public async Task<List<T>> ExecuteReaderAsync<T>(
            string storedProcedureName,
            Func<SqlDataReader, T> map,
            params SqlParameter[] parameters)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using SqlCommand command = CreateCommand(connection, storedProcedureName, parameters);
            using SqlDataReader reader = await command.ExecuteReaderAsync();

            var results = new List<T>();
            while (await reader.ReadAsync())
            {
                results.Add(map(reader));
            }

            return results;
        }

        private static SqlCommand CreateCommand(
            SqlConnection connection,
            string storedProcedureName,
            SqlParameter[]? parameters)
        {
            var command = new SqlCommand(storedProcedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null && parameters.Length > 0)
            {
                ChangeToDBNull(parameters);
                command.Parameters.AddRange(parameters);
            }

            return command;
        }

        private static void ChangeToDBNull(SqlParameter[] parameters)
        {
            foreach (SqlParameter parameter in parameters)
            {
                if (parameter.SqlDbType == SqlDbType.Structured)
                {
                    continue;
                }

                if (parameter.Value == null ||
                    (parameter.Value is string strValue && string.IsNullOrWhiteSpace(strValue)))
                {
                    parameter.Value = DBNull.Value;
                }
            }
        }
    }
}