using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using CKM_ManagementSystem.Models.Entities;
using CKM_ManagementSystem.Models.ViewModels.Project;
using CKM_ManagementSystem.DL;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CKM_ManagementSystem.BL
{
    public class ProjectBL
    {
        private readonly BaseDL bdl;

        public ProjectBL(BaseDL baseDL)
        {
            bdl = baseDL;
        }

        public List<SelectListItem> GetActiveManagers()
        {
            List<SelectListItem> managers = new List<SelectListItem>();

            string query =
                "SELECT Staff_Code, Name FROM Users WHERE Deleted_Date IS NULL";

            DataTable dt = bdl.ExecuteDataTable(
                query,
                Array.Empty<SqlParameter>()
            );

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    managers.Add(new SelectListItem
                    {
                        Value = row["Staff_Code"].ToString(),
                        Text = row["Name"].ToString()
                    });
                }
            }

            return managers;
        }

        public List<SelectListItem> GetDepartments()
        {
            List<SelectListItem> departments =
                new List<SelectListItem>();

            string query =
                "SELECT Department_Code, Department_Name " +
                "FROM Departments " +
                "WHERE Status = 1 " +
                "AND Deleted_Date IS NULL " +
                "ORDER BY Department_Name";

            DataTable dt = bdl.ExecuteDataTable(
                query,
                Array.Empty<SqlParameter>()
            );

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    departments.Add(new SelectListItem
                    {
                        Value =
                            row["Department_Code"].ToString(),

                        Text =
                            row["Department_Name"].ToString()
                    });
                }
            }

            return departments;
        }

        public bool IsDuplicateProjectCode(
            string projectCode)
        {
            string query =
                "SELECT COUNT(1) " +
                "FROM Projects " +
                "WHERE ProjectCode = @ProjectCode";

            SqlParameter[] sqlprms =
            {
                new SqlParameter(
                    "@ProjectCode",
                    (object)projectCode ?? DBNull.Value
                )
            };

            int count =
                bdl.ExecuteScalar(
                    query,
                    sqlprms
                );

            return count > 0;
        }

        public bool IsDuplicateProjectName(
            string projectName,
            string? currentProjectCode = null)
        {
            string query =
                "SELECT COUNT(1) " +
                "FROM Projects " +
                "WHERE ProjectName = @ProjectName";

            if (!string.IsNullOrEmpty(currentProjectCode))
            {
                query +=
                    " AND ProjectCode != @CurrentProjectCode";
            }

            SqlParameter[] sqlprms =
            {
                new SqlParameter(
                    "@ProjectName",
                    (object)projectName ?? DBNull.Value
                ),

                new SqlParameter(
                    "@CurrentProjectCode",
                    (object)currentProjectCode ?? DBNull.Value
                )
            };

            int count =
                bdl.ExecuteScalar(
                    query,
                    sqlprms
                );

            return count > 0;
        }

        public List<ProjectMemberSearchViewModel>
            SearchProjectMembers(
                string? searchText,
                string? departmentCode)
        {
            List<ProjectMemberSearchViewModel> members =
                new List<ProjectMemberSearchViewModel>();

            string query = @"
                SELECT
                    u.Staff_Code,
                    u.Name,
                    u.Image_URL,
                    d.Department_Name
                FROM Users u
                LEFT JOIN Departments d
                    ON d.Department_Code = u.Department_Code
                WHERE u.Deleted_Date IS NULL
                  AND
                  (
                      @SearchText IS NULL
                      OR @SearchText = ''
                      OR u.Staff_Code LIKE '%' + @SearchText + '%'
                      OR u.Name LIKE '%' + @SearchText + '%'
                  )
                  AND
                  (
                      @DepartmentCode IS NULL
                      OR @DepartmentCode = ''
                      OR d.Department_Code = @DepartmentCode
                  )";

            SqlParameter[] sqlprms =
            {
                new SqlParameter(
                    "@SearchText",
                    string.IsNullOrEmpty(searchText)
                        ? DBNull.Value
                        : searchText
                ),

                new SqlParameter(
                    "@DepartmentCode",
                    string.IsNullOrEmpty(departmentCode)
                        ? DBNull.Value
                        : departmentCode
                )
            };

            DataTable dt =
                bdl.ExecuteDataTable(
                    query,
                    sqlprms
                );

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    members.Add(
                        new ProjectMemberSearchViewModel
                        {
                            Staff_Code =
                                row["Staff_Code"]
                                    .ToString() ?? "",

                            Name =
                                row["Name"]
                                    .ToString() ?? "",

                            Image_URL =
                                row["Image_URL"] != DBNull.Value
                                    ? row["Image_URL"].ToString()
                                    : null,

                            Department_Name =
                                row["Department_Name"] != DBNull.Value
                                    ? row["Department_Name"].ToString()
                                    : null
                        }
                    );
                }
            }

            return members;
        }

        public bool SaveProject(
            ProjectEntryViewModel model,
            bool isEdit,
            out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!Regex.IsMatch(
                model.ProjectCode ?? "",
                @"^[a-zA-Z0-9\-_]+$"))
            {
                errorMessage =
                    "Project Code contains invalid characters. Special characters are not allowed.";

                return false;
            }

            if (!Regex.IsMatch(
                model.ProjectName ?? "",
                @"^[a-zA-Z0-9\s\-_]+$"))
            {
                errorMessage =
                    "Project Name contains invalid characters. Special characters are not allowed.";

                return false;
            }

            try
            {
                string queryProject;

                if (isEdit)
                {
                    queryProject = @"
                        UPDATE Projects
                        SET
                            ProjectName = @ProjectName,
                            ProjectManagerId = @ProjectManagerId,
                            GitRepositoryUrl = @GitRepositoryUrl,
                            Description = @Description,
                            StartDate = @StartDate,
                            EndDate = @EndDate,
                            Status = @Status,
                            ProjectType = @ProjectType,
                            Updated_Date = GETDATE()
                        WHERE ProjectCode = @ProjectCode";
                }
                else
                {
                    queryProject = @"
                        INSERT INTO Projects
                        (
                            ProjectCode,
                            ProjectName,
                            ProjectManagerId,
                            GitRepositoryUrl,
                            Description,
                            StartDate,
                            EndDate,
                            Status,
                            ProjectType,
                            Created_Date,
                            Updated_Date
                        )
                        VALUES
                        (
                            @ProjectCode,
                            @ProjectName,
                            @ProjectManagerId,
                            @GitRepositoryUrl,
                            @Description,
                            @StartDate,
                            @EndDate,
                            @Status,
                            @ProjectType,
                            GETDATE(),
                            GETDATE()
                        )";
                }

                SqlParameter[] sqlprms =
                {
                    new SqlParameter(
                        "@ProjectCode",
                        (object)model.ProjectCode
                        ?? DBNull.Value
                    ),

                    new SqlParameter(
                        "@ProjectName",
                        (object)model.ProjectName
                        ?? DBNull.Value
                    ),

                    new SqlParameter(
                        "@ProjectManagerId",
                        (object)model.ProjectManagerId
                        ?? DBNull.Value
                    ),

                    new SqlParameter(
                        "@GitRepositoryUrl",
                        string.IsNullOrEmpty(
                            model.GitRepositoryUrl)
                            ? DBNull.Value
                            : model.GitRepositoryUrl
                    ),

                    new SqlParameter(
                        "@Description",
                        string.IsNullOrEmpty(
                            model.Description)
                            ? DBNull.Value
                            : model.Description
                    ),

                    new SqlParameter(
                        "@StartDate",
                        model.StartDate == default(DateTime)
                            ? DBNull.Value
                            : model.StartDate
                    ),

                    new SqlParameter(
                        "@EndDate",
                        model.EndDate == default(DateTime)
                            ? DBNull.Value
                            : model.EndDate
                    ),

                    new SqlParameter(
                        "@Status",
                        string.IsNullOrEmpty(model.Status)
                            ? "Active"
                            : model.Status
                    ),

                    new SqlParameter(
                        "@ProjectType",
                        string.IsNullOrEmpty(
                            model.ProjectType)
                            ? "SYS"
                            : model.ProjectType
                    )
                };

                string result =
                    bdl.InsertUpdateDeleteData(
                        queryProject,
                        sqlprms
                    );

                if (
                    result == "true" ||
                    result == "1" ||
                    string.IsNullOrEmpty(result)
                )
                {
                    if (isEdit)
                    {
                        string deleteMembersQuery =
                            "DELETE FROM ProjectMembers " +
                            "WHERE ProjectCode = @ProjectCode";

                        SqlParameter[] delParams =
                        {
                            new SqlParameter(
                                "@ProjectCode",
                                model.ProjectCode
                            )
                        };

                        bdl.InsertUpdateDeleteData(
                            deleteMembersQuery,
                            delParams
                        );
                    }

                    if (
                        model.ProjectMembers != null &&
                        model.ProjectMembers.Count > 0
                    )
                    {
                        string insertMemberQuery = @"
                            INSERT INTO ProjectMembers
                            (
                                ProjectCode,
                                Staff_Code,
                                Created_Date,
                                Updated_Date
                            )
                            VALUES
                            (
                                @ProjectCode,
                                @Staff_Code,
                                GETDATE(),
                                GETDATE()
                            )";

                        foreach (
                            var member
                            in model.ProjectMembers)
                        {
                            SqlParameter[] memParams =
                            {
                                new SqlParameter(
                                    "@ProjectCode",
                                    model.ProjectCode
                                ),

                                new SqlParameter(
                                    "@Staff_Code",
                                    member.Staff_Code
                                )
                            };

                            bdl.InsertUpdateDeleteData(
                                insertMemberQuery,
                                memParams
                            );
                        }
                    }

                    return true;
                }

                errorMessage = result;

                return false;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;

                return false;
            }
        }

        public ProjectListPagedViewModel GetProjectList(
            string? searchKeyword,
            string? status,
            int pageNumber = 1,
            int pageSize = 6)
        {
            ProjectListPagedViewModel model =
                new ProjectListPagedViewModel
                {
                    SearchKeyword =
                        searchKeyword ?? string.Empty,

                    Status =
                        status ?? string.Empty,

                    PageNumber =
                        pageNumber,

                    PageSize =
                        pageSize
                };

            SqlParameter[] sqlprms =
            {
                new SqlParameter(
                    "@SearchText",
                    string.IsNullOrWhiteSpace(
                        searchKeyword)
                        ? DBNull.Value
                        : searchKeyword
                ),

                new SqlParameter(
                    "@Status",
                    string.IsNullOrWhiteSpace(status)
                        ? DBNull.Value
                        : status
                ),

                new SqlParameter(
                    "@PageNumber",
                    pageNumber
                ),

                new SqlParameter(
                    "@PageSize",
                    pageSize
                )
            };

            DataTable dt =
                bdl.ExecuteDataTable(
                    "sp_Project_List",
                    sqlprms
                );

            if (
                dt != null &&
                dt.Rows.Count > 0
            )
            {
                model.TotalRecords =
                    Convert.ToInt32(
                        dt.Rows[0]["TotalRecords"]
                    );

                foreach (
                    DataRow row
                    in dt.Rows)
                {
                    model.Projects.Add(
                        new ProjectListItemViewModel
                        {
                            No =
                                Convert.ToInt64(
                                    row["No"]
                                ),

                            ProjectCode =
                                row["ProjectCode"]
                                    ?.ToString()
                                ?? string.Empty,

                            ProjectName =
                                row["ProjectName"]
                                    ?.ToString()
                                ?? string.Empty,

                            ProjectManagerName =
                                row["ProjectManagerName"]
                                    ?.ToString()
                                ?? string.Empty,

                            GitRepositoryUrl =
                                row["GitRepositoryUrl"]
                                    != DBNull.Value
                                    ? row["GitRepositoryUrl"]
                                        .ToString()
                                    : null,

                            ProjectMemberCount =
                                row["ProjectMemberCount"]
                                    != DBNull.Value
                                    ? Convert.ToInt32(
                                        row["ProjectMemberCount"]
                                      )
                                    : 0,

                            StartDate =
                                row["StartDate"]
                                    != DBNull.Value
                                    ? Convert.ToDateTime(
                                        row["StartDate"]
                                      )
                                    : DateTime.MinValue,

                            EndDate =
                                row["EndDate"]
                                    != DBNull.Value
                                    ? Convert.ToDateTime(
                                        row["EndDate"]
                                      )
                                    : DateTime.MinValue,

                            Status =
                                row["Status"]
                                    ?.ToString()
                                ?? string.Empty,

                            ProjectType =
                                row["ProjectType"]
                                    ?.ToString()
                                ?? string.Empty
                        }
                    );
                }
            }

            return model;
        }

        public (bool Success, string Message)
            DeleteProject(
                string projectCode)
        {
            ProjectEntryViewModel project =
                GetProjectById(projectCode);

            if (
                project == null ||
                string.IsNullOrEmpty(
                    project.ProjectCode)
            )
            {
                return (
                    false,
                    "Project not found."
                );
            }

            if (
                string.Equals(
                    project.Status,
                    "Active",
                    StringComparison.OrdinalIgnoreCase)
            )
            {
                return (
                    false,
                    "Cannot delete an active project."
                );
            }

            SqlParameter[] sqlprms =
            {
                new SqlParameter(
                    "@ProjectCode",
                    (object?)projectCode
                    ?? DBNull.Value
                )
            };

            string result =
                bdl.InsertUpdateDeleteData(
                    "sp_Project_Delete",
                    sqlprms
                );

            if (
                string.IsNullOrEmpty(result) ||
                result.Equals(
                    "true",
                    StringComparison.OrdinalIgnoreCase) ||
                result == "1"
            )
            {
                return (
                    true,
                    "Project deleted successfully."
                );
            }

            return (
                false,
                "An error occurred while deleting the project: "
                + result
            );
        }

        public ProjectEntryViewModel GetProjectById(
            string projectCode)
        {
            ProjectEntryViewModel model =
                new ProjectEntryViewModel();

            string query = @"
                SELECT
                    ProjectCode,
                    ProjectName,
                    ProjectManagerId,
                    GitRepositoryUrl,
                    Description,
                    StartDate,
                    EndDate,
                    Status,
                    ProjectType
                FROM Projects
                WHERE ProjectCode = @ProjectCode";

            SqlParameter[] sqlprms =
            {
                new SqlParameter(
                    "@ProjectCode",
                    projectCode
                )
            };

            DataTable dt =
                bdl.ExecuteDataTable(
                    query,
                    sqlprms
                );

            if (
                dt != null &&
                dt.Rows.Count > 0
            )
            {
                DataRow row =
                    dt.Rows[0];

                model.ProjectCode =
                    row["ProjectCode"]
                        .ToString() ?? "";

                model.ProjectName =
                    row["ProjectName"]
                        .ToString() ?? "";

                model.ProjectManagerId =
                    row["ProjectManagerId"]
                        != DBNull.Value
                        ? row["ProjectManagerId"]
                            .ToString()!
                        : "";

                model.GitRepositoryUrl =
                    row["GitRepositoryUrl"]
                        != DBNull.Value
                        ? row["GitRepositoryUrl"]
                            .ToString()
                        : null;

                model.Description =
                    row["Description"]
                        != DBNull.Value
                        ? row["Description"]
                            .ToString()
                        : null;

                model.StartDate =
                    row["StartDate"]
                        != DBNull.Value
                        ? Convert.ToDateTime(
                            row["StartDate"]
                          )
                        : DateTime.Today;

                model.EndDate =
                    row["EndDate"]
                        != DBNull.Value
                        ? Convert.ToDateTime(
                            row["EndDate"]
                          )
                        : DateTime.Today;

                model.Status =
                    row["Status"]
                        != DBNull.Value
                        ? row["Status"]
                            .ToString()!
                        : "Active";

                model.ProjectType =
                    row["ProjectType"]
                        != DBNull.Value
                        ? row["ProjectType"]
                            .ToString()!
                        : "SYS";

                model.IsEdit = true;

                model.ProjectMembers =
                    GetProjectMembers(
                        projectCode
                    );
            }

            return model;
        }

        private List<ProjectMemberViewModel>
            GetProjectMembers(
                string projectCode)
        {
            List<ProjectMemberViewModel> members =
                new List<ProjectMemberViewModel>();

            string query = @"
                SELECT
                    pm.Staff_Code,
                    u.Name,
                    u.Image_URL
                FROM ProjectMembers pm
                INNER JOIN Users u
                    ON u.Staff_Code = pm.Staff_Code
                WHERE pm.ProjectCode = @ProjectCode";

            SqlParameter[] sqlprms =
            {
                new SqlParameter(
                    "@ProjectCode",
                    projectCode
                )
            };

            DataTable dt =
                bdl.ExecuteDataTable(
                    query,
                    sqlprms
                );

            if (
                dt != null &&
                dt.Rows.Count > 0
            )
            {
                foreach (
                    DataRow row
                    in dt.Rows)
                {
                    members.Add(
                        new ProjectMemberViewModel
                        {
                            Staff_Code =
                                row["Staff_Code"]
                                    .ToString() ?? "",

                            Name =
                                row["Name"]
                                    .ToString() ?? "",

                            Image_URL =
                                row["Image_URL"]
                                    != DBNull.Value
                                    ? row["Image_URL"]
                                        .ToString()
                                    : null
                        }
                    );
                }
            }

            return members;
        }
    }
}