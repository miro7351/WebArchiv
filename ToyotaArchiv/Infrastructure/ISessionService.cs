namespace ToyotaArchiv.Infrastructure
{
    public interface ISessionService
    {
        void WriteUserToSession(ISession session, UserDetail userDetail);
        public UserDetail? ReadUserFromSession(ISession session);

       
        string ReadLoginFromSession(ISession session);
        string ReadRoleFromSession(ISession session);

        (string? userLogin, string? userRole) ReadUserLoginAndRoleFromSession(ISession session);
    }
}
