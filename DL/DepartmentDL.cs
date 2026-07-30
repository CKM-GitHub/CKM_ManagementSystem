using CKM_ManagementSystem.Models.Entities;
using Microsoft.Data.SqlClient;

namespace CKM_ManagementSystem.DL
{
    public class DepartmentDL
    {
        private readonly BaseDL bdl;

        public DepartmentDL(BaseDL baseDL)
        {
            bdl = baseDL;
        }

        public string Department_Insert(Department department)
        {
            SqlParameter[] sqlprms = new SqlParameter[5];

            sqlprms[0] = new SqlParameter(
                "@Department_Code",
                department.DepartmentCode);

            sqlprms[1] = new SqlParameter(
                "@Department_Name",
                department.DepartmentName);

            sqlprms[2] = new SqlParameter(
                "@Manager_User_Id",
                department.ManagerUserId);

            sqlprms[3] = new SqlParameter(
                "@Description",
                department.Description);

            sqlprms[4] = new SqlParameter(
                "@Status",
                department.Status);

            return bdl.InsertUpdateDeleteData(
                "sp_Department_Insert",
                sqlprms);
        }

        public bool IsDepartmentCodeDuplicate(string departmentCode)
        {
            SqlParameter[] sqlprms = new SqlParameter[1];

            sqlprms[0] = new SqlParameter(
                "@Department_Code",
                departmentCode);

            int count = bdl.ExecuteScalar(
                "sp_CheckDuplicateDepartmentCode",
                sqlprms);

            return count > 0;
        }

        public bool IsDepartmentNameDuplicate(string departmentName)
        {
            SqlParameter[] sqlprms = new SqlParameter[1];

            sqlprms[0] = new SqlParameter(
                "@Department_Name",
                departmentName);

            int count = bdl.ExecuteScalar(
                "sp_CheckDuplicateDepartmentName",
                sqlprms);

            return count > 0;
        }
    }
}