using Microsoft.Data.SqlClient;
using CKM_ManagementSystem.DL;
using Microsoft.AspNetCore.Identity;
using System.Data;
using CKM_ManagementSystem.Models;
using CKM_ManagementSystem.Models.ViewModels.LoginUser;

namespace CKM_ManagementSystem.BL
{
    public class LoginUserBL
    {
        private readonly BaseDL bdl;
        private readonly PasswordHasher<User> _passwordHasher = new();
        public LoginUserBL (BaseDL baseDL)
        {
            bdl = baseDL;
        }
        public enum LoginStatus
        {
            Success = 0,
            UserNotFound = 1,
            AccountDisabled = 2,
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
            // try catch
            try
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
                
                    errorCodeParam
                };

                DataTable table = bdl.ExecuteDataTable("sp_LoginUser", parameters);
                int errorCode = errorCodeParam.Value != DBNull.Value ? Convert.ToInt32(errorCodeParam.Value) : -1;
                LoginStatus status = (LoginStatus)errorCode;

                if (status == LoginStatus.UserNotFound)
                {
                    return new LoginResult { IsSuccess = false, Message = "No account found with this email addreess." };
                };
                if (status == LoginStatus.AccountDisabled)
                {
                    return new LoginResult { IsSuccess = false, Message = "Your account has been disabled." };
                };
                if (status != LoginStatus.Success || table.Rows.Count == 0)
                {
                    return new LoginResult { IsSuccess = false, Message = "An unexpected system error occurred." };
                };
                
                DataRow row = table.Rows[0];
                
                string staffCode = row["Staff_Code"]?.ToString() ?? string.Empty;
                string passwordHash = row["Password"]?.ToString() ?? string.Empty;

                PasswordVerificationResult verificationResult = _passwordHasher.VerifyHashedPassword(
                                                                 new User
                                                                 {
                                                                     StaffCode = staffCode,
                                                                     Email = model.Email ?? string.Empty
                                                                 },
                                                                 passwordHash,
                                                                 model.Password ?? string.Empty
                                                                );
                if(verificationResult == PasswordVerificationResult.Failed)
                {
                    return new LoginResult
                    {
                        IsSuccess = false,
                        Message = "Invalid password."
                    };
                }
                return new LoginResult
                {
                    IsSuccess = true,
                    Message = "Login Successfully",
                    UserEmail = model.Email ?? string.Empty,
                    Staff_Code = staffCode
                };
            }
            catch
            {
                return new LoginResult()
                {
                    IsSuccess = false,
                    Message = "An unexpected system error occurred."
                };
            }
           
        }
    }
}
