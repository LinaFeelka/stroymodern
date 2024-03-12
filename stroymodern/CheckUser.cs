using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stroymodern
{
    public class CheckUser
    {
        public string Login { get; set; }

        public bool IsAdmin { get; }

        public string Status => IsAdmin ? "Администратор" : "Менеджер";
        public CheckUser(string login, bool isAdmin)
        {
            Login = login.Trim();
            IsAdmin = isAdmin;
        }

    }
}
