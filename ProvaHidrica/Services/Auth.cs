using ProvaHidrica.Database;
using ProvaHidrica.Models;
using ProvaHidrica.Types;
using ProvaHidrica.Windows;

namespace ProvaHidrica.Services
{
    public static class Auth
    {
        private static readonly Db db;
        public static User? LoggedInUser { get; private set; }
        public static string? LoggedAt { get; private set; }

        static Auth()
        {
            DbConnectionFactory connectionFactory = new();
            db = new(connectionFactory);
        }

        public async static void SetLoggedInUser(User user)
        {
            LoggedInUser = user;
            await db.LogUserLogin(user);

            _ = await Api.Authenticate(LoggedInUser!.BadgeNumber); // Notifies the server that the user has been authenticated
        }

        public static void SetLoggedAt(string time)
        {
            LoggedAt = time;
        }

        public static string GetUserId()
        {
            try
            {
                return LoggedInUser?.Id ?? "0";
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public static bool UserHasPermission(string permission)
        {
            return LoggedInUser?.HasPermission(permission) ?? false;
        }

        public async static void Logout()
        {
            await db.LogUserLogout(LoggedInUser!);
            LoggedInUser = null;
            LoggedAt = null;

            NfcWindow nfcWindow = new(Context.Login);
            nfcWindow.ShowDialog();
        }
    }
}
