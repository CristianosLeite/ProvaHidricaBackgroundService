using ProvaHidrica.Models;
using ProvaHidrica.Types;

namespace ProvaHidrica.Interfaces
{
    public interface ILogRepository
    {
        Task SaveLog(Log log);
        Task<List<Log>> LoadLogs();
        Task<List<UserLog>> GetUserLogsByDate(string initialDate, string finalDate);
        Task<List<SysLog>> GetSysLogsByDate(string initialDate, string finalDate);
        Task LogUserLogin(User user);
        Task LogUserLogout(User user);
        Task LogUserEditUser(User user, string target, Context context);
        Task LogUserDeleteUser(User user, string target);
        Task LogSysSwitchedMode(string mode);
        Task LogSysPlcStatusChanged(string status);
        Task LogUserEditRecipe(User user, Recipe recipe, Context context);
        Task LogUserDeleteRecipe(User user, Recipe recipe);
    }
}
