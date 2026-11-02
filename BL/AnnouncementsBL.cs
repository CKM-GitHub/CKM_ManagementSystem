using System.Data;
using CKM_ManagementSystem.DL;
using CKM_ManagementSystem.Models.ViewModels.Announcements;
using Microsoft.Data.SqlClient;

namespace CKM_ManagementSystem.BL
{
    public class AnnouncementsBL
    {
        private readonly BaseDL bdl;

        public AnnouncementsBL(BaseDL baseDL)
        {
            bdl = baseDL;
        }

        public async Task<AnnouncementListViewModel>
     GetAnnouncementListAsync(
         AnnouncementListFilterViewModel filter,
         int pageNumber = 1,
         int pageSize = 6
     )
        {
            SqlParameter[] parameters =
            {
        new SqlParameter(
            "@SearchText",
            string.IsNullOrWhiteSpace(filter.SearchText)
                ? DBNull.Value
                : filter.SearchText
        ),

        new SqlParameter(
            "@Priority",
            filter.Priority.HasValue
                ? filter.Priority.Value
                : DBNull.Value
        ),

        new SqlParameter(
            "@AnnouncedDate",
            filter.AnnouncedDate.HasValue
                ? filter.AnnouncedDate.Value.Date
                : DBNull.Value
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

            DataTable table =
                await bdl.SelectDataTableAsync(
                    "sp_GetAnnouncementList",
                    parameters
                );

            var announcements =
                new List<AnnouncementListItemViewModel>();

            int totalRecords = 0;

            foreach (DataRow row in table.Rows)
            {
                announcements.Add(
                    new AnnouncementListItemViewModel
                    {
                        ID =
                            Convert.ToInt32(
                                row["ID"]
                            ),

                        Title =
                            row["Title"]?.ToString()
                            ?? string.Empty,

                        Content =
                            row["Content"]?.ToString()
                            ?? string.Empty,

                        AnnouncedBy =
                            row["AnnouncedBy"]?.ToString()
                            ?? string.Empty,

                        AnnouncedDate =
                            Convert.ToDateTime(
                                row["AnnouncedDate"]
                            ),

                        Target =
                            row["Target"]?.ToString()
                            ?? string.Empty,

                        Priority =
                            Convert.ToInt32(
                                row["Priority"]
                            )
                    }
                );
            }

            if (table.Rows.Count > 0)
            {
                totalRecords =
                    Convert.ToInt32(
                        table.Rows[0]["TotalRecords"]
                    );
            }

            return new AnnouncementListViewModel
            {
                Filter = filter,
                Announcements = announcements,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
        }
        public async Task<AnnouncementDetailViewModel?> GetAnnouncementDetailAsync(
            int announcementId
 )
        {
            SqlParameter[] parameters =
            {
                new SqlParameter(
            "@AnnouncementID",
            announcementId
        )
    };

            DataTable table =
                await bdl.SelectDataTableAsync(
                    "sp_AnnouncementDetails",
                    parameters
                );

            if (table.Rows.Count == 0)
            {
                return null;
            }

            DataRow row = table.Rows[0];

            return new AnnouncementDetailViewModel
            {
                Title =
                    row["Title"]?.ToString()
                    ?? string.Empty,

                Content =
                    row["Content"]?.ToString()
                    ?? string.Empty,

                AnnouncedBy =
                    row["AnnouncedBy"]?.ToString()
                    ?? string.Empty,

                AnnouncedDate =
                    Convert.ToDateTime(
                        row["AnnouncedDate"]
                    ),

                Priority =
                    Convert.ToInt32(
                        row["Priority"]
                    ),

                Target =
                    row["Target"]?.ToString()
                    ?? string.Empty,

                AnnouncerDepartment =
                    row["AnnouncerDepartment"]?.ToString()
                    ?? string.Empty
            };
        }
        public async Task<bool> DeleteAnnouncementAsync(
                int announcementId
)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter(
            "@AnnouncementID",
            announcementId
        )
    };

            await bdl.ExecuteNonQueryAsync(
                "sp_DeleteAnnouncement",
                parameters
            );

            return true;
        }
    }
}