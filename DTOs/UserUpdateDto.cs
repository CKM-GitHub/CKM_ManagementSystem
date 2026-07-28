namespace CKM_ManagementSystem.DTOs
{
    public class UserUpdateDto
    {
        public string StaffCode { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string DepartmentCode { get; set; }
        public string RoleCode { get; set; }
        public bool Status { get; set; }
        public string imageUrl { get; set; }
    }
}
