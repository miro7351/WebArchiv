using Microsoft.AspNetCore.Http;
using System.Text.Json;


namespace SessionHelper
{
    //MH: april 2022
    //pozri: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-3.1
    //Pre ulozenie/nacitanie objektu do/zo session

    public static class SessionExtensionsMH
    {
        public static void SetMH<T>(this ISession session, string key, T value)
        {
            session.Set(key, JsonSerializer.SerializeToUtf8Bytes(value));//Nuget Newtonsoft.Json
        }


        public static T? GetMH<T>(this ISession session, string key)
        {
            byte[] data;
            T? s1 = default(T);

            if (session.TryGetValue(key, out data))
            {
                s1 = JsonSerializer.Deserialize<T>(data);
            }
            return s1;
        }

        //Pridany package: Microsoft.AspNetCore.Http.Extensions (2.2.0), tu je extension metoda pre Isession SetString(...) GetString(...)
        //Ak sa pouzije SetMH2, potom treba pouzit GetMH2
        /*
        public static void SetMH2<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
            //Nuget pridane: Microsoft.AspNetCore.Http.Extensions
        }

        public static T GetMH2<T>( this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
        */
    }
}
