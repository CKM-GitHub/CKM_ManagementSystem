using System.Data;
using System.Linq;
using System.Text.Json;
using Microsoft.Data.SqlClient;

namespace CKM_ManagementSystem.DL
{
    public class BaseDL
    {
        protected readonly string _connectionString;
        protected readonly int _commandTimeout;

        public BaseDL(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "DefaultConnection was not found.");
            _commandTimeout = 30;
        }

        public string InsertUpdateDeleteData(string storedProcedureName, params SqlParameter[] parameters)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            connection.Open();

            using SqlTransaction transaction = connection.BeginTransaction();

            using SqlCommand command = new SqlCommand(storedProcedureName, connection, transaction);

            command.CommandType = CommandType.StoredProcedure;

            if (parameters != null && parameters.Length > 0)
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

        public async Task<bool> ExecuteAsync(string storedProcedure, params SqlParameter[] parameters)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(storedProcedure, connection);

            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = _commandTimeout;

            if (parameters != null && parameters.Length > 0)
            {
                command.Parameters.AddRange(NormalizeParameters(parameters));

            }
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return true;
        }

        public int ExecuteScalar(string storedProcedureName, params SqlParameter[] parameters)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            using SqlCommand command = new SqlCommand(storedProcedureName, connection);

            command.CommandType = CommandType.StoredProcedure;

            if (parameters != null && parameters.Length > 0)
            {
                command.Parameters.AddRange(NormalizeParameters(parameters));
            }

            object? result = command.ExecuteScalar();

            return Convert.ToInt32(result);
        }

        public async Task<DataTable> SelectDataTableAsync(string storedProcedure, params SqlParameter[]? parameters)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(storedProcedure, connection);

            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = _commandTimeout;

            if (parameters != null && parameters.Length > 0)
            {
                command.Parameters.AddRange(NormalizeParameters(parameters));
            }
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            var table = new DataTable();
            table.Load(reader);
            return table;
        }
        public async Task<string> SelectJsonAsync(string storedProcedure, params SqlParameter[] parameters)
        {
            var table = await SelectDataTableAsync(storedProcedure, parameters);

            var rows = table.Rows.Cast<DataRow>()
                .Select(row => table.Columns.Cast<DataColumn>()
                    .ToDictionary(
                        col => col.ColumnName,
                        col => row[col] == DBNull.Value ? null : row[col]
                    ));
            return JsonSerializer.Serialize(rows);
        }

        private SqlParameter[] NormalizeParameters(SqlParameter[] parameters)
        {
            foreach (var parameter in parameters)
            {
                if (parameter.Value == null || string.IsNullOrWhiteSpace(parameter.Value.ToString()))
                {
                    parameter.Value = DBNull.Value;
                }
            }
            return parameters;
        }

        public DataTable SelectDataTable(
            string storedProcedureName,
            params SqlParameter[] parameters)
        {
            DataTable dataTable = new DataTable();

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand(storedProcedureName, connection);

            command.CommandType = CommandType.StoredProcedure;

            if (parameters != null && parameters.Length > 0)
            {
                command.Parameters.AddRange(
                 NormalizeParameters(parameters)
                );
            }

            using SqlDataAdapter adapter =
                new SqlDataAdapter(command);

            adapter.Fill(dataTable);

            return dataTable;
        }


    }
}