using CKM_ManagementSystem.Data;
using CKM_ManagementSystem.Models.Entities;
using CKM_ManagementSystem.Models.ViewModels;
using CKM_ManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CKM_ManagementSystem.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly ApplicationDbContext _context;

        public DepartmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool>IsDepartmentCodeDuplicateAsync(string departmentCode)
        {
            return await _context.Departments
                .AnyAsync(department =>
                department.DepartmentCode == departmentCode &&
                department.DeletedDate == null);
        }
        public async Task<bool>IsDepartmentNameDuplicateAsync(string departmentName)
        {
            return await _context.Departments.AnyAsync(
                d => d.DepartmentName == departmentName &&
                d.DeletedDate == null);
        }
        public async Task CreateAsync(DepartmentEntryViewModel model)
        {
            Department department = new Department();

            department.Id = Guid.NewGuid();
            department.DepartmentCode = model.DepartmentCode;   
            department.DepartmentName = model.DepartmentName;
            department.Description = model.Description;
            department.Status = model.Status;
            department.CreatedDate= DateTime.Now;

            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();

        }
    }

}