using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BRovAm.data;

namespace CreateAdministrator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter a Username");
            string username = Console.ReadLine();
            Console.WriteLine("Enter a password");
            string password = Console.ReadLine();
            string passwordSalt = PasswordHelper.GenerateRandomSalt();
            string passwordhash = PasswordHelper.HashPassword(password, passwordSalt);
            Administrator admin = new Administrator
            {
                UserName = username,
                PasswordHash = passwordhash,
                PasswordSalt = passwordSalt
            };
            BRovAmManager manager = new BRovAmManager(Properties.Settings.Default.constr);
            manager.SignUp(admin);
        }
    }
}
