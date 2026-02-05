using HospitalManagementSystem.BLL.Services;

namespace HospitalManagementSystem.Helpers
{
    /// <summary>
    /// Keeps current local user session.
    /// </summary>
    public static class UserSession
    {
        /// <summary>
        /// Gets current authenticated user.
        /// </summary>
        public static AuthenticatedUser CurrentUser { get; private set; }

        /// <summary>
        /// Starts a session.
        /// </summary>
        /// <param name="user">Authenticated user.</param>
        public static void Start(AuthenticatedUser user)
        {
            CurrentUser = user;
        }

        /// <summary>
        /// Ends current session.
        /// </summary>
        public static void End()
        {
            CurrentUser = null;
        }
    }
}
