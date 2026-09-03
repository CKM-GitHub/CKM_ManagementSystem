using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;

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
        public string InsertUpdateDeleteData(
            string storedProcedureName,
            params SqlParameter[] parameters)
        {
            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand(storedProcedureName, connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                };

            if (parameters != null && parameters.Length > 0)
            {
                command.Parameters.AddRange(
                    NormalizeParameters(parameters));
            }

            connection.Open();
            command.ExecuteNonQuery();

            return "Success";
        }
        public async Task<int> ExecuteNonQueryAsync(
            string storedProcedureName,
            params SqlParameter[] parameters)
        {
            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand(storedProcedureName, connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                };

            if (parameters != null && parameters.Length > 0)
            {
                command.Parameters.AddRange(
                    NormalizeParameters(parameters));
            }

            await connection.OpenAsync();

            return await command.ExecuteNonQueryAsync();
        }
        public async Task<int> ExecuteNonQueryWithErrorCodeAsync(
            string storedProcedureName,
            params SqlParameter[] parameters)
        {
            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand(storedProcedureName, connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                };

            if (parameters != null && parameters.Length > 0)
            {
                command.Parameters.AddRange(
                    NormalizeParameters(parameters));
            }

            SqlParameter? errorParameter =
                command.Parameters
                    .Cast<SqlParameter>()
                    .FirstOrDefault(p =>
                        p.ParameterName.Equals(
                            "@ErrorCode",
                            StringComparison.OrdinalIgnoreCase)
                        ||
                        p.ParameterName.Equals(
                            "@Error_Code",
                            StringComparison.OrdinalIgnoreCase));

            if (errorParameter == null)
            {
                errorParameter = new SqlParameter(
                    "@ErrorCode",
                    SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                command.Parameters.Add(errorParameter);
            }

            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();

            return errorParameter.Value == DBNull.Value
                ? 0
                : Convert.ToInt32(errorParameter.Value);
        }
        public int ExecuteScalar(
            string storedProcedureName,
            params SqlParameter[] parameters)
        {
            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand(storedProcedureName, connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                };

            if (parameters != null && parameters.Length > 0)
            {
                command.Parameters.AddRange(
                    NormalizeParameters(parameters));
            }

            connection.Open();

            object? result = command.ExecuteScalar();

            return result == null || result == DBNull.Value
                ? 0
                : Convert.ToInt32(result);
        }
        public DataTable SelectDataTable(
            string storedProcedureName,
            params SqlParameter[] parameters)
        {
            DataTable dataTable = new DataTable();

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand(storedProcedureName, connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                };

            if (parameters != null && parameters.Length > 0)
            {
                command.Parameters.AddRange(
                    NormalizeParameters(parameters));
            }

            using SqlDataAdapter adapter =
                new SqlDataAdapter(command);

            adapter.Fill(dataTable);

            return dataTable;
        }
        public async Task<DataTable> SelectDataTableAsync(
            string storedProcedureName,
            params SqlParameter[] parameters)
        {
            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand(storedProcedureName, connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                };

            if (parameters != null && parameters.Length > 0)
            {
                command.Parameters.AddRange(
                    NormalizeParameters(parameters));
            }

            await connection.OpenAsync();

            using SqlDataReader reader =
                await command.ExecuteReaderAsync();

            DataTable dataTable = new DataTable();

            dataTable.Load(reader);

            return dataTable;
        }
        public DataSet SelectDataSet(
            string storedProcedureName,
            params SqlParameter[] parameters)
        {
            DataSet dataSet = new DataSet();

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand(storedProcedureName, connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                };

            if (parameters != null && parameters.Length > 0)
            {
                command.Parameters.AddRange(
                    NormalizeParameters(parameters));
            }

            using SqlDataAdapter adapter =
                new SqlDataAdapter(command);

            adapter.Fill(dataSet);

            return dataSet;
        }
        public async Task<string> SelectJsonAsync(
            string storedProcedureName,
            params SqlParameter[] parameters)
        {
            DataTable table =
                await SelectDataTableAsync(
                    storedProcedureName,
                    parameters);

            var rows = table.Rows
                .Cast<DataRow>()
                .Select(row =>
                    table.Columns
                        .Cast<DataColumn>()
                        .ToDictionary(
                            column => column.ColumnName,
                            column => row[column] == DBNull.Value
                                ? null
                                : row[column]
                        ));

            return JsonSerializer.Serialize(rows);
        }
        protected SqlParameter CreateParameter(
            string parameterName,
            object? value)
        {
            return new SqlParameter(
                parameterName,
                value ?? DBNull.Value);
        }
        private SqlParameter[] NormalizeParameters(
            SqlParameter[] parameters)
        {
            foreach (SqlParameter parameter in parameters)
            {
                if (parameter.Value == null ||
                    parameter.Value == DBNull.Value ||
                    string.IsNullOrWhiteSpace(
                        parameter.Value.ToString()))
                {
                    parameter.Value = DBNull.Value;
                }
            }
            return parameters;
        }
    }
}