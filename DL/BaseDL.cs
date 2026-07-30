using System.Data;
using System.Linq;
using System.Text.Json;
using Microsoft.Data.SqlClient;

namespace DL
{
    public class BaseDL
    {
        protected readonly string _connectionString;
        protected readonly int _commandTimeout;

        public BaseDL(string connectionString, int connectionTimeout = 30)
        {
            _connectionString = connectionString;
            _commandTimeout = connectionTimeout;
        }

        public async Task<DataTable> SelectDataTableAsync(string storedProcedure, params SqlParameter[] parameters)
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

            return true;
        }


        private  SqlParameter[] NormalizeParameters(SqlParameter[] parameters)
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


    }
}
