namespace HospitalManagementSystem.BLL.Services
{
    /// <summary>
    /// Represents an authenticated application user.
    /// </summary>
    public sealed class AuthenticatedUser
    {
        public int UserID { get; set; }

        public string Username { get; set; }

        public int RoleID { get; set; }

        public string RoleName { get; set; }
    }
}
