using Microsoft.Data.SqlClient;
using CKM_ManagementSystem.Models;
using System.Data;
using Microsoft.Extensions.Configuration;
using CKM_ManagementSystem.Models.ViewModels;

public class BaseDL
{
    protected readonly string _connectionString;
   
    public BaseDL(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }
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
    }
    protected SqlParameter CreateParameter(string ParameterName, object? value)
    {
        return new SqlParameter(ParameterName, value ?? DBNull.Value);
    }   
}
