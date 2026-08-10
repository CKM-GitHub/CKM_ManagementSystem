using System.Data;
using CKM_ManagementSystem.DL;
using CKM_ManagementSystem.Models.Entities;
using CKM_ManagementSystem.Models.ViewModels.Departments;
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
                new SqlParameter(
                    "@Department_Code",
                    department.DepartmentCode),

                new SqlParameter(
                    "@Department_Name",
                    department.DepartmentName),

                new SqlParameter(
                    "@Description",
                    department.Description),

                new SqlParameter(
                    "@Status",
                    department.Status)
            };

            return bdl.InsertUpdateDeleteData(
                "sp_Department_Insert",
                sqlprms);
        }

        public bool IsDepartmentCodeDuplicate(
            string departmentCode)
        {
            SqlParameter[] sqlprms =
            {
                new SqlParameter(
                    "@Department_Code",
                    departmentCode)
            };

            int count = bdl.ExecuteScalar(
                "sp_CheckDuplicateDepartmentCode",
                sqlprms);

            return count > 0;
        }

        public bool IsDepartmentNameDuplicate(
            string departmentName)
        {
            SqlParameter[] sqlprms =
            {
                new SqlParameter(
                    "@Department_Name",
                    departmentName)
            };

            int count = bdl.ExecuteScalar(
                "sp_CheckDuplicateDepartmentName",
                sqlprms);

            return count > 0;
        }

        public bool IsDepartmentNameDuplicateForUpdate(
            string departmentName,
            string originalDepartmentCode)
        {
            SqlParameter[] sqlprms =
            {
                new SqlParameter(
                    "@Department_Name",
                    departmentName),

                new SqlParameter(
                    "@Original_Department_Code",
                    originalDepartmentCode)
            };

            int count = bdl.ExecuteScalar(
                "sp_CheckDuplicateDepartmentNameForUpdate",
                sqlprms);

            return count > 0;
        }

        public DepartmentListViewModel GetDepartmentList(
            string? searchText,
            bool? status,
            int pageNumber = 1,
            int pageSize = 10)
        {
            SqlParameter[] sqlprms =
            {
                new SqlParameter(
                    "@SearchText",
                    (object?)searchText ?? DBNull.Value),

                new SqlParameter(
                    "@Status",
                    (object?)status ?? DBNull.Value),

                new SqlParameter(
                    "@PageNumber",
                    pageNumber),

                new SqlParameter(
                    "@PageSize",
                    pageSize)
            };

            DataTable dataTable = bdl.SelectDataTable(
                "sp_Department_List",
                sqlprms);

            DepartmentListViewModel viewModel =
                new DepartmentListViewModel
                {
                    SearchText = searchText,
                    Status = status,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

            foreach (DataRow row in dataTable.Rows)
            {
                DepartmentListItemViewModel item =
                    new DepartmentListItemViewModel
                    {
                        DepartmentCode =
                            row["Department_Code"]?.ToString()
                            ?? string.Empty,

                        DepartmentName =
                            row["Department_Name"]?.ToString()
                            ?? string.Empty,

                        Description =
                            row["Description"] == DBNull.Value
                                ? null
                                : row["Description"].ToString(),

                        Status =
                            Convert.ToBoolean(row["Status"])
                    };

                viewModel.Departments.Add(item);
            }

            if (dataTable.Rows.Count > 0)
            {
                viewModel.TotalRecords =
                    Convert.ToInt32(
                        dataTable.Rows[0]["TotalRecords"]);
            }

            return viewModel;
        }

        public DepartmentEntryViewModel? GetDepartmentByCode(
            string departmentCode)
        {
            SqlParameter[] sqlprms =
            {
                new SqlParameter(
                    "@Department_Code",
                    departmentCode)
            };

            DataTable dataTable = bdl.SelectDataTable(
                "sp_Department_GetByCode",
                sqlprms);

            if (dataTable.Rows.Count == 0)
            {
                return null;
            }

            DataRow row = dataTable.Rows[0];

            return new DepartmentEntryViewModel
            {
                OriginalDepartmentCode =
                    row["Department_Code"]?.ToString()
                    ?? string.Empty,

                DepartmentCode =
                    row["Department_Code"]?.ToString()
                    ?? string.Empty,

                DepartmentName =
                    row["Department_Name"]?.ToString()
                    ?? string.Empty,

                Description =
                    row["Description"] == DBNull.Value
                        ? null
                        : row["Description"].ToString(),

                Status =
                    Convert.ToBoolean(row["Status"])
            };
        }

        public string Department_Update(
            Department department)
        {
            SqlParameter[] sqlprms =
            {
                new SqlParameter(
                    "@Original_Department_Code",
                    department.OriginalDepartmentCode),

                new SqlParameter(
                    "@Department_Name",
                    department.DepartmentName),

                new SqlParameter(
                    "@Description",
                    department.Description),

                new SqlParameter(
                    "@Status",
                    department.Status)
            };

            return bdl.InsertUpdateDeleteData(
                "sp_Department_Update",
                sqlprms);
        }

        public string DeleteDepartment(
            string departmentCode)
        {
            SqlParameter[] sqlprms =
            {
                new SqlParameter(
                    "@Department_Code",
                    departmentCode)
            };

            return bdl.InsertUpdateDeleteData(
                "sp_Department_Delete",
                sqlprms);
        }
    }
}