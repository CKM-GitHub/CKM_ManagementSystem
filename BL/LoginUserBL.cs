using Microsoft.Data.SqlClient;
using CKM_ManagementSystem.DL;
using CKM_ManagementSystem.Models.ViewModels.LoginUser;

namespace CKM_ManagementSystem.BL
{
    public class LoginUserBL
    {
        private readonly BaseDL bdl;
        public LoginUserBL (BaseDL baseDL)
        {
            bdl = baseDL;
        }
        public enum LoginStatus
        {
            Success = 0,
            UserNotFound = 1,
            AccountDisabled = 2,
            InvalidPassword = 3
        }
        public class LoginResult
        {
            public bool IsSuccess { get; set; }
            public string Message { get; set; } = string.Empty;
            public string UserEmail { get; set; } = string.Empty;
            public string? Staff_Code { get; set; }
        }
        public LoginResult LoginUser_select(LoginRequest model) 
        {
            var errorCodeParam = new SqlParameter("@ErrorCode", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Email", System.Data.SqlDbType.VarChar) 
                {
                    Value = model.Email ?? (object)DBNull.Value
                },
                new SqlParameter("@Password", System.Data.SqlDbType.VarChar) 
                {
                    Value = model.Password ?? (object)DBNull.Value
                },
                errorCodeParam
            };
            object? scalarResult = bdl.ExecuteScalarObject("sp_LoginUser", parameters);
            int errorCode = errorCodeParam.Value != DBNull.Value ? Convert.ToInt32(errorCodeParam.Value) : -1;
            LoginStatus status = (LoginStatus)errorCode;
            string? staffCode = scalarResult?.ToString();

            return status switch
            {
                LoginStatus.Success => new LoginResult
                {
                    IsSuccess = true,
                    Message = "Login Successfully",
                    UserEmail = model.Email ?? string.Empty,
                    Staff_Code = staffCode
                },
                LoginStatus.UserNotFound => new LoginResult
                {
                    IsSuccess = false,
                    Message = "No account found with this email address."
                },
                LoginStatus.AccountDisabled => new LoginResult
                {
                    IsSuccess = false,
                    Message = "Your account has been disabled."
                },
                LoginStatus.InvalidPassword => new LoginResult
                {
                    IsSuccess = false,
                    Message = "Invalid password."
                },
                _ => new LoginResult{
                    IsSuccess = false,
                    Message = "An unexpected system error occurred."
                }
            };
        }
    }
}
