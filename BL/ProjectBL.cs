using System;
using System.Collections.Generic;
using System.Data;
using CKM_ManagementSystem.Models.Entities;
using CKM_ManagementSystem.Models.ViewModels.Projects;
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

            
            string query = "SELECT Staff_Code, Name FROM Users WHERE Status = 1 AND Deleted_Date IS NULL";
            DataTable dt = bdl.SelectData(query, Array.Empty<SqlParameter>());

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

        public bool IsDuplicateProjectCode(string projectCode)
        {
            string query = "SELECT COUNT(1) FROM Projects WHERE ProjectCode = @ProjectCode";
            SqlParameter[] sqlprms =
            {
                new SqlParameter("@ProjectCode", (object)projectCode ?? DBNull.Value)
            };

            int count = bdl.ExecuteScalar(query, sqlprms);
            return count > 0;
        }

        public bool SaveProject(ProjectEntryViewModel model, bool isEdit, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                string query = "";
                if (isEdit)
                {
                    query = @"UPDATE Projects 
                              SET ProjectName = @ProjectName, 
                                  ProjectManagerId = @ProjectManagerId, 
                                  GitRepositoryUrl = @GitRepositoryUrl, 
                                  Description = @Description, 
                                  StartDate = @StartDate, 
                                  EndDate = @EndDate, 
                                  Status = @Status,
                                  Updated_Date = GETDATE()
                              WHERE ProjectCode = @ProjectCode";
                }
                else
                {
                    query = @"INSERT INTO Projects 
                                (ProjectCode, ProjectName, ProjectManagerId, GitRepositoryUrl, Description, StartDate, EndDate, Status, Created_Date) 
                              VALUES 
                                (@ProjectCode, @ProjectName, @ProjectManagerId, @GitRepositoryUrl, @Description, @StartDate, @EndDate, @Status, GETDATE())";
                }

                SqlParameter[] sqlprms =
                {
                    new SqlParameter("@ProjectCode", (object)model.ProjectCode ?? DBNull.Value),
                    new SqlParameter("@ProjectName", (object)model.ProjectName ?? DBNull.Value),
                    new SqlParameter("@ProjectManagerId", (object)model.ProjectManagerId ?? DBNull.Value),
                    new SqlParameter("@GitRepositoryUrl", string.IsNullOrEmpty(model.GitRepositoryUrl) ? DBNull.Value : model.GitRepositoryUrl),
                    new SqlParameter("@Description", string.IsNullOrEmpty(model.Description) ? DBNull.Value : model.Description),
                    new SqlParameter("@StartDate", model.StartDate == default(DateTime) ? DBNull.Value : model.StartDate),
                    new SqlParameter("@EndDate", model.EndDate == default(DateTime) ? DBNull.Value : model.EndDate),
                    new SqlParameter("@Status", string.IsNullOrEmpty(model.Status) ? "Active" : model.Status)
                };

                string result = bdl.InsertUpdateDeleteData(query, sqlprms);

                if (result == "true" || result == "1" || string.IsNullOrEmpty(result))
                {
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

        public ProjectEntryViewModel GetProjectById(string projectCode)
        {
            var model = new ProjectEntryViewModel();
            string query = "SELECT ProjectCode, ProjectName, ProjectManagerId, GitRepositoryUrl, Description, StartDate, EndDate, Status FROM Projects WHERE ProjectCode = @ProjectCode";
            SqlParameter[] sqlprms =
            {
                new SqlParameter("@ProjectCode", projectCode)
            };

            DataTable dt = bdl.SelectData(query, sqlprms);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                model.ProjectCode = row["ProjectCode"].ToString() ?? "";
                model.ProjectName = row["ProjectName"].ToString() ?? "";
                model.ProjectManagerId = row["ProjectManagerId"] != DBNull.Value ? row["ProjectManagerId"].ToString()! : "";
                model.GitRepositoryUrl = row["GitRepositoryUrl"] != DBNull.Value ? row["GitRepositoryUrl"].ToString() : null;
                model.Description = row["Description"] != DBNull.Value ? row["Description"].ToString() : null;
                model.StartDate = row["StartDate"] != DBNull.Value ? Convert.ToDateTime(row["StartDate"]) : DateTime.Today;
                model.EndDate = row["EndDate"] != DBNull.Value ? Convert.ToDateTime(row["EndDate"]) : DateTime.Today;
                model.Status = row["Status"] != DBNull.Value ? row["Status"].ToString()! : "Active";
            }

            return model;
        }
    }
}