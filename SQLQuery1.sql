USE CKM_ManagementSystem;
GO


CREATE OR ALTER PROCEDURE sp_CheckDuplicateRoleCode
    @RoleCode VARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(1) FROM UserRoles WHERE Role_Code = @RoleCode;
END
GO


CREATE OR ALTER PROCEDURE sp_SaveRoleInfo
    @RoleCode VARCHAR(30),
    @RoleName NVARCHAR(100),
    @Description NVARCHAR(500),
    @Status BIT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO UserRoles (Role_Code, Role_Name, Description, Status, Created_Date)
    VALUES (@RoleCode, @RoleName, @Description, @Status, GETDATE());
END
GO


CREATE OR ALTER PROCEDURE sp_SaveRolePermission
    @RoleCode VARCHAR(30),
    @MenuId INT,
    @CanRead BIT,
    @CanWrite BIT,
    @CanDelete BIT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO UserRolePermissions (Role_Code, MenuID, CanRead, CanWrite, CanDelete, Created_Date)
    VALUES (@RoleCode, @MenuId, @CanRead, @CanWrite, @CanDelete, GETDATE());
END
GO