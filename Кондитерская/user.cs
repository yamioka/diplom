using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Кондитерская
{
    internal class user
    {
        public static string Login { get; set; } // Логин пользователя
        public static string FullName { get; set; } // ФИО пользователя
        public static string Position { get; set; } // Должность пользователя

        public static DateTime CurrentDate { get; set; } = DateTime.Now;
    }
}
