using ToyotaArchiv.Global;

namespace ToyotaArchiv.Infrastructure
{
    public class MHsessionService
    {
        //Metody pre zjednoduseny zapi a citanie zo session
        public static void WriteLoginToSession(ISession session, string login)
        {
            session.SetString(AppData.SessionLogin, login);
        }

        public static string ReadLoginFromSession(ISession session)
        {
            string? login = session.GetString(AppData.SessionLogin);
            return login ?? string.Empty;
        }

        public static void WriteRoleToSession(ISession session, string role)
        {
            session.SetString(AppData.SessionRole, role);
        }

        public static void WriteRoleToSession(ISession session, USER_ROLE role)
        {
            session.SetString(AppData.SessionRole, role.ToString());
        }

        public static USER_ROLE ReadRoleFromSession(ISession session)
        {
            string? role = session.GetString(AppData.SessionRole);
            if (role == null)
            {
                return USER_ROLE.READONLY;
            }
            else
            {
                if( !string.IsNullOrEmpty(role) )
                {
                    return (USER_ROLE)Enum.Parse(typeof(USER_ROLE),  role); 
                }
                else
                {
                    return USER_ROLE.READONLY;
                }
            }
        }
    }
}
