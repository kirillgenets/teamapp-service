using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamAppService.Models
{
    public class AuthConfirmation
    {
        public bool isAuth { get; set; }

        public AuthConfirmation(bool confirmation)
        {
            isAuth = confirmation;
        }
    }
}
