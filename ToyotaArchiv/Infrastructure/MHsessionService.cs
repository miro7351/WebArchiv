using SessionHelper;
using ToyotaArchiv.Global;

namespace ToyotaArchiv.Infrastructure
{
    //MH: april 2022
    //Metody pre zjednoduseny zapis a citanie zo session
    public class MHsessionService : ISessionService 
    {
        public  void WriteUserToSession(ISession session, UserDetail userDetail)
        {
            session.SessionWrite(AppData.SessionUser, userDetail);  
        }

        public  UserDetail?  ReadUserFromSession(ISession session )
        {
            UserDetail? user = session.SessionRead<UserDetail>(AppData.SessionUser);
            return user;    
        }


        public  void WriteLoginToSession(ISession session, string login)
        {
            session.SetString(AppData.SessionLogin, login);
        }

        public  string ReadLoginFromSession(ISession session)
        {
            UserDetail? user = ReadUserFromSession(session);

            return user?.UserLogin ?? string.Empty; 
        }

        public string ReadRoleFromSession(ISession session)
        {
            UserDetail? user = ReadUserFromSession(session);

            return user?.UserRole ?? string.Empty;
        }

        public (string? userLogin, string? userRole) ReadUserLoginAndRoleFromSession(ISession session)
        {
            UserDetail? user = ReadUserFromSession(session);
            if (user != null)
            {
                return (user.UserLogin, user.UserRole);
            }
            return (null, null);    
        }


        //public static void WriteRoleToSession(ISession session, USER_ROLE role)
        //{
        //    session.SetString(AppData.SessionRole, role.ToString());
        //}




        //public static USER_ROLE ReadRoleFromSession(ISession session)
        //{
        //    string? role = session.GetString(AppData.SessionRole);
        //    if (role == null)
        //    {
        //        return USER_ROLE.READONLY;
        //    }
        //    else
        //    {
        //        if( !string.IsNullOrEmpty(role) )
        //        {
        //            return (USER_ROLE)Enum.Parse(typeof(USER_ROLE),  role); 
        //        }
        //        else
        //        {
        //            return USER_ROLE.READONLY;
        //        }
        //    }
        //}
    }
}
