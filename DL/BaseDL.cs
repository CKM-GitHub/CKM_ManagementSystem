using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;

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
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();
            using var command = new SqlCommand(storedProcedureName, connection, transaction)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = _commandTimeout
            };

            if (parameters is { Length: > 0 })
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
        public async Task<int> ExecuteNonQueryAsync(string storedProcedureName, params SqlParameter[] parameters)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = CreateCommand(connection, storedProcedureName, parameters);

            if (parameters != null && parameters.Length > 0)
            {
                command.Parameters.AddRange(
                    NormalizeParameters(parameters));
            }

            await connection.OpenAsync();

            return await command.ExecuteNonQueryAsync();
        }
        public async Task<int> ExecuteNonQueryWithErrorCodeAsync(string storedProcedureName, params SqlParameter[] parameters)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = CreateCommand(connection, storedProcedureName, parameters);

            var errorParameter = command.Parameters
                .Cast<SqlParameter>()
                .FirstOrDefault(p =>
                    p.ParameterName.Equals("@ErrorCode", StringComparison.OrdinalIgnoreCase) ||
                    p.ParameterName.Equals("@Error_Code", StringComparison.OrdinalIgnoreCase));

            if (errorParameter == null)
            {
                errorParameter = new SqlParameter("@ErrorCode", SqlDbType.Int)
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
        public async Task<bool> ExecuteAsync(string storedProcedure, params SqlParameter[] parameters)
        {
            await ExecuteNonQueryAsync(storedProcedure, parameters);
            return true;
        }

        public int ExecuteScalar(string storedProcedureName, params SqlParameter[] parameters)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = CreateCommand(connection, storedProcedureName, parameters);

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

        public DataTable SelectDataTable(string storedProcedureName, params SqlParameter[] parameters)
        {
            var dataTable = new DataTable();

            using var connection = new SqlConnection(_connectionString);
            using var command = CreateCommand(connection, storedProcedureName, parameters);
            using var adapter = new SqlDataAdapter(command);

            adapter.Fill(dataTable);
            return dataTable;
        }

        public async Task<DataTable> SelectDataTableAsync(string storedProcedureName, params SqlParameter[]? parameters)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = CreateCommand(connection, storedProcedureName, parameters);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var table = new DataTable();
            using (var reader = await command.ExecuteReaderAsync())
            {
                table.Load(reader);
            }
                if (parameters != null)
                {
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        if (parameters[i].Direction == ParameterDirection.Output || parameters[i].Direction == ParameterDirection.InputOutput)
                        {
                            parameters[i].Value = command.Parameters[parameters[i].ParameterName].Value;
                        }
                    }
                }
            return table;
        }

        public DataSet SelectDataSet(string storedProcedureName, params SqlParameter[] parameters)
        {
            var dataSet = new DataSet();

            using var connection = new SqlConnection(_connectionString);
            using var command = CreateCommand(connection, storedProcedureName, parameters);
            using var adapter = new SqlDataAdapter(command);

            adapter.Fill(dataSet);
            return dataSet;
        }

        public async Task<string> SelectJsonAsync(string storedProcedureName, params SqlParameter[] parameters)
        {
            var table = await SelectDataTableAsync(storedProcedureName, parameters);

            var rows = table.Rows
                .Cast<DataRow>()
                .Select(row => table.Columns
                    .Cast<DataColumn>()
                    .ToDictionary(
                        col => col.ColumnName,
                        col => row[col] == DBNull.Value ? null : row[col]
                    ));

            return JsonSerializer.Serialize(rows);
        }
        public async Task<List<T>> ExecuteReaderAsync<T>(
            string storedProcedureName,
            Func<SqlDataReader, T> map,
            params SqlParameter[] parameters)
        {
            ArgumentNullException.ThrowIfNull(map);

            var results = new List<T>();

            using var connection = new SqlConnection(_connectionString);
            using var command = CreateCommand(
                connection,
                storedProcedureName,
                parameters);

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(map(reader));
            }

            return results;
        }      

        #region Helpers

        protected SqlParameter CreateParameter(string parameterName, object? value)
        {
            return new SqlParameter(parameterName, value ?? DBNull.Value);
        }

        private SqlCommand CreateCommand(SqlConnection connection, string storedProcedureName, SqlParameter[]? parameters)
        {
            var command = new SqlCommand(storedProcedureName, connection)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = _commandTimeout
            };

            if (parameters is { Length: > 0 })
            {
                command.Parameters.AddRange(NormalizeParameters(parameters));
            }
        public async Task<bool> ExecuteAsync(string storedProcedure, params SqlParameter[] parameters){
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand( storedProcedure, connection);

            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout= _commandTimeout;

            if(parameters != null && parameters.Length > 0)
            {
                command.Parameters.AddRange (NormalizeParameters(parameters));

            }
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            if(parameters != null)
            {
                for(int i = 0;i< parameters.Length; i++)
                {
                    if (parameters[i].Direction == ParameterDirection.Output || parameters[i].Direction == ParameterDirection.InputOutput)
                    {
                        parameters[i].Value = command.Parameters[parameters[i].ParameterName].Value;
                    }
                }
            }

            return true;
        }

        private  SqlParameter[] NormalizeParameters(SqlParameter[] parameters)
            return command;
        }
        private SqlParameter[] NormalizeParameters(SqlParameter[] parameters)
        {
            foreach (var parameter in parameters)
            {
                if (parameter.Value == null ||
                    parameter.Value == DBNull.Value ||
                    string.IsNullOrWhiteSpace(parameter.Value.ToString()))
                {
                    parameter.Value = DBNull.Value;
                }
            }

            return parameters;
        }

        public object? ExecuteScalarObject(string storedProcedureName, params SqlParameter[] parameters)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();
            using SqlCommand command = new SqlCommand(storedProcedureName, connection);
            command.CommandType = CommandType.StoredProcedure;
            if(parameters != null && parameters.Length > 0)
            {
                NormalizeParameters(parameters);
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
                NormalizeParameters(parameters);
                command.Parameters.AddRange(parameters);
            }
            using SqlDataAdapter adapter= new SqlDataAdapter(command);
            DataTable dataTable = new DataTable();

            adapter.Fill(dataTable);

            return dataTable;
        }

    }
}