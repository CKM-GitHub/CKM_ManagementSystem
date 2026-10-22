using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using CKM_ManagementSystem.DL;

namespace CKM_ManagementSystem.BL
{
    public class ProjectDetailsBL : BaseDL
    {
        public ProjectDetailsBL(IConfiguration configuration) : base(configuration) { }

        public DataTable GetProjectListWithTaskCounts(string projectName)
        {
            var parameters = new SqlParameter[]
            {
                CreateParameter("@ProjectName", projectName ?? string.Empty)
            };
            return SelectDataTable("sp_GetProjectListWithTaskCounts", parameters);
        }
       
        public DataTable GetDashboardOverview(string projectName)
        {
            var parameters = new SqlParameter[]
            {
                CreateParameter("@ProjectName", projectName ?? string.Empty)
            };
            return SelectDataTable("sp_GetDashboardOverview", parameters);
        }
        
        public (DataTable ProjectInfo, DataTable ProjectMembers, int TotalMembers) GetProjectDetailsByCode(string projectCode, int pageNumber = 1, int pageSize = 5)
        {
            var parameters = new SqlParameter[]
            {
                CreateParameter("@ProjectCode", projectCode),
                CreateParameter("@PageNumber", pageNumber),
                CreateParameter("@PageSize", pageSize)
            };

            DataSet ds = SelectDataSet("sp_GetProjectDetailsByCode", parameters);

            DataTable projectInfo = new DataTable();
            DataTable projectMembers = new DataTable();
            int totalMembers = 0;

            if(ds != null && ds.Tables.Count > 0)
            {
                projectInfo = ds.Tables[0];

                if(ds.Tables.Count > 1)
                {
                    projectMembers = ds.Tables[1];
                }
                
                if(ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
                {
                    totalMembers = Convert.ToInt32(ds.Tables[2].Rows[0]["TotalMembers"]);
                }
            }
            return (projectInfo, projectMembers, totalMembers);
        }
    }
}