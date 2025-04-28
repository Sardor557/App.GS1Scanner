using System;

namespace App.Shared.Models
{
    public class viUserLogin
    {
        public string Login { get; set; }
        public string Password { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is viUserLogin other)
                return Login == other.Login && Password == other.Password;
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Login, Password);
        }
    }
}
