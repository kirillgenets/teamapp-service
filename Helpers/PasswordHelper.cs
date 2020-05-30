using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamAppService.Helpers
{
    public class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            return Encoding.ASCII.GetString(System.Security.Cryptography.SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(password)));
        }

        public static bool IsCorrect(string originalPassword, string passwordToCompare)
        {
            return HashPassword(passwordToCompare) == originalPassword;
        }
    }
}
