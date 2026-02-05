namespace HospitalManagementSystem.DAL.DTOs
{
    /// <summary>
    /// Represents an authenticated user projection from the database.
    /// </summary>
    public sealed class AuthenticatedUserDto
    {
        public int UserID { get; set; }

        public string Username { get; set; }

        public string PasswordHash { get; set; }

        public int RoleID { get; set; }

        public string RoleName { get; set; }

        public bool IsActive { get; set; }
    }
}
