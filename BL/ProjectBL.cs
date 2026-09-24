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
            List<SelectListItem> managers =
                new List<SelectListItem>();

            DataTable dt =
                bdl.ExecuteDataTable(
                    "SP_Project_GetActiveManagers",
                    Array.Empty<SqlParameter>()
                );

            if (
                dt != null &&
                dt.Rows.Count > 0
            )
            {
                foreach (DataRow row in dt.Rows)
                {
                    managers.Add(
                        new SelectListItem
                        {
                            Value =
                                row["Staff_Code"]
                                    .ToString(),

                            Text =
                                row["Name"]
                                    .ToString()
                        }
                    );
                }
            }

            return managers;
        }

        public List<SelectListItem> GetDepartments()
        {
            List<SelectListItem> departments =
                new List<SelectListItem>();

            DataTable dt =
                bdl.ExecuteDataTable(
                    "sp_Project_GetDepartments",
                    Array.Empty<SqlParameter>()
                );

            if (
                dt != null &&
                dt.Rows.Count > 0
            )
            {
                foreach (DataRow row in dt.Rows)
                {
                    departments.Add(
                        new SelectListItem
                        {
                            Value =
                                row["Department_Code"]
                                    .ToString(),

                            Text =
                                row["Department_Name"]
                                    .ToString()
                        }
                    );
                }
            }

            return departments;
        }

        public bool IsDuplicateProjectCode(
            string projectCode)
        {
            SqlParameter[] sqlprms =
            {
                new SqlParameter(
                    "@ProjectCode",
                    (object)projectCode
                    ?? DBNull.Value
                )
            };

            int count =
                bdl.ExecuteScalar(
                    "sp_CheckDuplicateProjectCode",
                    sqlprms
                );

            return count > 0;
        }

        public bool IsDuplicateProjectName(
            string projectName,
            string? currentProjectCode = null)
        {
            SqlParameter[] sqlprms =
            {
                new SqlParameter(
                    "@ProjectName",
                    (object)projectName
                    ?? DBNull.Value
                ),

                new SqlParameter(
                    "@CurrentProjectCode",
                    (object?)currentProjectCode
                    ?? DBNull.Value
                )
            };

            int count =
                bdl.ExecuteScalar(
                    "sp_CheckDuplicateProjectName",
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

            SqlParameter[] sqlprms =
            {
                new SqlParameter(
                    "@SearchText",
                    string.IsNullOrEmpty(
                        searchText)
                        ? DBNull.Value
                        : searchText
                ),

                new SqlParameter(
                    "@DepartmentCode",
                    string.IsNullOrEmpty(
                        departmentCode)
                        ? DBNull.Value
                        : departmentCode
                )
            };

            DataTable dt =
                bdl.ExecuteDataTable(
                    "sp_GetProjectMembers_Dropdown",
                    sqlprms
                );

            if (
                dt != null &&
                dt.Rows.Count > 0
            )
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
                                row["Image_URL"]
                                    != DBNull.Value
                                    ? row["Image_URL"]
                                        .ToString()
                                    : null,

                            Department_Name =
                                row["Department_Name"]
                                    != DBNull.Value
                                    ? row["Department_Name"]
                                        .ToString()
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
                string projectCommand;

                if (isEdit)
                {
                    projectCommand =
                        "sp_Project_Update";
                }
                else
                {
                    projectCommand =
                        "sp_InsertProject";
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
                        string.IsNullOrEmpty(
                            model.Status)
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
                        projectCommand,
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
                        SqlParameter[] delParams =
                        {
                            new SqlParameter(
                                "@ProjectCode",
                                model.ProjectCode
                            )
                        };

                        bdl.InsertUpdateDeleteData(
                            "sp_Project_DeleteMembers",
                            delParams
                        );
                    }

                    if (
                        model.ProjectMembers != null &&
                        model.ProjectMembers.Count > 0
                    )
                    {
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
                                "sp_Project_InsertMember",
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
                    string.IsNullOrWhiteSpace(
                        status)
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

            SqlParameter[] sqlprms =
            {
                new SqlParameter(
                    "@ProjectCode",
                    projectCode
                )
            };

            DataTable dt =
                bdl.ExecuteDataTable(
                    "sp_Project_GetById",
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

            SqlParameter[] sqlprms =
            {
                new SqlParameter(
                    "@ProjectCode",
                    projectCode
                )
            };

            DataTable dt =
                bdl.ExecuteDataTable(
                    "sp_Project_GetMembers",
                    sqlprms
                );

            if (
                dt != null &&
                dt.Rows.Count > 0
            )
            {
                foreach (
                    DataRow row in dt.Rows)
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