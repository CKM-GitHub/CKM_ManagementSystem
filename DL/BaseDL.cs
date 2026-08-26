using CKM_ManagementSystem.Models;
using CKM_ManagementSystem.Models.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using System.Data;

namespace CKM_ManagementSystem.DL
{
    public class BaseDL
    {
        protected readonly string _connectionString;

        public BaseDL(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }
        /*
        protected async Task<int> ExecuteNonQuery(string spName, params SqlParameter[] parameters)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand(spName, conn);

            cmd.CommandType = CommandType.StoredProcedure;

            if (parameters != null && parameters.Length > 0)
            {
                cmd.Parameters.AddRange(parameters);
            }

            await conn.OpenAsync();

            return await cmd.ExecuteNonQueryAsync();
        } */

        protected SqlParameter CreateParameter(string parameterName, object? value)
        {
            return new SqlParameter(parameterName, value ?? DBNull.Value);
        }
        public string InsertUpdateDeleteData(string spName, SqlParameter[] parameters)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand(spName, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            if (parameters != null && parameters.Length > 0)
                cmd.Parameters.AddRange(parameters);

            conn.Open();
            cmd.ExecuteNonQuery();

            return "Success";
        }
        public int ExecuteScalar(string spName, SqlParameter[] parameters)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand(spName, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            if (parameters != null && parameters.Length > 0)
                cmd.Parameters.AddRange(parameters);

            conn.Open();
            object? result = cmd.ExecuteScalar();

            return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
        }
        public async Task<int> ExecuteNonQueryWithErrorCodeAsync(string spName, SqlParameter[] parameters)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand(spName, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            if(parameters != null && parameters.Length > 0)
            {
                cmd.Parameters.AddRange(parameters);    
            }

            SqlParameter? errorParam = parameters?.FirstOrDefault(p => p.ParameterName.Equals("@ErrorCode", StringComparison.OrdinalIgnoreCase));

            if (errorParam == null)
            {
                errorParam = new SqlParameter("@ErrorCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output,
                };
                cmd.Parameters.Add(errorParam);
            }
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return Convert.ToInt32(errorParam.Value);
        }
        public async Task<List<T>> ExecuteReaderAsync<T>(string spName, Func<SqlDataReader, T> map, params SqlParameter[] parameters)
        {
            var list = new List<T>();
            using SqlConnection conn = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand(spName, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            if (parameters != null && parameters.Length > 0)
                cmd.Parameters.AddRange(parameters);

            await conn.OpenAsync();

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(map(reader));
            }
            return list;
        }
    }
}