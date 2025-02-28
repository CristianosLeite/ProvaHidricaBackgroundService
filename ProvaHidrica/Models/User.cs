namespace ProvaHidrica.Models
{
    public class User(string id, string username, string badgeNumber, List<string> permissions)
    {
        public string Id { get; set; } = id;
        public string UserName { get; set; } = username;
        public string BadgeNumber { get; set; } = badgeNumber;
        public List<string> Permissions { get; set; } = permissions;
        public string StringPermissions => string.Join(", ", Permissions);

        public bool HasPermission(string permission)
        {
            return Permissions.Contains(permission);
        }
    }
}
