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

        private static CommandType GetCommandType(string commandText)
        {
            return commandText.Trim().Contains(" ") ? CommandType.Text : CommandType.StoredProcedure;
        }

        public string InsertUpdateDeleteData(
            string commandText,
            params SqlParameter[] parameters)
        {
            using SqlConnection connection =
                new SqlConnection(_connectionString);

            connection.Open();

            using SqlTransaction transaction =
                connection.BeginTransaction();

            using SqlCommand command = new SqlCommand(
                commandText,
                connection,
                transaction);

            command.CommandType = GetCommandType(commandText);

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
            string commandText,
            params SqlParameter[] parameters)
        {
            using SqlConnection connection =
                new SqlConnection(_connectionString);

            connection.Open();

            using SqlCommand command =
                new SqlCommand(commandText, connection);

            command.CommandType = GetCommandType(commandText);

            if (parameters != null && parameters.Length > 0)
            {
                ChangeToDBNull(parameters);
                command.Parameters.AddRange(parameters);
            }

            object? result = command.ExecuteScalar();

            return Convert.ToInt32(result);
        }

        public DataTable SelectData(
            string commandText,
            params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand(commandText, connection);

            command.CommandType = GetCommandType(commandText);

            if (parameters != null && parameters.Length > 0)
            {
                ChangeToDBNull(parameters);
                command.Parameters.AddRange(parameters);
            }

            using SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dt);

            return dt;
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
    }
}