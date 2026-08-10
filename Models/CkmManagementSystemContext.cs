using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CKM_ManagementSystem.Models;

public partial class CkmManagementSystemContext : DbContext
{
    public CkmManagementSystemContext()
    {
    }

    public CkmManagementSystemContext(DbContextOptions<CkmManagementSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<UserRolePermission> UserRolePermissions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=163.43.116.245;Initial Catalog=CKM_ManagementSystem;User ID=sa;Password=admin123456!;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentCode);

            entity.HasIndex(e => e.DepartmentCode, "UQ_Department_Code").IsUnique();

            entity.HasIndex(e => e.DepartmentName, "UQ_Department_Name").IsUnique();

            entity.Property(e => e.DepartmentCode)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Department_Code");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("Created_Date");
            entity.Property(e => e.DeletedDate).HasColumnName("Deleted_Date");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(150)
                .HasColumnName("Department_Name");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.UpdatedDate).HasColumnName("Updated_Date");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.Property(e => e.MenuId).HasColumnName("MenuID");
            entity.Property(e => e.ActionName).HasMaxLength(50);
            entity.Property(e => e.ControllerName).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("Created_Date");
            entity.Property(e => e.DeletedDate).HasColumnName("Deleted_Date");
            entity.Property(e => e.MenuIcon).HasMaxLength(30);
            entity.Property(e => e.MenuName).HasMaxLength(50);
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.UpdatedDate).HasColumnName("Updated_Date");

            entity.HasOne(d => d.ParentMenu).WithMany(p => p.InverseParentMenu)
                .HasForeignKey(d => d.ParentMenuId)
                .HasConstraintName("FK_Menus_Parent");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.StaffCode);

            entity.HasIndex(e => e.Email, "UQ_Users_Email").IsUnique();

            entity.HasIndex(e => e.StaffCode, "UQ_Users_StaffCode").IsUnique();

            entity.Property(e => e.StaffCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Staff_Code");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("Created_Date");
            entity.Property(e => e.DeletedDate).HasColumnName("Deleted_Date");
            entity.Property(e => e.DepartmentCode)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Department_Code");
            entity.Property(e => e.Email)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(400)
                .HasColumnName("Image_URL");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RoleCode)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Role_Code");
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.TimeZone)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("UTC");
            entity.Property(e => e.UpdatedDate).HasColumnName("Updated_Date");

            entity.HasOne(d => d.DepartmentCodeNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.DepartmentCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Department");

            entity.HasOne(d => d.RoleCodeNavigation).WithMany(p => p.Users)
                .HasPrincipalKey(p => p.RoleCode)
                .HasForeignKey(d => d.RoleCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Role");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasIndex(e => e.RoleCode, "UQ_UserRoles_RoleCode").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("ID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("Created_Date");
            entity.Property(e => e.DeletedDate).HasColumnName("Deleted_Date");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.RoleCode)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Role_Code");
            entity.Property(e => e.RoleName)
                .HasMaxLength(100)
                .HasColumnName("Role_Name");
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.UpdatedDate).HasColumnName("Updated_Date");
        });

        modelBuilder.Entity<UserRolePermission>(entity =>
        {
            entity.HasKey(e => new { e.RoleCode, e.MenuId });

            entity.Property(e => e.RoleCode)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Role_Code");
            entity.Property(e => e.MenuId).HasColumnName("MenuID");
            entity.Property(e => e.CanDelete).HasDefaultValue(true);
            entity.Property(e => e.CanRead).HasDefaultValue(true);
            entity.Property(e => e.CanWrite).HasDefaultValue(true);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("Created_Date");
            entity.Property(e => e.DeletedDate).HasColumnName("Deleted_Date");
            entity.Property(e => e.UpdatedDate).HasColumnName("Updated_Date");

            entity.HasOne(d => d.Menu).WithMany(p => p.UserRolePermissions)
                .HasForeignKey(d => d.MenuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRolePermissions_Menu");

            entity.HasOne(d => d.RoleCodeNavigation).WithMany(p => p.UserRolePermissions)
                .HasPrincipalKey(p => p.RoleCode)
                .HasForeignKey(d => d.RoleCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRolePermissions_Role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
