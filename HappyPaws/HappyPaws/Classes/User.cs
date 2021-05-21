using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace HappyPaws.Classes
{
    [Table("User")]
    public class User
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }
}
