using CKM_ManagementSystem.Models.Entities;
using CKM_ManagementSystem.DL;
using Microsoft.Data.SqlClient;

namespace CKM_ManagementSystem.BL
{
    public class DepartmentBL
    {
        private readonly BaseDL bdl;

        public DepartmentBL(BaseDL baseDL)
        {
            bdl = baseDL;
        }

        public string Department_Insert(Department department)
        {
            SqlParameter[] sqlprms =
            {
                new SqlParameter("@Department_Code", department.DepartmentCode),
                new SqlParameter("@Department_Name", department.DepartmentName),
                new SqlParameter("@Manager_User_Id", department.ManagerUserId),
                new SqlParameter("@Description", department.Description),
                new SqlParameter("@Status", department.Status)
            };

            return bdl.InsertUpdateDeleteData(
                "sp_Department_Insert",
                sqlprms);
        }

        public bool IsDepartmentCodeDuplicate(string departmentCode)
        {
            SqlParameter[] sqlprms =
            {
                new SqlParameter("@Department_Code", departmentCode)
            };

            int count = bdl.ExecuteScalar(
                "sp_CheckDuplicateDepartmentCode",
                sqlprms);

            return count > 0;
        }

        public bool IsDepartmentNameDuplicate(string departmentName)
        {
            SqlParameter[] sqlprms =
            {
                new SqlParameter("@Department_Name", departmentName)
            };

            int count = bdl.ExecuteScalar(
                "sp_CheckDuplicateDepartmentName",
                sqlprms);

            return count > 0;
        }
    }
}