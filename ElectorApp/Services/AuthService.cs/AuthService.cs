using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectorApp.Services.AuthService.cs
{
    internal class AuthService
    {
        internal String Login(string username, string password)
        {
            if (username == "admin" && password == "password")
            {
                return "Login successful";
            }
            else
            {
                return "Login failed";
            }
        }
    }
}
