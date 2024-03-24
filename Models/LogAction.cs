using BSBoilerPlate.Models;

namespace BSBoilerPlate.Models
{
    public class LogAction
    {
        public enum Names
        {
            Login,
            Logout,
            UserInsert,
            UserUpdate,
            UserDelete,
            RoleInsert,
            RoleUpdate,
            RoleDelete,
            ApplicationUpdate,
            ChatMessageSend,
        }

        public int ID { get; set; }
        public string Name { get; set; }
    }
}
