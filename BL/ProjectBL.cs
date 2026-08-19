using System;
using System.Collections.Generic;
using System.Data;
using CKM_ManagementSystem.Models.Entities;
using CKM_ManagementSystem.DL;
using Microsoft.Data.SqlClient;

namespace CKM_ManagementSystem.BL
{
    public class ProjectBL
    {
        private readonly BaseDL bdl;

        public ProjectBL(BaseDL baseDL)
        {
            bdl = baseDL;
        }

       
        public string Project_Insert(Project project)
        {
            SqlParameter[] sqlprms =
            {
                new SqlParameter("@ProjectCode", (object)project.ProjectCode ?? DBNull.Value),
                new SqlParameter("@ProjectName", (object)project.ProjectName ?? DBNull.Value),
                new SqlParameter("@ProjectManagerId", (object)project.ProjectManagerId ?? DBNull.Value),
                new SqlParameter("@GitRepositoryUrl", string.IsNullOrEmpty(project.GitRepositoryUrl) ? DBNull.Value : project.GitRepositoryUrl),
                new SqlParameter("@Description", string.IsNullOrEmpty(project.Description) ? DBNull.Value : project.Description),
                new SqlParameter("@StartDate", project.StartDate == default(DateTime) ? DBNull.Value : project.StartDate),
                new SqlParameter("@EndDate", project.EndDate == default(DateTime) ? DBNull.Value : project.EndDate),
                new SqlParameter("@Status", string.IsNullOrEmpty(project.Status) ? "Active" : project.Status)
            };

            return bdl.InsertUpdateDeleteData("sp_InsertProject", sqlprms);
        }

        
        public bool IsProjectCodeDuplicate(string projectCode)
        {
            SqlParameter[] sqlprms =
            {
                new SqlParameter("@ProjectCode", (object)projectCode ?? DBNull.Value)
            };

            int count = bdl.ExecuteScalar("sp_CheckDuplicateProjectCode", sqlprms);

            return count > 0;
        }

        public bool IsProjectNameDuplicate(string projectName)
        {
            SqlParameter[] sqlprms =
            {
                new SqlParameter("@ProjectName", (object)projectName ?? DBNull.Value)
            };

            int count = bdl.ExecuteScalar("sp_CheckDuplicateProjectName", sqlprms);

            return count > 0;
        }

        
        public List<dynamic> GetActiveManagers()
        {
            List<dynamic> managers = new List<dynamic>();

            DataTable dt = bdl.SelectData("sp_GetActiveManagers", null);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    managers.Add(new
                    {
                        StaffCode = row["StaffCode"].ToString(),
                        Name = row["Name"].ToString()
                    });
                }
            }

            return managers;
        }
    }
}