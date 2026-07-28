namespace CKM_ManagementSystem.DTOs
{
    public class UserManagementDto
    {
        public string StaffCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department_Name {  get; set; } = string.Empty;
        public string Role_Name {  get; set; } = string.Empty;
        public bool Status { get; set; } = false;
        public string imageUrl {  get; set; } = string.Empty;
    }
}
