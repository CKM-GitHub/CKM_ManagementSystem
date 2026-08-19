using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.Models.Entities;
using CKM_ManagementSystem.Models.ViewModels.Projects;

namespace CKM_ManagementSystem.Services
{
    public class ProjectService
    {
        private readonly ProjectBL _projectBL;

        public ProjectService(ProjectBL projectBL)
        {
            _projectBL = projectBL;
        }

        public List<dynamic> GetActiveManagers()
        {
            
            return _projectBL.GetActiveManagers();
        }

        public async Task<(bool isSuccess, string message)> SaveProjectAsync(ProjectEntryViewModel model)
        {
           
            if (model.StartDate >= model.EndDate)
            {
                return (false, "Start Date must be earlier than Target End Date.");
            }

            
            if (_projectBL.IsProjectCodeDuplicate(model.ProjectCode))
            {
                return (false, "Project Code already exists.");
            }

            
            if (_projectBL.IsProjectNameDuplicate(model.ProjectName))
            {
                return (false, "Project Name already exists.");
            }

            var entity = new Project
            {
                ProjectCode = model.ProjectCode,
                ProjectName = model.ProjectName,
                ProjectManagerId = model.ProjectManagerId,
                GitRepositoryUrl = model.GitRepositoryUrl,
                Description = model.Description,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Status = string.IsNullOrEmpty(model.Status) ? "Active" : model.Status
            };

            string result = await Task.Run(() => _projectBL.Project_Insert(entity));

            if (result == "true")
            {
                return (true, "Project created successfully.");
            }

            return (false, "Failed to save project.");
        }
    }
}